namespace ValueObjects.ValueObject;

public record struct DateRange 
{
    public DateRange(DateOnly startDate, DateOnly? endDate)
    {
        if (endDate.HasValue && endDate < startDate)
            throw new Exception($"Дата закінчення {endDate} не може бути раніше дати початку {startDate}");
        StartDate = startDate;
        EndDate = endDate;
    }

    public DateOnly StartDate { get; }
    public DateOnly? EndDate { get; }
    
    
    public static DateRange StartingAt(DateOnly startDate) => new DateRange(startDate, null);
    public static DateRange Between(DateOnly startDate, DateOnly endDate) => new DateRange(startDate, endDate);
    
    public int? DurationInDays =>
        EndDate is null ? null : EndDate.Value.DayNumber - StartDate.DayNumber + 1;

    public override string ToString() =>
        EndDate is null ? $"{StartDate} → ∞" : $"{StartDate} → {EndDate}";
    
}