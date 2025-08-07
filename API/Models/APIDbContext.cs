using Microsoft.EntityFrameworkCore;
using API.Models;
using System;

namespace API.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<API.Models.Task> Tasks { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<API.Models.Task>().HasData(
                new API.Models.Task
                {
                    Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                    Title = "Sample Task 1",
                    Description = "This is the first seeded task.",
                    DueDate = new DateTime(2025, 8, 14, 0, 0, 0, DateTimeKind.Utc),
                    IsCompleted = false
                },
                new API.Models.Task
                {
                    Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                    Title = "Sample Task 2",
                    Description = "This is the second seeded task.",
                    DueDate = new DateTime(2025, 8, 26, 0, 0, 0, DateTimeKind.Utc),
                    IsCompleted = false
                }
            );
        }
    }
}