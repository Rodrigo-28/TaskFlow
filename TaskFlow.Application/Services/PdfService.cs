using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using TaskFlow.Application.Dtos.Response;
using TaskFlow.Application.Interfaces;

namespace TaskFlow.Application.Services
{
    public class PdfService : IPdfService
    {
        public async Task<byte[]> GenerateTasksOverviewPdf(IEnumerable<ScheduledTaskDto> tasks)
        {
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(1, Unit.Centimetre);
                    page.Header().Text("Catálogo de Tareas Programadas")
                                  .Bold().FontSize(18).AlignCenter();

                    page.Content().Table(tbl =>
                    {
                        tbl.ColumnsDefinition(cd =>
                        {
                            cd.ConstantColumn(30);  // Id
                            cd.RelativeColumn();    // Name
                            cd.ConstantColumn(60);  // Type
                            cd.RelativeColumn();    // Schedule
                            cd.ConstantColumn(60);  // LastRun
                        });

                        // Header
                        tbl.Header(h =>
                        {
                            h.Cell().Text("ID").Bold();
                            h.Cell().Text("Name").Bold();
                            h.Cell().Text("Type").Bold();
                            h.Cell().Text("Schedule").Bold();
                            h.Cell().Text("Last Run").Bold();
                        });

                        // Rows
                        foreach (var t in tasks)
                        {
                            tbl.Cell().Text(t.Id.ToString());
                            tbl.Cell().Text(t.Name);
                            tbl.Cell().Text(t.Type.ToString());
                            var sched = !string.IsNullOrWhiteSpace(t.CronExpression)
                                ? t.CronExpression
                                : t.ScheduledTime?.ToString("yyyy-MM-dd HH:mm");
                            tbl.Cell().Text(sched ?? "-");
                            tbl.Cell().Text(t.LastRunTime?.ToString("yyyy-MM-dd HH:mm") ?? "-");
                        }
                    });

                    page.Footer().AlignCenter().Text(x =>
                    {
                        x.Span("Página ");
                        x.CurrentPageNumber();
                        x.Span(" de ");
                        x.TotalPages();
                    });
                });
            });

            return document.GeneratePdf();
        }
    }
}
