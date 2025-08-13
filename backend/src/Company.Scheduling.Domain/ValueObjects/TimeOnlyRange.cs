namespace Company.Scheduling.Domain.ValueObjects;

public record TimeOnlyRange(TimeOnly Start, TimeOnly End)
{
    public bool Overlaps(TimeOnlyRange other) => Start < other.End && other.Start < End;
}
