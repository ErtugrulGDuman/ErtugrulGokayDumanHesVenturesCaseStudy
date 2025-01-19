using ErtugrulGokayDumanHesVenturesCaseStudy.Data;
using ErtugrulGokayDumanHesVenturesCaseStudy.Hangfire;
using Hangfire;
using Hangfire.SqlServer;
using Microsoft.EntityFrameworkCore;
using ErtugrulGokayDumanHesVenturesCaseStudy.Services.TrackingServices;
using ErtugrulGokayDumanHesVenturesCaseStudy.Services.PttScrapingServices;
using OpenQA.Selenium.Chrome;
using System.Reflection;

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

builder.Services.AddScoped<IPttScrapingService, PttScrapingService>(serviceProvider =>
{
    var logger = serviceProvider.GetRequiredService<ILogger<PttScrapingService>>();

    try
    {
        var options = new ChromeOptions();
        options.AddArguments("--headless");
        options.AddArguments("--no-sandbox");
        options.AddArguments("--disable-dev-shm-usage");
        options.AddArguments("--disable-gpu");
        options.AddArguments("--disable-features=SeleniumManager");

        // ChromeDriver'ýn yolunu belirleme
        var driverPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        var service = ChromeDriverService.CreateDefaultService(driverPath);
        service.SuppressInitialDiagnosticInformation = true;
        service.HideCommandPromptWindow = true;

        // Chrome binary'sinin yolunu manuel olarak belirtme
        var chromePath = @"C:\Program Files\Google\Chrome\Application\chrome.exe";
        if (File.Exists(chromePath))
        {
            options.BinaryLocation = chromePath;
        }

        var driver = new ChromeDriver(service, options);
        return new PttScrapingService(logger, driver);
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "ChromeDriver baþlatma hatasý: {Message}", ex.Message);
        throw;
    }
});

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

RecurringJob.AddOrUpdate<ITrackingService>(
    "update-tracking-statuses",
    service => service.UpdateStatusesAsync(),
    "*/1 * * * *"
);

app.Run();