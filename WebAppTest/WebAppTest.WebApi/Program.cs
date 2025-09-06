using FluentMigrator.Runner;
using WebAppTest.Application.Services.EmployeeService;
using WebAppTest.Core.Interfaces;
using WebAppTest.Core.Interfaces.Services;
using WebAppTest.Infrastructure.Data.Migrations;
using WebAppTest.Infrastructure.Data.UnitOfWork;
using WebAppTest.WebApi.Data.Migrations;
using WebAppTest.WebApi.Middlewares;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddScoped<IEmployeeService, EmployeeService>();
builder.Services.AddScoped<IUnitOfWork>(provider =>
{

    var connectionString = builder.Configuration.GetConnectionString("PostgreSQL");
    return string.IsNullOrEmpty(connectionString)
        ? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.")
        : new UnitOfWork(connectionString);
});

builder.Services.AddFluentMigratorCore()
    .ConfigureRunner(rb => rb
        .AddPostgres()
        .WithGlobalConnectionString(builder.Configuration.GetConnectionString("PostgreSQL"))
        .ScanIn(typeof(_1_InitialTablesMigration).Assembly).For.Migrations())
    .AddLogging(lb => lb.AddFluentMigratorConsole());

builder.Services.AddScoped<DatabaseMigrator>();

builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<ExceptionMiddleware>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

using (var scope = app.Services.CreateScope())
{
    var migrator = scope.ServiceProvider.GetRequiredService<DatabaseMigrator>();
    migrator.Migrate();
}

app.UseHttpsRedirection();

app.MapControllers();
app.UseMiddleware<ExceptionMiddleware>();

app.Run();