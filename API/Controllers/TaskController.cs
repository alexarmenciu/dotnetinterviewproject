using API.Services.TaskService;
[ApiController]
[Route("tasks/[controller]")]
class TaskController : ControllerBase
{
    private readonly TaskService _taskService;

    public TaskController(TaskService taskService)
    {
        _taskService = taskService;
    }
    [HttpGet]
    public async Task<IActionResult> GetTasks()
    {
        try
        {
            var tasks = await _taskService.GetTasksAsync();
            return Ok(tasks);
        }
        catch (Exception)
        {
            return StatusCode(500, "An unexpected error occurred.");
        }
    }

    [HttpPost]
    public async Task<IActionResult> CreateTask([FromBody] TaskDto taskDto)
    {
        if (taskDto == null)
        {
            return BadRequest("Task data is null.");
        }
        try
        {
            var createdTask = await _taskService.CreateTaskAsync(taskDto);
            return CreatedAtAction(nameof(GetTaskById), new { id = createdTask.Id }, createdTask);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception)
        {
            return StatusCode(500, "An unexpected error occurred.");
        }
    }
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTask(int id, [FromBody] TaskDto taskDto)
    {
        if (taskDto == null || id != taskDto.Id)
        {
            return BadRequest("Task data is null or ID mismatch.");
        }
        try
        {
            var updatedTask = await _taskService.UpdateTaskAsync(taskDto);
            if (updatedTask == null)
            {
                return NotFound();
            }
            return Ok(updatedTask);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception)
        {
            return StatusCode(500, "An unexpected error occurred.");
        }
    }
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTask(int id)
    {
        try
        {
            var deleted = await _taskService.DeleteTaskAsync(id);
            if (!deleted)
            {
                return NotFound();
            }
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (Exception)
        {
            return StatusCode(500, "An unexpected error occurred.");
        }
    }   
}