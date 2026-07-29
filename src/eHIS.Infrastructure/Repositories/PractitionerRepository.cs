using Microsoft.EntityFrameworkCore;
using eHIS.Domain.Aggregates.PractitionerAggregate;
using eHIS.Domain.SeedWork;
using eHIS.Infrastructure.Persistence;

namespace eHIS.Infrastructure.Repositories;

public class PractitionerRepository : IPractitionerRepository
{
    private readonly ApplicationDbContext _context;

    public IUnitOfWork UnitOfWork => _context;

    public PractitionerRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public Practitioner Add(Practitioner practitioner)
    {
        return _context.Practitioners.Add(practitioner).Entity;
    }

    public void Update(Practitioner practitioner)
    {
        _context.Entry(practitioner).State = EntityState.Modified;
    }

    public async Task<Practitioner?> GetByIdAsync(string id)
    {
        return await _context.Practitioners
            .FirstOrDefaultAsync(p => p.Id == id);
    }
}
