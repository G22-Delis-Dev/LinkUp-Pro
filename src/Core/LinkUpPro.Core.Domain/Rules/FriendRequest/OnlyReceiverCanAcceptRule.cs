namespace LinkUpPro.Domain.Rules.FriendRequest;
public class OnlyReceiverCanAcceptRule(Guid currentUserId, Guid receiverId) : Common.IBusinessRule
{
    public string Message => "Solo el destinatario puede aceptar la solicitud.";
    public bool IsBroken() => currentUserId != receiverId;
}