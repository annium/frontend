using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Annium.Components.State.Core;
using Annium.Core.Mapper;
using Annium.Data.Models.Extensions;
using Annium.Logging;

namespace Annium.Components.State.Forms.Internal;

internal class MapContainer<TKey, TValue> : ObservableState, IMapContainer<TKey, TValue>, ILogSubject
    where TKey : notnull
    where TValue : notnull, new()
{
    private static MethodInfo Factory { get; } = StateFactoryResolver.ResolveFactory(typeof(TValue));
    public Dictionary<TKey, TValue> Value => CreateValue();
    public bool HasChanged => !Value.IsShallowEqual(_initialValue, _mapper);
    public bool HasBeenTouched => _hasBeenTouched || _states.Values.Any(x => x.Ref.HasBeenTouched);
    public IReadOnlyCollection<TKey> Keys => _states.Keys.ToArray();
    public ILogger Logger { get; }
    private readonly IStateFactory _stateFactory;
    private readonly IMapper _mapper;
    private readonly IDictionary<TKey, StateReference> _states;
    private Dictionary<TKey, TValue> _initialValue;
    private bool _hasBeenTouched;

    public MapContainer(
        Dictionary<TKey, TValue> initialValue,
        IStateFactory stateFactory,
        IMapper mapper,
        ILogger logger
    )
    {
        _initialValue = initialValue;
        _stateFactory = stateFactory;
        _mapper = mapper;
        Logger = logger;
        _states = new Dictionary<TKey, StateReference>();
        Init(_initialValue);
    }

    public void Init(Dictionary<TKey, TValue> value)
    {
        _initialValue = value;
        using (Mute())
        {
            foreach (var key in _states.Keys.ToArray())
                if (!value.ContainsKey(key))
                    _states.Remove(key);
            foreach (var (key, item) in value)
            {
                if (_states.TryGetValue(key, out var state))
                    state.Ref.Init(item);
                else
                    AddInternal(key, item);
            }
        }

        _hasBeenTouched = false;

        NotifyChanged();
    }

    public bool Set(Dictionary<TKey, TValue> value)
    {
        var changed = false;
        using (Mute())
        {
            foreach (var key in _states.Keys.ToArray())
                if (!value.ContainsKey(key))
                    _states.Remove(key);
            foreach (var (key, item) in value)
            {
                if (_states.TryGetValue(key, out var state))
                    changed = state.Ref.Set(item) || changed;
                else
                {
                    AddInternal(key, item);
                    changed = true;
                }
            }
        }

        if (changed)
        {
            _hasBeenTouched = true;
            NotifyChanged();
        }

        return changed;
    }

    public void Reset() => Init(_initialValue);

    public bool IsStatus(params Status[] statuses)
    {
        foreach (var state in _states.Values)
            if (!state.Ref.IsStatus(statuses))
                return false;

        return true;
    }

    public bool HasStatus(params Status[] statuses)
    {
        foreach (var state in _states.Values)
            if (state.Ref.HasStatus(statuses))
                return true;

        return false;
    }

    public IObjectContainer<TI> AtObject<TI>(Expression<Func<Dictionary<TKey, TValue>, TI>> ex)
        where TI : notnull, new()
    {
        return At<IObjectContainer<TI>>(ex);
    }

    public IArrayContainer<TI> AtArray<TI>(Expression<Func<Dictionary<TKey, TValue>, List<TI>>> ex)
        where TI : notnull, new()
    {
        return At<IArrayContainer<TI>>(ex);
    }

    public IMapContainer<TK, TV> AtMap<TK, TV>(Expression<Func<Dictionary<TKey, TValue>, Dictionary<TK, TV>>> ex)
        where TK : notnull
        where TV : notnull, new()
    {
        return At<IMapContainer<TK, TV>>(ex);
    }

    public IAtomicContainer<TI> AtAtomic<TI>(Expression<Func<Dictionary<TKey, TValue>, TI>> ex)
    {
        return At<IAtomicContainer<TI>>(ex);
    }

    public void Add(TKey key, TValue item)
    {
        using (Mute())
            AddInternal(key, item);
        _hasBeenTouched = true;
        NotifyChanged();
    }

    public void Remove(TKey key)
    {
        using (Mute())
            RemoveInternal(key);
        _hasBeenTouched = true;
        NotifyChanged();
    }

    private TX At<TX>(LambdaExpression ex)
        where TX : ITrackedState
    {
        try
        {
            var key = ResolveKey(ex);
            if (!_states.ContainsKey(key))
                throw new IndexOutOfRangeException($"There's no item in container with key {key}");

            return (TX)_states[key].Ref;
        }
        catch (Exception e)
        {
            this.Error(e);
            throw;
        }
    }

    private Dictionary<TKey, TValue> CreateValue()
    {
        var value = new Dictionary<TKey, TValue>();

        foreach (var (key, state) in _states)
            value[key] = state.Ref.Value;

        return value;
    }

    private TKey ResolveKey(LambdaExpression ex)
    {
        if (
            ex.Body is MethodCallExpression { Method.IsSpecialName: true } body
            && body.Method.ReturnType == typeof(TValue)
        )
        {
            var parameters = body.Method.GetParameters();
            if (parameters.Length == 1 && parameters[0].ParameterType == typeof(TKey))
            {
                var value = Expression.Lambda(body.Arguments.ElementAt(0)).Compile().DynamicInvoke();
                if (value is TKey key)
                    return key;
            }
        }

        throw new ArgumentException($"{ex} is not a valid dictionary index expression");
    }

    private void AddInternal(TKey key, TValue item)
    {
        var state = (IValueTrackedState<TValue>)Factory.Invoke(_stateFactory, [(object)item])!;
        _states[key] = new StateReference(state, state.Changed.Subscribe(_ => NotifyChanged()));
    }

    private void RemoveInternal(TKey key)
    {
        _states[key].Subscription.Dispose();
        _states.Remove(key);
    }

    private class StateReference
    {
        public IValueTrackedState<TValue> Ref { get; }
        public IDisposable Subscription { get; }

        public StateReference(IValueTrackedState<TValue> @ref, IDisposable subscription)
        {
            Ref = @ref;
            Subscription = subscription;
        }

        public override string ToString() => Ref.GetType().FriendlyName();
    }
}
