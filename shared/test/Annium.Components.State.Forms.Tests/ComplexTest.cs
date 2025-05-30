using System;
using System.Collections.Generic;
using System.Reactive;
using Annium.Testing;
using Xunit;

// ReSharper disable UnusedAutoPropertyAccessor.Local

namespace Annium.Components.State.Forms.Tests;

public class ComplexTest : TestBase
{
    public ComplexTest(ITestOutputHelper outputHelper)
        : base(outputHelper) { }

    [Fact]
    public void Init_Ok()
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
        state.AtObject(x => x.Author).AtAtomic(x => x.Name).Value.Is(initialValue.Author.Name);
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
        var otherValue = ArrangeOther();
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
        state.AtObject(x => x.Author).Value.IsEqual(otherValue.Author);
        state.AtArray(x => x.Messages).Value.IsEqual(otherValue.Messages);
        state.HasChanged.IsTrue();
        state.HasBeenTouched.IsTrue();
        log.Has(1);

        // act
        state.Set(initialValue).IsTrue();

        // assert
        state.Value.IsEqual(initialValue);
        state.AtAtomic(x => x.Name).Value.Is(initialValue.Name);
        state.AtObject(x => x.Author).Value.IsEqual(initialValue.Author);
        state.AtArray(x => x.Messages).Value.IsEqual(initialValue.Messages);
        state.HasChanged.IsFalse();
        state.HasBeenTouched.IsTrue();
        log.Has(2);
    }

    [Fact]
    public void Reset_Ok()
    {
        // arrange
        var log = new List<Unit>();
        var factory = GetFactory();
        var initialValue = Arrange();
        var otherValue = ArrangeOther();
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
        state
            .AtArray(x => x.Messages)
            .AtObject(x => x[0])
            .AtAtomic(x => x.Text)
            .Value.Is(initialValue.Messages.At(0).Text);
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

    private Blog Arrange() =>
        new()
        {
            Name = "Sample",
            Author = new User { Name = "Max" },
            Messages = [new Message { Text = "one", IsRead = true }, new Message { Text = "two" }],
            EmbeddedDictionary = new Dictionary<string, Dictionary<int, Message>>
            {
                {
                    "a",
                    new Dictionary<int, Message>
                    {
                        {
                            1,
                            new Message { Text = "hey", IsRead = true }
                        },
                    }
                },
            },
        };

    private Blog ArrangeOther() =>
        new()
        {
            Name = "Demo",
            Author = new User { Name = "Lex" },
            Messages =
            [
                new Message { Text = "three", IsRead = true },
                new Message { Text = "four" },
                new Message { Text = "five", IsRead = true },
            ],
        };

    private class Blog
    {
        public string Name { get; set; } = string.Empty;
        public User Author { get; set; } = null!;
        public List<Message> Messages { get; set; } = [];
        public Dictionary<string, Dictionary<int, Message>> EmbeddedDictionary { get; set; } = new();
    }

    private class User
    {
        public string Name { get; set; } = string.Empty;
    }

    private class Message
    {
        public string Text { get; set; } = string.Empty;
        public bool IsRead { get; set; }
    }
}
