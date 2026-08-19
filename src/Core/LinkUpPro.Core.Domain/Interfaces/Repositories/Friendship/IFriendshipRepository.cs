using System;
using LinkUpPro.Domain.Entities.Friendship;

namespace LinkUpPro.Domain.Interfaces.Repositories.Friendship;

public interface IFriendshipRepository : IGenericRepository<Entities.Friendship.Friendship, Guid>
{
    Task<bool> AreActiveFriendsAsync(Guid userA, Guid userB);
}