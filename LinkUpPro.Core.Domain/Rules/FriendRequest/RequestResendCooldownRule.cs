namespace LinkUpPro.Domain.Rules.FriendRequest;
public class RequestResendCooldownRule(DateTime? lastRequestDate, int cooldownHours = 24) : Common.IBusinessRule
{
    public string Message => "Debes esperar antes de enviar otra solicitud a este usuario.";
    public bool IsBroken() => lastRequestDate.HasValue && (DateTime.UtcNow - lastRequestDate.Value).TotalHours < cooldownHours;
}