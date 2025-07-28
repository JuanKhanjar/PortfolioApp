using Microsoft.EntityFrameworkCore;
using PortfolioApp.Application.UseCases;
using PortfolioApp.Domain.Interfaces;
using PortfolioApp.Infrastructure.Data;
using PortfolioApp.Infrastructure.Repositories;
using PortfolioApp.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

// Configure Entity Framework with SQLite
builder.Services.AddDbContext<PortfolioDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IProjectRepository, ProjectRepository>();
builder.Services.AddScoped<IContactMessageRepository, ContactMessageRepository>();

// Register use cases
builder.Services.AddScoped<UserUseCases>();
builder.Services.AddScoped<ProjectUseCases>();
builder.Services.AddScoped<ContactMessageUseCases>();

// Register infrastructure services
builder.Services.AddScoped<IFileUploadService, SimpleFileUploadService>();

// Configure email settings
var emailSettings = new EmailSettings();
builder.Configuration.GetSection("EmailSettings").Bind(emailSettings);
builder.Services.AddSingleton(emailSettings);
builder.Services.AddScoped<IEmailService, EmailService>();

// Add support for file uploads
builder.Services.Configure<IISServerOptions>(options =>
{
    options.MaxRequestBodySize = 2147483648; // 2GB
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

// Ensure database is created
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<PortfolioDbContext>();
    context.Database.EnsureCreated();
}

app.Run();

