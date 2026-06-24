namespace LinkUpPro.Domain.Rules.FriendRequest;
public class OnlyReceiverCanRejectRule(Guid currentUserId, Guid receiverId) : Common.IBusinessRule
{
    public string Message => "Solo el destinatario puede rechazar la solicitud.";
    public bool IsBroken() => currentUserId != receiverId;
}