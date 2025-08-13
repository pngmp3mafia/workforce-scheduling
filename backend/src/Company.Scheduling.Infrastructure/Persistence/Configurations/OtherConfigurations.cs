using Company.Scheduling.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Company.Scheduling.Infrastructure.Persistence.Configurations;

public class EmployeeSkillConfiguration : IEntityTypeConfiguration<EmployeeSkill>
{
    public void Configure(EntityTypeBuilder<EmployeeSkill> b)
    {
        b.ToTable("EmployeeSkills");
        b.HasKey(x => x.Id);
        b.Property(x => x.Name).HasMaxLength(100).IsRequired();
    }
}

public class AvailabilityConfiguration : IEntityTypeConfiguration<AvailabilityWindow>
{
    public void Configure(EntityTypeBuilder<AvailabilityWindow> b)
    {
        b.ToTable("AvailabilityWindows");
        b.HasKey(x => x.Id);

        b.OwnsOne(x => x.DateRange, dr =>
        {
            dr.Property(p => p.Start).HasColumnName("StartDate");
            dr.Property(p => p.End).HasColumnName("EndDate");
        });

        b.OwnsOne(x => x.TimeRange, tr =>
        {
            tr.Property(p => p.Start).HasColumnName("StartTime");
            tr.Property(p => p.End).HasColumnName("EndTime");
        });
    }
}

public class ShiftSkillRequirementConfiguration : IEntityTypeConfiguration<ShiftSkillRequirement>
{
    public void Configure(EntityTypeBuilder<ShiftSkillRequirement> b)
    {
        b.ToTable("ShiftSkillRequirements");
        b.HasKey(x => x.Id);
        b.Property(x => x.Name).HasMaxLength(100).IsRequired();
        b.Property(x => x.Count).IsRequired();
    }
}

public class AssignmentConfiguration : IEntityTypeConfiguration<Assignment>
{
    public void Configure(EntityTypeBuilder<Assignment> b)
    {
        b.ToTable("Assignments");
        b.HasKey(x => x.Id);
        b.Property(x => x.EmployeeId).IsRequired();
        b.Property(x => x.ShiftId).IsRequired();
    }
}
