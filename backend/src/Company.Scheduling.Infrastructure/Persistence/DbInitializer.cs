using System.Linq;
using Microsoft.EntityFrameworkCore;
using Company.Scheduling.Domain;

namespace Company.Scheduling.Infrastructure.Persistence;

public static class DbInitializer
{
    public static async Task MigrateAndSeedAsync(SchedulingDbContext db, CancellationToken ct = default)
    {
        await db.Database.MigrateAsync(ct);

        if (!db.Employees.Any())
        {
            var emp = new Employee { FirstName = "Ava", LastName = "Nguyen" };
            emp.Skills.Add(new EmployeeSkill { Name = "Cashier" });
            emp.Skills.Add(new EmployeeSkill { Name = "Forklift" });
            db.Employees.Add(emp);

            var shift = new Shift
            {
                Date = DateOnly.FromDateTime(DateTime.Today.AddDays(1)),
                TimeRange = new(new(9, 0), new(17, 0))
            };
            shift.RequiredSkills.Add(new ShiftSkillRequirement { Name = "Cashier", Count = 1 });
            db.Shifts.Add(shift);

            await db.SaveChangesAsync(ct);
        }
    }
}