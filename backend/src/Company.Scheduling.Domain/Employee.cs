namespace Company.Scheduling.Domain;

public class Employee
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";
    public List<EmployeeSkill> Skills { get; set; } = new();
    public List<AvailabilityWindow> Availability { get; set; } = new();
}

public class EmployeeSkill
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = "";
    public Guid EmployeeId { get; set; }
}
