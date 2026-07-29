using eHIS.Domain.SeedWork;

namespace eHIS.Domain.Aggregates.ObservationAggregate;

public interface IObservationRepository : IRepository<Observation>
{
    Observation Add(Observation observation);
    void Update(Observation observation);
    Task<Observation?> GetByIdAsync(string id);
    Task<List<Observation>> GetByPatientIdAsync(string patientId);
}
