using System.Linq;
using System.Collections.Generic;
using Company.Scheduling.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

// Request model for POST /employees

var builder = WebApplication.CreateBuilder(args);

// DbContext with retries + timeout
builder.Services.AddDbContext<SchedulingDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default"), sql =>
    {
        sql.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
        sql.CommandTimeout(30);
    }));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapGet("/health", () => Results.Ok(new { status = "ok", time = DateTime.UtcNow }));

app.MapGet("/employees", async (SchedulingDbContext db) =>
{
    var data = await db.Employees
        .Select(e => new
        {
            e.Id,
            Name = e.FirstName + " " + e.LastName,
            Skills = e.Skills.Select(s => s.Name).ToList()
        })
        .ToListAsync();

    return Results.Ok(data);
});

// Create a new employee with skills
app.MapPost("/employees", async (CreateEmployeeRequest req, SchedulingDbContext db) =>
{
    var e = new Company.Scheduling.Domain.Employee
    {
        FirstName = req.FirstName,
        LastName = req.LastName,
        Skills = req.Skills.Select(s => new Company.Scheduling.Domain.EmployeeSkill { Name = s }).ToList()
    };

    db.Employees.Add(e);
    await db.SaveChangesAsync();

    return Results.Created($"/employees/{e.Id}", new
    {
        e.Id,
        Name = e.FirstName + " " + e.LastName,
        Skills = e.Skills.Select(s => s.Name).ToList()
    });
});

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<SchedulingDbContext>();
    await DbInitializer.MigrateAndSeedAsync(db);
}

await app.RunAsync();
public record CreateEmployeeRequest(string FirstName, string LastName, List<string> Skills);