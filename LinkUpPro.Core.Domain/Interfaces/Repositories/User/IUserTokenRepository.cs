using System;
using LinkUpPro.Domain.Entities.User;

namespace LinkUpPro.Domain.Interfaces.Repositories.User;

public interface IUserTokenRepository : IGenericRepository<UserToken, Guid>
{
}