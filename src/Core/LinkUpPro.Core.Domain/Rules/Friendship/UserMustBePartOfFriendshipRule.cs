namespace LinkUpPro.Domain.Rules.Friendship;
public class UserMustBePartOfFriendshipRule(Guid userId, Guid user1Id, Guid user2Id) : Common.IBusinessRule
{
    public string Message => "El usuario no forma parte de esta amistad.";
    public bool IsBroken() => userId != user1Id && userId != user2Id;
}