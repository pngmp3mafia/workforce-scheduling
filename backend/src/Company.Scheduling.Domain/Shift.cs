using Company.Scheduling.Api.Contracts;
using Company.Scheduling.Domain;
using Company.Scheduling.Domain.ValueObjects;
namespace Company.Scheduling.Domain;

public class Shift
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public DateOnly Date { get; set; }
    public TimeOnlyRange TimeRange { get; set; } = new(new(9,0), new(17,0));
    public List<ShiftSkillRequirement> RequiredSkills { get; set; } = new();
    public List<Assignment> Assignments { get; set; } = new();
}

public class ShiftSkillRequirement
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = "";
    public int Count { get; set; } = 1;
    public Guid ShiftId { get; set; }
}

public class Assignment
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid EmployeeId { get; set; }
    public Guid ShiftId { get; set; }
}
