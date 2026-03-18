using Shared.Data;

namespace Shared.Outbox.Repository;

public interface IOutboxRepository : IRepository<Model.Outbox, long>
{
    
}
