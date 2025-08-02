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
        var tasks = await _taskService.GetTasksAsync();
        return Ok(tasks);
    }

    [HttpPost]
    public async Task<IActionResult> CreateTask([FromBody] TaskDto taskDto)
    {
        if (taskDto == null)
        {
            return BadRequest("Task data is null.");
        }
        var createdTask = await _taskService.CreateTaskAsync(taskDto);
        return CreatedAtAction(nameof(GetTaskById), new { id = createdTask.Id }, createdTask);
    }
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTask(int id, [FromBody] TaskDto taskDto)
    {
        if (taskDto == null || id != taskDto.Id)
        {
            return BadRequest("Task data is null or ID mismatch.");
        }
        var updatedTask = await _taskService.UpdateTaskAsync(taskDto);
        if (updatedTask == null)
        {
            return NotFound();
        }
        return Ok(updatedTask);
    }
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTask(int id)
    {
        var deleted = await _taskService.DeleteTaskAsync(id);
        if (!deleted)
        {
            return NotFound();
        }
        return NoContent();
    }   
}