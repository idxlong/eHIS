using Microsoft.EntityFrameworkCore;
using eHIS.Domain.Aggregates.EncounterAggregate;
using eHIS.Domain.SeedWork;
using eHIS.Infrastructure.Persistence;

namespace eHIS.Infrastructure.Repositories;

public class EncounterRepository : IEncounterRepository
{
    private readonly ApplicationDbContext _context;

    public IUnitOfWork UnitOfWork => _context;

    public EncounterRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public Encounter Add(Encounter encounter)
    {
        return _context.Encounters.Add(encounter).Entity;
    }

    public void Update(Encounter encounter)
    {
        _context.Entry(encounter).State = EntityState.Modified;
    }

    public async Task<Encounter?> GetByIdAsync(string id)
    {
        return await _context.Encounters
            .FirstOrDefaultAsync(e => e.Id == id);
    }
}
