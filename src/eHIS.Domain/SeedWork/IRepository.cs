namespace eHIS.Domain.SeedWork;

public interface IRepository<T> where T : AggregateRoot<string>
{
    IUnitOfWork UnitOfWork { get; }
}
