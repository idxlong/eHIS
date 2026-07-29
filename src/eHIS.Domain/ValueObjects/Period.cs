using eHIS.Domain.SeedWork;

namespace eHIS.Domain.ValueObjects;

public class Period : ValueObject
{
    public DateTime? Start { get; private set; }
    public DateTime? End { get; private set; }

    private Period() { } // EF Core

    public Period(DateTime? start, DateTime? end)
    {
        if (start.HasValue && end.HasValue && start.Value > end.Value)
        {
            throw new ArgumentException("Start date must be before or equal to End date.");
        }
        Start = start;
        End = end;
    }

    public static Period CreateOpen(DateTime start) => new(start, null);
    public static Period CreateClosed(DateTime start, DateTime end) => new(start, end);

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Start;
        yield return End;
    }
}
