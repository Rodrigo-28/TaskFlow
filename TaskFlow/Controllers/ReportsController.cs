using Microsoft.AspNetCore.Mvc;
using TaskFlow.Application.Dtos.Response;
using TaskFlow.Application.Interfaces;

namespace TaskFlow.Controllers
{
    [ApiController]
    [Route("api/reports")]
    public class ReportsController : ControllerBase
    {
        private readonly IReportService _reportService;

        public ReportsController(IReportService reportService)
        {
            this._reportService = reportService;
        }
        [HttpGet("overview/latest")]
        [Produces("application/pdf")]
        public async Task<IActionResult> DownloadLatestOverview()
        {
            ReportFileDto pdf = await _reportService.GetLatestOverviewReportAsync();
            return File(
                pdf.Content,
                "application/pdf",
                pdf.FileName
                );
        }
    }
}
