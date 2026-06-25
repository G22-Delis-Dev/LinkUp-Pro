namespace LinkUpPro.Domain.Rules.Friendship;
public class NoDuplicateActiveFriendshipRule(bool alreadyFriends) : Common.IBusinessRule
{
    public string Message => "Estos usuarios ya son amigos.";
    public bool IsBroken() => alreadyFriends;
}