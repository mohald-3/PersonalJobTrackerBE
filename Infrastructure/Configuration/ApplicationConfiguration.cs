using Domain.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Configuration
{
    public class ApplicationConfiguration : IEntityTypeConfiguration<JobApplication>
    {
        public void Configure(EntityTypeBuilder<JobApplication> builder)
        {
            builder.ToTable("Applications");

            builder.HasKey(a => a.Id);

            builder.Property(a => a.PositionTitle)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(a => a.Status)
                .IsRequired();

            builder.Property(a => a.AppliedDate);

            builder.Property(a => a.LastUpdated)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()"); // SQL Server default

            builder.Property(a => a.ContactEmail)
                .HasMaxLength(255);

            builder.Property(a => a.ContactPhone)
                .HasMaxLength(50);

            builder.Property(a => a.Source)
                .HasMaxLength(200);

            builder.Property(a => a.Priority);

            builder.Property(a => a.Notes)
                .HasMaxLength(2000);

            // Relationship side is already defined in CompanyConfiguration
            builder.HasOne(a => a.Company)
                .WithMany(c => c.Applications)
                .HasForeignKey(a => a.CompanyId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
