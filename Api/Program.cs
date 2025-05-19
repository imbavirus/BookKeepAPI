using Microsoft.EntityFrameworkCore;
using BookKeepAPI.Application.Data;
using FluentValidation;
using FluentValidation.AspNetCore;
using BookKeepAPI.Api.Middleware.Implementation;
using BookKeepAPI.Application.Validators;
using BookKeepAPI.Application.Validators.BookData;
using BookKeepAPI.Application.Managers.BookData;
using BookKeepAPI.Application.Managers.BookData.Implementation;
using BookKeepAPI.Api.Services.BookData;
using BookKeepAPI.Api.Services.BookData.Implementation;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSwaggerGen(); 
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection") ?? "Data Source=bookkeep.db",
        b => b.MigrationsAssembly("BookKeepAPI.Application"))
);

// Add controllers
builder.Services.AddControllers();

builder.WebHost.UseUrls("http://0.0.0.0:5000");

// Register FluentValidation
builder.Services.AddFluentValidationAutoValidation(); // Enables automatic server-side validation
builder.Services.AddValidatorsFromAssemblyContaining<BaseModelValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<BookValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<BookDtoValidator>();

// Add Managers
builder.Services.AddScoped<IBookManager, BookManager>();

// Add Services
builder.Services.AddScoped<IBookService, BookService>();

// Add CORS services and define a policy
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "AllowAllDev",
                      policy =>
                      {
                          policy.AllowAnyOrigin()
                                .AllowAnyMethod()
                                .AllowAnyHeader();
                      });
});


var app = builder.Build();

// Apply migrations at startup (creates DB and schema if they don't exist and migrations are set up)
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var dbContext = services.GetRequiredService<AppDbContext>();
        dbContext.Database.Migrate(); // Applies pending migrations. Creates the database if it doesn't exist.
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while migrating or seeding the database.");
    }
}

app.UseExceptionHandler(options => options.UseMiddleware<ExceptionMiddleware>());

app.UseHttpsRedirection();

app.UseRouting();

// Apply the CORS policy. This should come after UseRouting and before
app.UseCors("AllowAllDev");

app.MapControllers();

// Configure swagger
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "BookKeep API V1");
});

app.Run();
