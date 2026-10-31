using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace GestionClinicaNutricional.Infrastructure;

/// <summary>
/// Npgsql writes <c>timestamp with time zone</c> only from <see cref="DateTimeKind.Utc"/> values and throws
/// for local or unspecified ones, while the domain and the API receive <c>DateTime</c>s of any kind.
/// Local values are converted; unspecified ones (what JSON binding produces) are taken as UTC.
/// </summary>
internal sealed class UtcDateTimeConverter() : ValueConverter<DateTime, DateTime>(
    v => ToUtc(v),
    v => DateTime.SpecifyKind(v, DateTimeKind.Utc))
{
    internal static DateTime ToUtc(DateTime value) => value.Kind switch
    {
        DateTimeKind.Utc => value,
        DateTimeKind.Local => value.ToUniversalTime(),
        _ => DateTime.SpecifyKind(value, DateTimeKind.Utc)
    };
}

internal sealed class NullableUtcDateTimeConverter() : ValueConverter<DateTime?, DateTime?>(
    v => v.HasValue ? UtcDateTimeConverter.ToUtc(v.Value) : v,
    v => v.HasValue ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc) : v);
