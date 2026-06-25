using System;
using LinkUpPro.Domain.Entities.Friendship;

namespace LinkUpPro.Domain.Interfaces.Repositories.Friendship;

public interface IFriendRequestRepository : IGenericRepository<FriendRequest, Guid>
{
}