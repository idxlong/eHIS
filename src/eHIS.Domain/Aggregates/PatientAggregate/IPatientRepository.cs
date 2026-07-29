using eHIS.Domain.SeedWork;

namespace eHIS.Domain.Aggregates.PatientAggregate;

public interface IPatientRepository : IRepository<Patient>
{
    Patient Add(Patient patient);
    void Update(Patient patient);
    Task<Patient?> GetByIdAsync(string id);
}
