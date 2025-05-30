using Annium.Data.Models;
using NodaTime;

namespace Annium.Blazor.Charts.Extensions;

public static class InstantExtensions
{
    private static readonly DateTimeZone _timeZone = DateTimeZoneProviders.Tzdb.GetSystemDefault();

    public static string S(this Instant t) => t.InZone(_timeZone).LocalDateTime.ToString("dd.MM.yyyy HH:mm", null);

    public static string S(this ValueRange<Instant> r) => $"{r.Start.S()} - {r.End.S()}";
}
