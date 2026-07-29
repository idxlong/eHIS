using FluentValidation;
using Microsoft.EntityFrameworkCore;
using eHIS.API.Endpoints;
using eHIS.Application.Behaviors;
using eHIS.Application.Patients;
using eHIS.Domain.Aggregates.EncounterAggregate;
using eHIS.Domain.Aggregates.ObservationAggregate;
using eHIS.Domain.Aggregates.PatientAggregate;
using eHIS.Domain.Aggregates.PractitionerAggregate;
using eHIS.Infrastructure.Persistence;
using eHIS.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add EF Core DB context using SQLite
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection") ?? "Data Source=eHIS.db"));

// Register Repositories
builder.Services.AddScoped<IPatientRepository, PatientRepository>();
builder.Services.AddScoped<IPractitionerRepository, PractitionerRepository>();
builder.Services.AddScoped<IEncounterRepository, EncounterRepository>();
builder.Services.AddScoped<IObservationRepository, ObservationRepository>();

// Register MediatR
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(CreatePatientCommand).Assembly);
    
    // Register Pipeline Behaviors (Logging and Validation)
    cfg.AddOpenBehavior(typeof(LoggingBehavior<,>));
    cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
});

// Register FluentValidation
builder.Services.AddValidatorsFromAssembly(typeof(CreatePatientCommand).Assembly);

builder.Services.AddOpenApi();

var app = builder.Build();

// Auto-migrate / auto-create database for ease of testing
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    // Ensure database is created based on our entity configurations
    context.Database.EnsureCreated();
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

// Map Minimal API Endpoints
app.MapPatientEndpoints();
app.MapPractitionerEndpoints();
app.MapEncounterEndpoints();
app.MapObservationEndpoints();

app.Run();
