using LibraryApplicationApi.Data;
using LibraryApplicationApi.Middleware;
using Microsoft.EntityFrameworkCore;
using Serilog;

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File(
        "Logs/library-api-.log",
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 14
    )
    .CreateLogger();

try
{
    Log.Information("Starting Library API");

    var builder = WebApplication.CreateBuilder(args);

    // Connect ASP.NET Core logging to Serilog
    builder.Host.UseSerilog();

    // Connect to SQL Server
    builder.Services.AddDbContext<LibraryDbContext>(options =>
        options.UseSqlServer(
            builder.Configuration.GetConnectionString(
                "DefaultConnection"
            )
        )
    );

    // Register controllers
    builder.Services.AddControllers();

    // Register Swagger
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    var app = builder.Build();

    // Log incoming HTTP requests
    app.UseSerilogRequestLogging();

    // Handle unexpected exceptions globally
    app.UseMiddleware<GlobalExceptionMiddleware>();

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();

    app.UseAuthorization();

    app.MapControllers();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Library API terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}