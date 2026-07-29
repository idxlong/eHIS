using Microsoft.EntityFrameworkCore;
using eHIS.Domain.Aggregates.PatientAggregate;
using eHIS.Domain.SeedWork;
using eHIS.Infrastructure.Persistence;

namespace eHIS.Infrastructure.Repositories;

public class PatientRepository : IPatientRepository
{
    private readonly ApplicationDbContext _context;

    public IUnitOfWork UnitOfWork => _context;

    public PatientRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public Patient Add(Patient patient)
    {
        return _context.Patients.Add(patient).Entity;
    }

    public void Update(Patient patient)
    {
        _context.Entry(patient).State = EntityState.Modified;
    }

    public async Task<Patient?> GetByIdAsync(string id)
    {
        return await _context.Patients
            .FirstOrDefaultAsync(p => p.Id == id);
    }
}
