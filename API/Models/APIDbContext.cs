using Microsoft.EntityFrameworkCore;
using API.Models;
using System;

namespace API.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Task> Tasks { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
            var folder = Environment.SpecialFolder.LocalApplicationData;
            var path = Environment.GetFolderPath(folder);
            DbPath = System.IO.Path.Join(path, "tasks.db");
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite($"Data Source={DbPath}");
    }
}