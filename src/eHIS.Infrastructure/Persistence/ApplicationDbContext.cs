using MediatR;
using Microsoft.EntityFrameworkCore;
using eHIS.Domain.Aggregates.EncounterAggregate;
using eHIS.Domain.Aggregates.ObservationAggregate;
using eHIS.Domain.Aggregates.PatientAggregate;
using eHIS.Domain.Aggregates.PractitionerAggregate;
using eHIS.Domain.SeedWork;
using eHIS.Domain.ValueObjects;
using eHIS.Infrastructure.Extensions;

namespace eHIS.Infrastructure.Persistence;

public class ApplicationDbContext : DbContext, IUnitOfWork
{
    private readonly IMediator _mediator;

    public DbSet<Patient> Patients => Set<Patient>();
    public DbSet<Practitioner> Practitioners => Set<Practitioner>();
    public DbSet<Encounter> Encounters => Set<Encounter>();
    public DbSet<Observation> Observations => Set<Observation>();

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IMediator mediator)
        : base(options)
    {
        _mediator = mediator;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // --- Patient Configuration ---
        modelBuilder.Entity<Patient>(builder =>
        {
            builder.HasKey(p => p.Id);
            builder.Property(p => p.Id).ValueGeneratedNever();

            builder.Property(p => p.Gender).HasMaxLength(50);
            builder.Property(p => p.BirthDate);
            builder.Property(p => p.Active);
            builder.Property(p => p.DeceasedBoolean);
            builder.Property(p => p.DeceasedDateTime);

            // Storing lists of complex structures (value objects) as JSON columns (supported in EF Core)
            builder.OwnsMany(p => p.Names, namesBuilder =>
            {
                namesBuilder.ToJson();
                namesBuilder.OwnsOne(n => n.Period);
            });

            builder.OwnsMany(p => p.Telecoms, telecomBuilder =>
            {
                telecomBuilder.ToJson();
                telecomBuilder.OwnsOne(t => t.Period);
            });

            builder.OwnsMany(p => p.Addresses, addressBuilder =>
            {
                addressBuilder.ToJson();
                addressBuilder.OwnsOne(a => a.Period);
            });
        });

        // --- Practitioner Configuration ---
        modelBuilder.Entity<Practitioner>(builder =>
        {
            builder.HasKey(p => p.Id);
            builder.Property(p => p.Id).ValueGeneratedNever();

            builder.Property(p => p.Gender).HasMaxLength(50);
            builder.Property(p => p.BirthDate);
            builder.Property(p => p.Active);

            builder.OwnsMany(p => p.Names, namesBuilder =>
            {
                namesBuilder.ToJson();
                namesBuilder.OwnsOne(n => n.Period);
            });

            builder.OwnsMany(p => p.Telecoms, telecomBuilder =>
            {
                telecomBuilder.ToJson();
                telecomBuilder.OwnsOne(t => t.Period);
            });
        });

        // --- Encounter Configuration ---
        modelBuilder.Entity<Encounter>(builder =>
        {
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).ValueGeneratedNever();

            builder.Property(e => e.Status).HasMaxLength(50);
            builder.Property(e => e.PatientId).IsRequired();
            builder.Property(e => e.PractitionerId).IsRequired();

            // Value Objects mapped as owned/inline columns
            builder.OwnsOne(e => e.Class);
            builder.OwnsOne(e => e.Period);

            builder.OwnsMany(e => e.Diagnoses, diag =>
            {
                diag.ToJson();
                diag.OwnsOne(d => d.Condition, cond =>
                {
                    cond.OwnsMany(c => c.Coding);
                });
                diag.OwnsOne(d => d.Use);
            });
        });

        // --- Observation Configuration ---
        modelBuilder.Entity<Observation>(builder =>
        {
            builder.HasKey(o => o.Id);
            builder.Property(o => o.Id).ValueGeneratedNever();

            builder.Property(o => o.Status).HasMaxLength(50);
            builder.Property(o => o.PatientId).IsRequired();
            builder.Property(o => o.PerformerId).IsRequired();
            builder.Property(o => o.EffectiveDateTime);
            builder.Property(o => o.EncounterId);

            builder.OwnsOne(o => o.Code, code =>
            {
                code.ToJson();
                code.OwnsMany(c => c.Coding);
            });

            builder.OwnsOne(o => o.Value, val =>
            {
                val.ToJson();
            });

            builder.OwnsMany(o => o.Category, cat =>
            {
                cat.ToJson();
                cat.OwnsMany(c => c.Coding);
            });
        });
    }

    public async Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default)
    {
        // Dispatch Domain Events collection BEFORE saving changes
        await _mediator.DispatchDomainEventsAsync(this);

        // Commit transaction changes
        await base.SaveChangesAsync(cancellationToken);

        return true;
    }
}
