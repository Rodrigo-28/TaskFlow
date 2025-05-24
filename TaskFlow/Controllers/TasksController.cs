using Microsoft.AspNetCore.Mvc;
using TaskFlow.Application.Dtos.Requests;
using TaskFlow.Application.Dtos.Response;
using TaskFlow.Application.Interfaces;

namespace TaskFlow.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TasksController : ControllerBase
    {
        private readonly ITaskService _taskService;

        public TasksController(ITaskService taskService)
        {
            this._taskService = taskService;
        }

        // GET /api/tasks
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ScheduledTaskDto>>> GetAll()
        {
            var task = await _taskService.GetAllTasks();
            return Ok(task);

        }
        // GET /api/tasks/{id}
        [HttpGet("{id:int}")]
        public async Task<ActionResult<ScheduledTaskDto>> Get(int id)
        {
            var task = await _taskService.GetTask(id);
            return Ok(task);
        }
        // POST /api/tasks
        [HttpPost]
        public async Task<ActionResult<ScheduledTaskDto>> Create([FromBody] CreateTaskDto createTaskDto)
        {
            var created = await _taskService.CreateTask(createTaskDto);
            return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
        }
        // PUT /api/tasks/{id}
        [HttpPut("{id:int}")]
        public async Task<ActionResult<ScheduledTaskDto>> Update(int id, [FromBody] UpdateTaskDto updateTaskDto)
        {
            var updated = await _taskService.UpdateTask(id, updateTaskDto);
            return Ok(updated);
        }
        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            await _taskService.DeleteTask(id);
            return NoContent();
        }
    }
}
