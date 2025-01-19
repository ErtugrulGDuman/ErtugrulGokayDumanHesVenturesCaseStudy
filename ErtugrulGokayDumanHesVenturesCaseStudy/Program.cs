using ErtugrulGokayDumanHesVenturesCaseStudy.Data;
using ErtugrulGokayDumanHesVenturesCaseStudy.Hangfire;
using Hangfire;
using Hangfire.SqlServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using ErtugrulGokayDumanHesVenturesCaseStudy.Services.TrackingServices;
using ErtugrulGokayDumanHesVenturesCaseStudy.Services.PttScrapingServices;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddHangfire(configuration => configuration
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UseSqlServerStorage(builder.Configuration.GetConnectionString("HangfireConnection")));

builder.Services.AddHttpClient();
builder.Services.AddHangfireServer();
builder.Services.AddScoped<ITrackingService, TrackingService>();
builder.Services.AddScoped<IPttScrapingService, PttScrapingService>();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.UseHangfireDashboard();
app.MapControllers();
app.UseCors(policy => policy
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader());

RecurringJob.AddOrUpdate<ITrackingService>(
    "update-tracking-statuses",
    service => service.UpdateStatusesAsync(),
    "*/10 * * * * *" // Düzeltilmiş cron ifadesi
);

app.Run();