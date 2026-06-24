namespace LinkUpPro.Domain.Rules.Post;
public class PostVisibilityRule(Enums.Post.PostPrivacy privacy, bool isFriend, Guid postOwnerId, Guid currentUserId) : Common.IBusinessRule
{
    public string Message => "No tienes permisos para ver este post.";
    public bool IsBroken() => postOwnerId != currentUserId && privacy == Enums.Post.PostPrivacy.Private || (privacy == Enums.Post.PostPrivacy.FriendsOnly && !isFriend && postOwnerId != currentUserId);
}