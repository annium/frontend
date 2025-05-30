using System;
using System.Collections.Generic;
using Annium.Blazor.Charts.Domain.Lookup;
using Annium.Data.Models;
using NodaTime;

namespace Annium.Blazor.Charts.Data.Sources;

public interface ISeriesSource<T> : ISeriesSource, IDisposable
{
    bool GetItems(Instant start, Instant end, out IReadOnlyList<T> data);
    T? GetItem(Instant moment, LookupMatch match = LookupMatch.Exact);
    void LoadItems(Instant start, Instant end);
}

public interface ISeriesSource
{
    event Action Loaded;
    event Action<ValueRange<Instant>> OnBoundsChange;
    Duration Resolution { get; }
    ValueRange<Instant> Bounds { get; }
    bool IsLoading { get; }
    void SetResolution(Duration resolution);
    void Clear();
}
