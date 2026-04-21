using Global_Logistics_Management_System.Models.Entities;
using System.Collections.Generic;
using System.Reflection.Emit;
using Microsoft.EntityFrameworkCore;

namespace Global_Logistics_Management_System.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Client> Clients { get; set; }
        public DbSet<Contract> Contracts { get; set; }
        public DbSet<ServiceRequest> ServiceRequests { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Client
            modelBuilder.Entity<Client>(entity =>
            {
                entity.HasKey(e => e.ClientId);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.HasIndex(e => e.Name);
            });

            // Contract
            modelBuilder.Entity<Contract>(entity =>
            {
                entity.HasKey(e => e.ContractId);
                entity.Property(e => e.Status)
                      .HasConversion<string>()
                      .HasMaxLength(20);

                // Foreign key relationship with Client
                entity.HasOne(c => c.Client)
                      .WithMany(c => c.Contracts)
                      .HasForeignKey(c => c.ClientId)
                      .OnDelete(DeleteBehavior.Restrict);

                // Index for search by date and status
                entity.HasIndex(c => new { c.StartDate, c.EndDate, c.Status });
            });

            // ServiceRequest
            modelBuilder.Entity<ServiceRequest>(entity =>
            {
                entity.HasKey(e => e.ServiceRequestId);
                entity.Property(e => e.Status)
                      .HasConversion<string>()
                      .HasMaxLength(20);

                entity.HasOne(sr => sr.Contract)
                      .WithMany(c => c.ServiceRequests)
                      .HasForeignKey(sr => sr.ContractId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(sr => sr.CreatedAt);
            });
        }
    }
}
