using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Annium.Data.Operations;
using Annium.Extensions.Validation;

namespace Annium.Components.State.Forms.Extensions;

/// <summary>
/// Provides extension methods for adding validation support to object containers.
/// </summary>
public static class ObjectContainerValidationExtensions
{
    /// <summary>
    /// Adds validation to an object container using the specified validator.
    /// Validation is triggered immediately when the container value changes.
    /// </summary>
    /// <typeparam name="T">The type of object being validated.</typeparam>
    /// <param name="state">The object container to add validation to.</param>
    /// <param name="validator">The validator to use for validation.</param>
    /// <returns>The same object container instance with validation enabled.</returns>
    public static IObjectContainer<T> UseValidator<T>(this IObjectContainer<T> state, IValidator<T> validator)
        where T : notnull, new()
    {
        return state.Changed.SubscribeValidator(state, validator);
    }

    /// <summary>
    /// Adds validation to an object container using the specified validator with throttling.
    /// Validation is triggered after the specified delay when the container value changes.
    /// </summary>
    /// <typeparam name="T">The type of object being validated.</typeparam>
    /// <param name="state">The object container to add validation to.</param>
    /// <param name="validator">The validator to use for validation.</param>
    /// <param name="dueTime">The delay before validation is triggered after value changes.</param>
    /// <returns>The same object container instance with throttled validation enabled.</returns>
    public static IObjectContainer<T> UseValidator<T>(
        this IObjectContainer<T> state,
        IValidator<T> validator,
        TimeSpan dueTime
    )
        where T : notnull, new()
    {
        return state.Changed.Throttle(dueTime).SubscribeValidator(state, validator);
    }

    /// <summary>
    /// Subscribes a validator to an observable stream to perform validation when events occur.
    /// </summary>
    /// <typeparam name="T">The type of object being validated.</typeparam>
    /// <param name="observable">The observable stream to subscribe to.</param>
    /// <param name="state">The object container to validate.</param>
    /// <param name="validator">The validator to use for validation.</param>
    /// <returns>The same object container instance.</returns>
    private static IObjectContainer<T> SubscribeValidator<T>(
        this IObservable<Unit> observable,
        IObjectContainer<T> state,
        IValidator<T> validator
    )
        where T : notnull, new()
    {
        var holder = new CtsHolder();
        observable.Subscribe(change =>
        {
            // atomically swap in a fresh token source and cancel the previous run (Interlocked guards the
            // reassign against overlapping notifications that could otherwise clobber each other's token).
            var next = new CancellationTokenSource();
            var previous = Interlocked.Exchange(ref holder.Cts, next);
            previous.Cancel();
            previous.Dispose();

            // fire-and-forget: do NOT block the (single-threaded, UI-bound) Changed callback on an awaitable —
            // that would deadlock a genuinely-async validator on Blazor WASM. ValidateAsync catches the
            // validator itself, so the discarded task cannot fault unobserved.
            _ = state.ValidateAsync(validator, next.Token);
        });

        return state;
    }

    /// <summary>
    /// Runs the validator against the container's value and updates child statuses. Sets children to
    /// <see cref="Status.Validating"/> up front; on completion (unless cancelled) applies labeled errors to their
    /// matching child and plain (unlabeled) errors — e.g. a thrown validator — to every child. A synchronous
    /// validator completes inline; a genuinely-async one resolves later without blocking the caller.
    /// </summary>
    /// <typeparam name="T">The type of object being validated.</typeparam>
    /// <param name="state">The object container to validate.</param>
    /// <param name="validator">The validator to use for validation.</param>
    /// <param name="ct">Cancellation token; if cancelled after the validator returns, the result is discarded.</param>
    /// <returns>A task that completes when validation has been applied (or discarded on cancellation).</returns>
    private static async Task ValidateAsync<T>(
        this IObjectContainer<T> state,
        IValidator<T> validator,
        CancellationToken ct
    )
        where T : notnull, new()
    {
        var children = state
            .Children.Where(x => x.Value is IStatusContainer)
            .ToDictionary(x => x.Key, x => (IStatusContainer)x.Value);

        using (state.Mute())
        {
            foreach (var child in children.Values)
                child.SetStatus(Status.Validating);
        }

        IResult result;
        try
        {
            result = await validator.ValidateAsync(state.Value);
        }
        catch (Exception exception)
        {
            result = Result.Create().Error(exception.Message);
        }

        if (ct.IsCancellationRequested)
            return;

        // plain (unlabeled) errors are not tied to a specific child, so apply them to every child rather than
        // silently dropping them (a thrown validator surfaces as a plain error).
        var plainMessage = result.PlainErrors.Count > 0 ? string.Join("; ", result.PlainErrors) : null;

        using (state.Mute())
        {
            foreach (var (name, child) in children)
                if (result.LabeledErrors.TryGetValue(name, out var errors))
                    child.SetStatus(Status.Error, string.Join("; ", errors));
                else if (plainMessage is not null)
                    child.SetStatus(Status.Error, plainMessage);
                else
                    child.SetStatus(Status.None);
        }
    }

    /// <summary>
    /// Mutable holder for the current validation cancellation source, enabling an atomic swap via
    /// <see cref="Interlocked.Exchange{T}(ref T, T)"/> (a captured local cannot be passed by ref).
    /// </summary>
    private sealed class CtsHolder
    {
        /// <summary>
        /// The cancellation source for the in-flight validation run; swapped atomically on each change.
        /// </summary>
        public CancellationTokenSource Cts = new();
    }
}
