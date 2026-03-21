using FluentResults;

namespace Analytics.Application.Common;

public readonly record struct Period
{
    public DateTime StartUtc { get; }
    public DateTime EndUtc   { get; }

    private Period(DateTime startUtc, DateTime endUtc)
    {
        StartUtc = startUtc;
        EndUtc   = endUtc;
    }
    public static Period ForMonth(DateTime date)
    {
        var start = new DateTime(date.Year, date.Month, 1, 0, 0, 0, DateTimeKind.Utc);
        return new Period(start, start.AddMonths(1));
    }

    public static Result<Period> Create(DateTime startUtc, DateTime endUtc)
    {
        if (startUtc.Kind != DateTimeKind.Utc)
            return Result.Fail("StartUtc must be UTC");
            
        if (endUtc.Kind != DateTimeKind.Utc)
            return Result.Fail($"EndUtc must be UTC ({nameof(endUtc)})"); 
            
        if (endUtc < startUtc)
            return Result.Fail("EndUtc must be greater than StartUtc");

        return Result.Ok(new Period(startUtc, endUtc));
    }
    
    public static Result<Period> ResolvePeriod(DateTime? from, DateTime? to)
    {
        if (from is null && to is null)
            return Result.Ok(AllTime);

        if (from is null || to is null)
            return Result.Fail<Period>("Both fromDate and toDate must be provided, or neither.");

        return Create(
            DateTime.SpecifyKind(from.Value, DateTimeKind.Utc),
            DateTime.SpecifyKind(to.Value,   DateTimeKind.Utc));
    }

    public static Period AllTime => new(
        new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc), 
        DateTime.SpecifyKind(DateTime.MaxValue, DateTimeKind.Utc)
    );
    public bool IsDefault => StartUtc == default && EndUtc == default;

    public TimeSpan Duration => IsDefault ? TimeSpan.Zero : EndUtc - StartUtc;
    public double Days => Duration.TotalDays;
}