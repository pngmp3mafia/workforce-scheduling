using Company.Scheduling.Domain.ValueObjects;

namespace Company.Scheduling.Domain;

public class AvailabilityWindow
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid EmployeeId { get; set; }
    public DateOnlyRange DateRange { get; set; } = new(DateOnly.FromDateTime(DateTime.Today),
                                                      DateOnly.FromDateTime(DateTime.Today));
    public TimeOnlyRange TimeRange { get; set; } = new(new(9,0), new(17,0));
}
