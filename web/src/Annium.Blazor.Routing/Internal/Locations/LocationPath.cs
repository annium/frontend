using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Annium.Blazor.Routing.Internal.Locations.Segments;
using Annium.Core.Mapper;

namespace Annium.Blazor.Routing.Internal.Locations;

internal class LocationPath : ILocationPath
{
    private static readonly Regex _paramRe = new(@"^\{([A-z0-9]+)\}$", RegexOptions.Compiled | RegexOptions.Singleline);

    public static (ILocationPath, IReadOnlyCollection<PropertyInfo>) Parse(
        string template,
        IReadOnlyCollection<PropertyInfo> properties,
        IMapper mapper
    )
    {
        var propertiesDictionary = properties.ToPropertiesDictionary();
        var pathProperties = new Dictionary<string, PropertyInfo>();
        var segments = Helper
            .ParseTemplateParts(template)
            .Select<string, ILocationSegment>(x =>
            {
                var match = _paramRe.Match(x);
                if (!match.Success)
                    return new FixedLocationSegment(x);

                var name = match.Groups[1].Value;
                if (!propertiesDictionary.TryGetValue(name, out var property))
                    throw new ArgumentException($"Path template '{template}' contains unknown parameter '{name}'");

                pathProperties[name] = property;

                return new ParamLocationSegment(name, property.PropertyType);
            })
            .ToArray();

        return (new LocationPath(segments, mapper), pathProperties.Values);
    }

    private readonly IReadOnlyList<ILocationSegment> _segments;
    private readonly IMapper _mapper;

    private LocationPath(IReadOnlyList<ILocationSegment> segments, IMapper mapper)
    {
        _segments = segments;
        _mapper = mapper;
    }

    public LocationMatch Match(IReadOnlyList<string> segments, PathMatch match)
    {
        if (segments.Count > _segments.Count)
            return LocationMatch.Empty;

        if (match == PathMatch.Exact && segments.Count != _segments.Count)
            return LocationMatch.Empty;

        var routeValues = new Dictionary<string, object?>();
        for (var i = 0; i < segments.Count; i++)
        {
            var segment = _segments[i];
            var raw = segments[i];

            switch (segment)
            {
                case FixedLocationSegment fixedSegment:
                    if (!fixedSegment.Match(raw))
                        return LocationMatch.Empty;
                    break;
                case ParamLocationSegment paramSegment:
                    var value = paramSegment.Match(raw, _mapper);
                    if (value is null)
                        return LocationMatch.Empty;
                    routeValues[paramSegment.Name] = value;
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        return new LocationMatch(true, routeValues);
    }

    public string Link(IReadOnlyDictionary<string, object?> parameters) =>
        string.Join(
            Constants.Separator,
            _segments.Select(x =>
            {
                if (x is FixedLocationSegment fs)
                    return fs.Part;
                if (x is ParamLocationSegment ps)
                    if (parameters.TryGetValue(ps.Name, out var value))
                        return _mapper.Map<string>(value);
                    else
                        throw new ArgumentException($"Path requires parameter '{ps.Name}'");

                throw new NotImplementedException($"Segment {x} is not supported");
            })
        );
}
