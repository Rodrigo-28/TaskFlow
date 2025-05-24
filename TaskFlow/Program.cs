using Hangfire;
using Microsoft.EntityFrameworkCore;
using TaskFlow.Application.Extensions;
using TaskFlow.Infrastructure.Contexts;
using TaskFlow.Infrastructure.Extensions;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
//Infrastructure
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddApplicationServices();

builder.Services.AddControllers();
//Configure DB Context
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
);
//HangfireServices
builder.Services.AddHangfireServices(builder.Configuration);
builder.Services.AddSwaggerGen();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHangfireDashboard("/hangfire");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
