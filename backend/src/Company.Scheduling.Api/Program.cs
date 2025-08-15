using System.Linq;
using System.Collections.Generic;
using Company.Scheduling.Api.Contracts;
using Company.Scheduling.Domain;
using Company.Scheduling.Domain.ValueObjects;
using Company.Scheduling.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

var cfg = builder.Configuration;

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
  .AddJwtBearer(o =>
  {
    o.TokenValidationParameters = new TokenValidationParameters
    {
      ValidateIssuer = true,
      ValidateAudience = true,
      ValidateIssuerSigningKey = true,
      ValidIssuer = cfg["Jwt:Issuer"],
      ValidAudience = cfg["Jwt:Audience"],
      IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(cfg["Jwt:Key"]!))
    };
  });

builder.Services.AddAuthorization();

builder.Services.AddSwaggerGen(c =>
{
  c.SwaggerDoc("v1", new OpenApiInfo { Title = "Scheduling API", Version = "v1" });
  var jwt = new OpenApiSecurityScheme
  {
    Name = "Authorization",
    Type = SecuritySchemeType.Http,
    Scheme = "bearer",
    BearerFormat = "JWT",
    In = ParameterLocation.Header,
    Description = "Enter JWT only (no 'Bearer ' prefix).",
    Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = JwtBearerDefaults.AuthenticationScheme }
  };
  c.AddSecurityDefinition(jwt.Reference.Id, jwt);
  c.AddSecurityRequirement(new OpenApiSecurityRequirement { { jwt, Array.Empty<string>() } });
});


// CORS for the frontend
builder.Services.AddCors(o => o.AddDefaultPolicy(p =>
    p.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));

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

app.UseCors();
app.UseSwagger();
app.UseSwaggerUI();
app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/health", () => Results.Ok(new { status = "ok", time = DateTime.UtcNow }));

// ---------------- Employees ----------------
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

app.MapPost("/employees", async (CreateEmployeeRequest req, SchedulingDbContext db) =>
{
    if (req is null) return Results.BadRequest(new { error = "Body is required." });

    var skills = (req.Skills ?? new List<string>())
        .Where(s => !string.IsNullOrWhiteSpace(s))
        .Select(s => new EmployeeSkill { Name = s.Trim() })
        .ToList();

    var e = new Employee
    {
        FirstName = req.FirstName ?? "",
        LastName = req.LastName ?? "",
        Skills = skills
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

// ---------------- Shifts ----------------
app.MapGet("/shifts", async (SchedulingDbContext db) =>
{
    var data = await db.Shifts
        .Select(s => new
        {
            s.Id,
            s.Date,
            Start = s.TimeRange.Start,
            End = s.TimeRange.End,
            Required = s.RequiredSkills.Select(r => new { r.Name, r.Count }).ToList()
        })
        .ToListAsync();

    return Results.Ok(data);
});

app.MapPost("/shifts", async (CreateShiftRequest req, SchedulingDbContext db) =>
{
    if (req is null)
        return Results.BadRequest(new { error = "Body is required." });

    if (!DateOnly.TryParse(req.Date, out var date) ||
        !TimeOnly.TryParse(req.Start, out var start) ||
        !TimeOnly.TryParse(req.End, out var end) ||
        start >= end)
    {
        return Results.BadRequest(new { error = "Use date=YYYY-MM-DD, start/end=HH:mm, and ensure start < end." });
    }

    var reqSkills = (req.RequiredSkills ?? new List<RequiredSkillDto>())
        .Where(r => !string.IsNullOrWhiteSpace(r.Name));

    var shift = new Shift
    {
        Date = date,
        TimeRange = new TimeOnlyRange(start, end),
        RequiredSkills = reqSkills
            .Select(r => new ShiftSkillRequirement
            {
                Name = r.Name.Trim(),
                Count = r.Count < 1 ? 1 : r.Count
            })
            .ToList()
    };

    db.Shifts.Add(shift);
    await db.SaveChangesAsync();

    var payload = new
    {
        shift.Id,
        shift.Date,
        Start = shift.TimeRange.Start,
        End = shift.TimeRange.End,
        Required = shift.RequiredSkills.Select(r => new { r.Name, r.Count }).ToList()
    };

    return Results.Created($"/shifts/{shift.Id}", payload);
});

// Migrate & seed
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<SchedulingDbContext>();
    await DbInitializer.MigrateAndSeedAsync(db);
}

await app.RunAsync();

// ---------- DTOs (keep below top-level statements) ----------
public record CreateEmployeeRequest(string FirstName, string LastName, List<string> Skills);
public record RequiredSkillDto(string Name, int Count = 1);
public record CreateShiftRequest(string Date, string Start, string End, List<RequiredSkillDto> RequiredSkills);
