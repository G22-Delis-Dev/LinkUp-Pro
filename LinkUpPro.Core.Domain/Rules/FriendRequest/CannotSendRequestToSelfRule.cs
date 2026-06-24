namespace LinkUpPro.Domain.Rules.FriendRequest;
public class CannotSendRequestToSelfRule(Guid senderId, Guid receiverId) : Common.IBusinessRule
{
    public string Message => "No puedes enviarte una solicitud de amistad a ti mismo.";
    public bool IsBroken() => senderId == receiverId;
}