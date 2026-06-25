namespace LinkUpPro.Domain.Rules.FriendRequest;
public class OnlySenderCanHideFromHistoryRule(Guid currentUserId, Guid senderId) : Common.IBusinessRule
{
    public string Message => "No tienes permisos para ocultar este historial.";
    public bool IsBroken() => currentUserId != senderId;
}