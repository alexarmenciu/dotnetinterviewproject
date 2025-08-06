using API.Data;
using API.Models;
using API.Hubs;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.Services
{
    public class TaskService
    {
        private readonly AppDbContext _context;
        private readonly IHubContext<TaskHub>? _hubContext;

        public TaskService(AppDbContext context, IHubContext<TaskHub>? hubContext = null)
        {
            _context = context;
            _hubContext = hubContext;
        }

        public async Task<List<API.Models.Task>> GetAllTasksAsync()
        {
            return await _context.Tasks.ToListAsync();
        }

        public async Task<API.Models.Task> CreateTaskAsync(API.Models.Task task)
        {
            _context.Tasks.Add(task);
            await _context.SaveChangesAsync();
            
            // Notify clients about the new task
            if (_hubContext != null)
            {
                await _hubContext.Clients.Group("TaskUpdates").SendAsync("TaskCreated", task);
            }
            
            return task;
        }

        public async Task<API.Models.Task> UpdateTaskAsync(Guid id, API.Models.Task task)
        {
            if (id != task.Id)
            {
                throw new ArgumentException("Task ID mismatch.");
            }

            var existingTask = await _context.Tasks.FindAsync(id);
            if (existingTask == null)
            {
                throw new KeyNotFoundException("Task not found.");
            }

            existingTask.Title = task.Title;
            existingTask.Description = task.Description;
            existingTask.IsCompleted = task.IsCompleted;
            existingTask.DueDate = task.DueDate;

            _context.Entry(existingTask).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            
            // Notify clients about the updated task
            if (_hubContext != null)
            {
                await _hubContext.Clients.Group("TaskUpdates").SendAsync("TaskUpdated", existingTask);
            }
            
            return existingTask;
        }

        public async Task<API.Models.Task> DeleteTaskAsync(Guid id)
        {
            var task = await _context.Tasks.FindAsync(id);
            if (task != null)
            {
                _context.Tasks.Remove(task);
                await _context.SaveChangesAsync();
                
                // Notify clients about the deleted task
                if (_hubContext != null)
                {
                    await _hubContext.Clients.Group("TaskUpdates").SendAsync("TaskDeleted", id);
                }
                
                return task;
            }
            else
            {
                throw new KeyNotFoundException("Task not found.");
            }
        }

        public async Task<API.Models.Task> CompleteTaskAsync(Guid id)
        {
            var task = await _context.Tasks.FindAsync(id);
            if (task == null)
            {
                throw new KeyNotFoundException("Task not found.");
            }
            task.IsCompleted = true;
            _context.Entry(task).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            
            // Notify clients about the completed task
            if (_hubContext != null)
            {
                await _hubContext.Clients.Group("TaskUpdates").SendAsync("TaskCompleted", task);
            }
            
            return task;
        }
    }
}
