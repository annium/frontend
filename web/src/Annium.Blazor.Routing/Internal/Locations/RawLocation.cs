using System.Collections.Generic;
using Annium.Net.Base;
using Microsoft.Extensions.Primitives;

namespace Annium.Blazor.Routing.Internal.Locations;

internal sealed record RawLocation
{
    public static RawLocation Parse(string uri)
    {
        if (!uri.Contains('?'))
            return new RawLocation(Helper.ParseTemplateParts(uri), new Dictionary<string, StringValues>());

        var (path, rawQuery, _) = uri.Split('?');
        var query = UriQuery.Parse(rawQuery);

        return new RawLocation(Helper.ParseTemplateParts(path), query);
    }

    public IReadOnlyList<string> Segments { get; }
    public IReadOnlyDictionary<string, StringValues> Parameters { get; }

    private RawLocation(IReadOnlyList<string> segments, IReadOnlyDictionary<string, StringValues> parameters)
    {
        Segments = segments;
        Parameters = parameters;
    }
}
