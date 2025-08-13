namespace Company.Scheduling.Domain.ValueObjects;

public record DateOnlyRange(DateOnly Start, DateOnly End)
{
    public bool Contains(DateOnly d) => d >= Start && d <= End;
}
