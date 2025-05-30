using System;
using System.Collections.Generic;
using System.Reactive;
using Annium.Components.State.Forms.Extensions;
using Annium.Extensions.Validation;
using Annium.Testing;
using Xunit;

namespace Annium.Components.State.Forms.Tests;

public class ObjectContainerTest : TestBase
{
    public ObjectContainerTest(ITestOutputHelper outputHelper)
        : base(outputHelper) { }

    [Fact]
    public void Create_Ok()
    {
        // arrange
        var log = new List<Unit>();
        var factory = GetFactory();
        var initialValue = Arrange();

        // act
        var state = factory.CreateObject(initialValue);
        state.Changed.Subscribe(log.Add);

        // assert
        state.Value.IsEqual(initialValue);
        state.Children.At(nameof(User.Age)).Is(state.AtAtomic(x => x.Age));
        state.Children.At(nameof(User.Name)).Is(state.AtAtomic(x => x.Name));
        state.AtAtomic(x => x.Name).Value.Is(initialValue.Name);
        state.HasChanged.IsFalse();
        state.HasBeenTouched.IsFalse();
        state.IsStatus(Status.None).IsTrue();
        state.HasStatus(Status.None).IsTrue();
        log.IsEmpty();
    }

    [Fact]
    public void Set_Ok()
    {
        // arrange
        var log = new List<Unit>();
        var factory = GetFactory();
        var initialValue = Arrange();
        var otherValue = new User { Name = "Lex" };
        var state = factory.CreateObject(initialValue);
        state.Changed.Subscribe(log.Add);

        // act
        state.Set(initialValue).IsFalse();

        // assert
        state.Value.IsEqual(initialValue);
        state.AtAtomic(x => x.Name).Value.Is(initialValue.Name);
        state.HasChanged.IsFalse();
        state.HasBeenTouched.IsFalse();
        log.IsEmpty();

        // act
        state.Set(otherValue).IsTrue();

        // assert
        state.Value.IsEqual(otherValue);
        state.AtAtomic(x => x.Name).Value.Is(otherValue.Name);
        state.HasChanged.IsTrue();
        state.HasBeenTouched.IsTrue();
        log.Has(1);

        // act
        state.Set(initialValue).IsTrue();

        // assert
        state.Value.IsEqual(initialValue);
        state.AtAtomic(x => x.Name).Value.Is(initialValue.Name);
        state.HasChanged.IsFalse();
        state.HasBeenTouched.IsTrue();
        log.Has(2);
    }

    [Fact]
    public void Init_Ok()
    {
        // arrange
        var log = new List<Unit>();
        var factory = GetFactory();
        var initialValue = Arrange();
        var otherValue = new User { Name = "Lex" };
        var state = factory.CreateObject(initialValue);
        state.Changed.Subscribe(log.Add);

        // act
        state.Set(initialValue).IsFalse();

        // assert
        state.Value.IsEqual(initialValue);
        state.AtAtomic(x => x.Name).Value.Is(initialValue.Name);
        state.HasChanged.IsFalse();
        state.HasBeenTouched.IsFalse();
        log.IsEmpty();

        // act
        state.Set(otherValue).IsTrue();

        // assert
        state.Value.IsEqual(otherValue);
        state.AtAtomic(x => x.Name).Value.Is(otherValue.Name);
        state.HasChanged.IsTrue();
        state.HasBeenTouched.IsTrue();
        log.Has(1);

        // act
        state.Init(otherValue);

        // assert
        state.Value.IsEqual(otherValue);
        state.AtAtomic(x => x.Name).Value.Is(otherValue.Name);
        state.HasChanged.IsFalse();
        state.HasBeenTouched.IsFalse();
        log.Has(2);
    }

    [Fact]
    public void Reset_Ok()
    {
        // arrange
        var log = new List<Unit>();
        var factory = GetFactory();
        var initialValue = Arrange();
        var otherValue = new User { Name = "Lex" };
        var state = factory.CreateObject(initialValue);
        state.Changed.Subscribe(log.Add);

        // act
        state.Set(otherValue).IsTrue();
        state.AtAtomic(x => x.Name).SetStatus(Status.Validating);

        // assert
        state.Value.IsEqual(otherValue);
        state.HasChanged.IsTrue();
        state.HasBeenTouched.IsTrue();
        state.IsStatus(Status.None, Status.Validating).IsTrue();
        state.HasStatus(Status.Validating).IsTrue();
        log.Has(2);

        // act
        state.Reset();

        // assert
        state.Value.IsEqual(initialValue);
        state.AtAtomic(x => x.Name).Value.Is(initialValue.Name);
        state.HasChanged.IsFalse();
        state.HasBeenTouched.IsFalse();
        state.IsStatus(Status.None).IsTrue();
        state.HasStatus(Status.None).IsTrue();
        log.Has(3);
    }

    [Fact]
    public void Status_Ok()
    {
        // arrange
        var log = new List<Unit>();
        var factory = GetFactory();
        var initialValue = Arrange();
        var state = factory.CreateObject(initialValue);
        state.Changed.Subscribe(log.Add);

        // act
        state.AtAtomic(x => x.Name).SetStatus(Status.Validating);

        // assert
        state.IsStatus(Status.None, Status.Validating).IsTrue();
        state.IsStatus(Status.Validating).IsFalse();
        state.HasStatus(Status.None, Status.Validating).IsTrue();
        state.HasStatus(Status.None, Status.Error).IsTrue();
        state.HasStatus(Status.Error).IsFalse();
        log.Has(1);
    }

    [Fact]
    public void Validation_Ok()
    {
        // arrange
        var log = new List<Unit>();
        var factory = GetFactory();
        var validator = GetValidator<User>();
        var initialValue = Arrange();
        var state = factory.CreateObject(initialValue);
        state.Changed.Subscribe(log.Add);
        state.UseValidator(validator);

        // act
        state.Set(new User { Age = 10, Name = "No" });

        // assert
        state.HasStatus(Status.Error).IsTrue();

        // act
        state.Set(new User { Age = 20, Name = "Name" });

        // assert
        state.IsStatus(Status.None).IsTrue();

        // assert
        log.Has(2);
    }

    private User Arrange() =>
        new()
        {
            Name = "Max",
            Age = 20,
            Sex = Sex.Male,
        };

    private class User
    {
        public string Name { get; set; } = string.Empty;
        public int Age { get; set; }
        public Sex Sex { get; set; }
    }

    private enum Sex
    {
        Female,
        Male,
    }

    // ReSharper disable once UnusedType.Local
    private class UserValidator : Validator<User>
    {
        public UserValidator()
        {
            Field(x => x.Age).GreaterThan(18);
            Field(x => x.Name).Required().MinLength(3);
        }
    }
}
