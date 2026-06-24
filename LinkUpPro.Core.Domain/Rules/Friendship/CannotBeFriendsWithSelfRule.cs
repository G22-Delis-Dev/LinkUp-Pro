namespace LinkUpPro.Domain.Rules.Friendship;
public class CannotBeFriendsWithSelfRule(Guid userId, Guid friendId) : Common.IBusinessRule
{
    public string Message => "No puedes ser amigo de ti mismo.";
    public bool IsBroken() => userId == friendId;
}