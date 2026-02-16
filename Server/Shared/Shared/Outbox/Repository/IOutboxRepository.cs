using Shared.Data.Repository;

namespace Shared.Outbox.Repository;

public interface IOutboxRepository : IRepository<Model.Outbox, long>
{
    
}
