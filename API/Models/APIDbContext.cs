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
    }
}