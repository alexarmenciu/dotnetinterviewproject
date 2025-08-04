using API.Models;
using API.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using API.Data; // <-- Add this line

namespace API.Tests.Services
{
    public class TaskServiceTests
    {
        private AppDbContext GetDbContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<API.Data.AppDbContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;
            return new API.Data.AppDbContext(options);
        }

        [Fact]
        public async System.Threading.Tasks.Task CreateTaskAsync_AddsTask()
        {
            var context = GetDbContext(Guid.NewGuid().ToString());
            var service = new TaskService(context);
            var task = new API.Models.Task {
                Id = Guid.NewGuid(),
                Title = "Test",
                Description = "Desc",
                DueDate = DateTime.UtcNow.AddDays(1),
                IsCompleted = false
            };

            var result = await service.CreateTaskAsync(task);

            Assert.Equal(task.Title, result.Title);
            Assert.Single(context.Tasks);
        }

        [Fact]
        public async System.Threading.Tasks.Task GetAllTasksAsync_ReturnsAllTasks()
        {
            var context = GetDbContext(Guid.NewGuid().ToString());
            context.Tasks.Add(new API.Models.Task {
                Id = Guid.NewGuid(),
                Title = "T1",
                Description = "Desc1",
                DueDate = DateTime.UtcNow.AddDays(1),
                IsCompleted = false
            });
            context.Tasks.Add(new API.Models.Task {
                Id = Guid.NewGuid(),
                Title = "T2",
                Description = "Desc2",
                DueDate = DateTime.UtcNow.AddDays(2),
                IsCompleted = false
            });
            await context.SaveChangesAsync();
            var service = new TaskService(context);

            var result = await service.GetAllTasksAsync();

            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async System.Threading.Tasks.Task UpdateTaskAsync_UpdatesTask()
        {
            var context = GetDbContext(Guid.NewGuid().ToString());
            var id = Guid.NewGuid();
            var task = new API.Models.Task {
                Id = id,
                Title = "Old",
                Description = "OldDesc",
                DueDate = DateTime.UtcNow.AddDays(1),
                IsCompleted = false
            };
            context.Tasks.Add(task);
            await context.SaveChangesAsync();
            var service = new TaskService(context);
            var updated = new API.Models.Task {
                Id = id,
                Title = "New",
                Description = "NewDesc",
                DueDate = DateTime.UtcNow.AddDays(2),
                IsCompleted = true
            };

            var result = await service.UpdateTaskAsync(id, updated);

            Assert.Equal("New", result.Title);
            Assert.True(result.IsCompleted);
        }

        [Fact]
        public async System.Threading.Tasks.Task DeleteTaskAsync_RemovesTask()
        {
            var context = GetDbContext(Guid.NewGuid().ToString());
            var id = Guid.NewGuid();
            var task = new API.Models.Task {
                Id = id,
                Title = "ToDelete",
                Description = "Desc",
                DueDate = DateTime.UtcNow.AddDays(1),
                IsCompleted = false
            };
            context.Tasks.Add(task);
            await context.SaveChangesAsync();
            var service = new TaskService(context);

            var result = await service.DeleteTaskAsync(id);

            Assert.Equal(id, result.Id);
            Assert.Empty(context.Tasks);
        }

        [Fact]
        public async System.Threading.Tasks.Task CompleteTaskAsync_SetsIsCompleted()
        {
            var context = GetDbContext(Guid.NewGuid().ToString());
            var id = Guid.NewGuid();
            var task = new API.Models.Task {
                Id = id,
                Title = "ToComplete",
                Description = "Desc",
                DueDate = DateTime.UtcNow.AddDays(1),
                IsCompleted = false
            };
            context.Tasks.Add(task);
            await context.SaveChangesAsync();
            var service = new TaskService(context);

            var result = await service.CompleteTaskAsync(id);

            Assert.True(result.IsCompleted);
        }

        [Fact]
        public async System.Threading.Tasks.Task UpdateTaskAsync_ThrowsIfNotFound()
        {
            var context = GetDbContext(Guid.NewGuid().ToString());
            var service = new TaskService(context);
            var id = Guid.NewGuid();
            var task = new API.Models.Task {
                Id = id,
                Title = "X",
                Description = "Desc",
                DueDate = DateTime.UtcNow.AddDays(1),
                IsCompleted = false
            };

            await Assert.ThrowsAsync<KeyNotFoundException>(() => service.UpdateTaskAsync(id, task));
        }

        [Fact]
        public async System.Threading.Tasks.Task DeleteTaskAsync_ThrowsIfNotFound()
        {
            var context = GetDbContext(Guid.NewGuid().ToString());
            var service = new TaskService(context);
            var id = Guid.NewGuid();

            await Assert.ThrowsAsync<KeyNotFoundException>(() => service.DeleteTaskAsync(id));
        }

        [Fact]
        public async System.Threading.Tasks.Task CompleteTaskAsync_ThrowsIfNotFound()
        {
            var context = GetDbContext(Guid.NewGuid().ToString());
            var service = new TaskService(context);
            var id = Guid.NewGuid();

            await Assert.ThrowsAsync<KeyNotFoundException>(() => service.CompleteTaskAsync(id));
        }
    }
}
