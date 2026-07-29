using Microsoft.EntityFrameworkCore;
using eHIS.Domain.Aggregates.ObservationAggregate;
using eHIS.Domain.SeedWork;
using eHIS.Infrastructure.Persistence;

namespace eHIS.Infrastructure.Repositories;

public class ObservationRepository : IObservationRepository
{
    private readonly ApplicationDbContext _context;

    public IUnitOfWork UnitOfWork => _context;

    public ObservationRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public Observation Add(Observation observation)
    {
        return _context.Observations.Add(observation).Entity;
    }

    public void Update(Observation observation)
    {
        _context.Entry(observation).State = EntityState.Modified;
    }

    public async Task<Observation?> GetByIdAsync(string id)
    {
        return await _context.Observations
            .FirstOrDefaultAsync(o => o.Id == id);
    }

    public async Task<List<Observation>> GetByPatientIdAsync(string patientId)
    {
        return await _context.Observations
            .Where(o => o.PatientId == patientId)
            .OrderByDescending(o => o.EffectiveDateTime)
            .ToListAsync();
    }
}
