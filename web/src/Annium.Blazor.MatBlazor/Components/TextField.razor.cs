using System;
using System.Collections.Generic;
using Annium.Blazor.Core.Tools;
using Annium.Components.State.Forms;
using Microsoft.AspNetCore.Components;

namespace Annium.Blazor.MatBlazor.Components
{
    public partial class TextField<TValue>
        where TValue : IEquatable<TValue>
    {
        [Parameter]
        public IAtomicContainer<TValue> State { get; set; } = null!;

        [Parameter]
        public string? Class { get; set; }

        [Parameter(CaptureUnmatchedValues = true)]
        public IReadOnlyDictionary<string, object> Attributes { get; set; } = null!;

        public string ClassName => _classBuilder.Clone().With(Class ?? string.Empty).Build();

        private readonly IClassBuilder _classBuilder;

        public TextField()
        {
            _classBuilder = ClassBuilder.With(() => State.IsStatus(Status.Error), "mdc-text-field--invalid");
        }
    }
}
