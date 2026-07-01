using System;
using System.Threading;
using System.Threading.Tasks;
using LinkUpPro.Domain.Entities.User;

namespace LinkUpPro.Domain.Interfaces.Repositories.User;

public interface IUserRepository : IGenericRepository<Entities.User.User, Guid>
{
    Task<Entities.User.User?> GetByAppUserIdAsync(string appUserId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Entities.User.User>> SearchActiveUsersAsync(string query, Guid excludeUserId, CancellationToken cancellationToken = default);
}