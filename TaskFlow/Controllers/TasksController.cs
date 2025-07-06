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
            var task = await _taskService.GetAllTasksAsync();
            return Ok(task);

        }
        // GET /api/tasks/{id}
        [HttpGet("{id:int}")]
        public async Task<ActionResult<ScheduledTaskDto>> Get(int id)
        {
            var task = await _taskService.GetTaskByIdAsync(id);
            return Ok(task);
        }
        // POST /api/tasks
        [HttpPost("email")]
        public async Task<ActionResult<EmailTaskDto>> CreateEmail([FromBody] CreateEmailTaskDto dto)
        {
            var created = await _taskService.CreateEmailTaskAsync(dto);
            return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
        }
        [HttpPost("pdf-report")]
        public async Task<ActionResult<PdfReportTaskDto>> CreatedPdfReport([FromBody] CreatePdfReportTaskDto dto)
        {
            var created = await _taskService.CreatePdfReportTaskAsync(dto);
            return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
        }
        [HttpPost("data-cleanup")]
        public async Task<ActionResult<DataCleanupTaskDto>> CreateDataCleanup([FromBody] CreateDataCleanupTaskDto dto)
        {
            var created = await _taskService.CreateDataCleanupTaskAsync(dto);
            return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
        }





        // PUT /api/tasks/{id}
        [HttpPut("email/{id:int}")]
        public async Task<ActionResult<EmailTaskDto>> UpdateEmail(int id, [FromBody] UpdateEmailTaskDto dto)
        {
            await _taskService.UpdateEmailTaskAsync(id, dto);
            return Ok(dto);
        }
        [HttpPut("pdf-report/{id:int}")]
        public async Task<ActionResult<PdfReportTaskDto>> UpdatePdfReport(int id, [FromBody] UpdatePdfReportTaskDto dto)
        {
            await _taskService.UpdatePdfReportTaskAsync(id, dto);
            return Ok(dto);
        }
        [HttpPut("data-cleanup/{id:int}")]
        public async Task<ActionResult> UpdateDataCleanup(int id, [FromBody] UpdateDataCleanupTaskDto dto)
        {
            await _taskService.UpdateDataCleanupTaskAsync(id, dto);
            return Ok(dto);
        }
        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            await _taskService.DeleteTaskAsync(id);
            return NoContent();
        }
        // POST /api/tasks/{id}/execute
        [HttpPost("{id:int}/execute")]
        public async Task<ActionResult> ExecuteNow(int id)
        {
            await _taskService.ExecuteTaskAsync(id);
            return Accepted();
        }
    }

}



