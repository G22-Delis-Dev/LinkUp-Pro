using System;
using LinkUpPro.Domain.Entities.Reaction;

namespace LinkUpPro.Domain.Interfaces.Repositories.Reaction;

public interface IReactionRepository : IGenericRepository<Entities.Reaction.Reaction, Guid>
{
}