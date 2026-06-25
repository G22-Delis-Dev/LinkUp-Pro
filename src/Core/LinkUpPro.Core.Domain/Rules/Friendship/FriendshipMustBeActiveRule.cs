namespace LinkUpPro.Domain.Rules.Friendship;

public class FriendshipMustBeActiveRule(Enums.Friendship.FriendshipStatus status) : Common.IBusinessRule
{
    public string Message => "La amistad debe estar activa.";
    public bool IsBroken() => status != Enums.Friendship.FriendshipStatus.Active;
}