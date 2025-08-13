using Company.Scheduling.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Company.Scheduling.Infrastructure.Persistence.Configurations;

public class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
{
    public void Configure(EntityTypeBuilder<Employee> b)
    {
        b.ToTable("Employees");
        b.HasKey(x => x.Id);
        b.Property(x => x.FirstName).HasMaxLength(100).IsRequired();
        b.Property(x => x.LastName).HasMaxLength(100).IsRequired();

        b.HasMany(x => x.Skills)
         .WithOne()
         .HasForeignKey(x => x.EmployeeId)
         .OnDelete(DeleteBehavior.Cascade);

        b.HasMany(x => x.Availability)
         .WithOne()
         .HasForeignKey(x => x.EmployeeId)
         .OnDelete(DeleteBehavior.Cascade);
    }
}
