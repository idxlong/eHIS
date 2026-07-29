using eHIS.Domain.SeedWork;

namespace eHIS.Domain.Aggregates.PractitionerAggregate;

public interface IPractitionerRepository : IRepository<Practitioner>
{
    Practitioner Add(Practitioner practitioner);
    void Update(Practitioner practitioner);
    Task<Practitioner?> GetByIdAsync(string id);
}
