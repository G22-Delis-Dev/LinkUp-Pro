namespace LinkUpPro.Domain.Rules.FriendRequest;
public class NoPendingRequestInAnyDirectionRule(bool hasPendingRequest) : Common.IBusinessRule
{
    public string Message => "Ya existe una solicitud de amistad pendiente entre estos usuarios.";
    public bool IsBroken() => hasPendingRequest;
}