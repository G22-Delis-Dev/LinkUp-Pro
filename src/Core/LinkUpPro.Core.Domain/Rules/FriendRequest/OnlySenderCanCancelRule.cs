namespace LinkUpPro.Domain.Rules.FriendRequest;
public class OnlySenderCanCancelRule(Guid currentUserId, Guid senderId) : Common.IBusinessRule
{
    public string Message => "Solo el remitente puede cancelar la solicitud.";
    public bool IsBroken() => currentUserId != senderId;
}