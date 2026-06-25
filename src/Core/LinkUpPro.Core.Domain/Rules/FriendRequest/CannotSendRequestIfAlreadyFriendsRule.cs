namespace LinkUpPro.Domain.Rules.FriendRequest;
public class CannotSendRequestIfAlreadyFriendsRule(bool alreadyFriends) : Common.IBusinessRule
{
    public string Message => "No puedes enviar una solicitud a alguien que ya es tu amigo.";
    public bool IsBroken() => alreadyFriends;
}