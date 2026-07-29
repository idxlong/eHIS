using eHIS.Domain.SeedWork;

namespace eHIS.Domain.Aggregates.EncounterAggregate;

public interface IEncounterRepository : IRepository<Encounter>
{
    Encounter Add(Encounter encounter);
    void Update(Encounter encounter);
    Task<Encounter?> GetByIdAsync(string id);
}
