using Company.Scheduling.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Company.Scheduling.Infrastructure.Persistence.Configurations;

public class ShiftConfiguration : IEntityTypeConfiguration<Shift>
{
    public void Configure(EntityTypeBuilder<Shift> b)
    {
        b.ToTable("Shifts");
        b.HasKey(x => x.Id);
        b.Property(x => x.Date).IsRequired();

        b.OwnsOne(x => x.TimeRange, tr =>
        {
            tr.Property(p => p.Start).HasColumnName("StartTime");
            tr.Property(p => p.End).HasColumnName("EndTime");
        });

        b.HasMany(x => x.RequiredSkills)
         .WithOne()
         .HasForeignKey(x => x.ShiftId)
         .OnDelete(DeleteBehavior.Cascade);

        b.HasMany(x => x.Assignments)
         .WithOne()
         .HasForeignKey(x => x.ShiftId)
         .OnDelete(DeleteBehavior.Cascade);
    }
}
