using Global_Logistics_Management_System.Data;
using Global_Logistics_Management_System.DesignPatterns.Currency;
using Global_Logistics_Management_System.DesignPatterns.Observer;
using Global_Logistics_Management_System.Services;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Logging
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();
builder.Host.UseSerilog();

// Database
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Design Patterns
builder.Services.AddHttpClient<ICurrencyConversionStrategy, ExchangeRateApiStrategy>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["CurrencyApi:BaseUrl"] ?? "https://api.exchangerate-api.com/v4/");
});
builder.Services.AddScoped<CurrencyConverter>();

builder.Services.AddScoped<LoggingObserver>();
builder.Services.AddScoped<EmailNotificationObserver>();
builder.Services.AddScoped<ServiceRequestSubject>(sp =>
{
    var subject = new ServiceRequestSubject();
    subject.Attach(sp.GetRequiredService<LoggingObserver>());
    subject.Attach(sp.GetRequiredService<EmailNotificationObserver>());
    return subject;
});

// Business Services
builder.Services.AddScoped<ContractValidationService>();
builder.Services.AddScoped<SearchService>();
builder.Services.AddScoped<FileService>();
builder.Services.AddHttpClient<ICurrencyService, CurrencyService>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["CurrencyApi:BaseUrl"] ?? "https://api.exchangerate-api.com/v4/");
});
// MVC
builder.Services.AddControllersWithViews();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Contracts}/{action=Index}/{id?}")
    .WithStaticAssets();

// seed database on startup
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await DbInitializer.InitializeAsync(db);
}

app.Run();
