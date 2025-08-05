
using API.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using API.Models;
using System.Linq;

namespace API.Controllers
{
    [ApiController]
    [Route("tasks")]
    public class TaskController : ControllerBase
    {
        private readonly TaskService _taskService;
        private readonly ILogger<TaskController> _logger;

        public TaskController(TaskService taskService, ILogger<TaskController> logger)
        {
            _taskService = taskService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetTasks()
        {
            try
            {
                var tasks = await _taskService.GetAllTasksAsync();
                return Ok(tasks);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving tasks");
                return StatusCode(500, new { 
                    message = "An unexpected error occurred while retrieving tasks.",
                    errorId = Guid.NewGuid().ToString()
                });
            }
        }

        [HttpPost]
        public async System.Threading.Tasks.Task<IActionResult> CreateTask([FromBody] API.Models.Task taskDto)
        {
            if (taskDto == null)
            {
                return BadRequest(new { 
                    message = "Task data is required.",
                    details = "The request body cannot be null or empty."
                });
            }

            if (!ModelState.IsValid)
            {
                var errors = ModelState
                    .Where(x => x.Value.Errors.Count > 0)
                    .ToDictionary(
                        kvp => kvp.Key,
                        kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                    );
                
                return BadRequest(new { 
                    message = "Validation failed.",
                    errors = errors
                });
            }

            try
            {
                await _taskService.CreateTaskAsync(taskDto);
                return Ok(taskDto);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid argument provided when creating task: {TaskId}", taskDto?.Id);
                return BadRequest(new { 
                    message = "Invalid data provided.",
                    details = ex.Message
                });
            }
            catch (Exception ex)
            {
                var errorId = Guid.NewGuid().ToString();
                _logger.LogError(ex, "An error occurred while creating task. ErrorId: {ErrorId}", errorId);
                return StatusCode(500, new { 
                    message = "An unexpected error occurred while creating the task.",
                    errorId = errorId
                });
            }
        }

        [HttpPut("{id}")]
        public async System.Threading.Tasks.Task<IActionResult> UpdateTask(Guid id, [FromBody] API.Models.Task taskDto)
        {
            if (taskDto == null)
            {
                return BadRequest(new { 
                    message = "Task data is required.",
                    details = "The request body cannot be null or empty."
                });
            }

            if (id != taskDto.Id)
            {
                return BadRequest(new { 
                    message = "ID mismatch.",
                    details = $"The task ID in the URL ({id}) does not match the task ID in the body ({taskDto.Id})."
                });
            }

            if (!ModelState.IsValid)
            {
                var errors = ModelState
                    .Where(x => x.Value.Errors.Count > 0)
                    .ToDictionary(
                        kvp => kvp.Key,
                        kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                    );
                
                return BadRequest(new { 
                    message = "Validation failed.",
                    errors = errors
                });
            }

            try
            {
                await _taskService.UpdateTaskAsync(id, taskDto);
                return Ok(taskDto);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning("Task not found during update: {TaskId}", id);
                return NotFound(new { 
                    message = "Task not found.",
                    details = $"No task found with ID: {id}"
                });
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid argument provided when updating task: {TaskId}", id);
                return BadRequest(new { 
                    message = "Invalid data provided.",
                    details = ex.Message
                });
            }
            catch (Exception ex)
            {
                var errorId = Guid.NewGuid().ToString();
                _logger.LogError(ex, "An error occurred while updating task {TaskId}. ErrorId: {ErrorId}", id, errorId);
                return StatusCode(500, new { 
                    message = "An unexpected error occurred while updating the task.",
                    errorId = errorId
                });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(Guid id)
        {
            try
            {
                await _taskService.DeleteTaskAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning("Task not found during deletion: {TaskId}", id);
                return NotFound(new { 
                    message = "Task not found.",
                    details = $"No task found with ID: {id}"
                });
            }
            catch (Exception ex)
            {
                var errorId = Guid.NewGuid().ToString();
                _logger.LogError(ex, "An error occurred while deleting task {TaskId}. ErrorId: {ErrorId}", id, errorId);
                return StatusCode(500, new { 
                    message = "An unexpected error occurred while deleting the task.",
                    errorId = errorId
                });
            }
        }

        [HttpPost("{id}/complete")]
        public async System.Threading.Tasks.Task<IActionResult> CompleteTask(Guid id)
        {
            try
            {
                await _taskService.CompleteTaskAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning("Task not found during completion: {TaskId}", id);
                return NotFound(new { 
                    message = "Task not found.",
                    details = $"No task found with ID: {id}"
                });
            }
            catch (Exception ex)
            {
                var errorId = Guid.NewGuid().ToString();
                _logger.LogError(ex, "An error occurred while completing task {TaskId}. ErrorId: {ErrorId}", id, errorId);
                return StatusCode(500, new { 
                    message = "An unexpected error occurred while completing the task.",
                    errorId = errorId
                });
            }
        }
    }
}