namespace LinkUpPro.Application.Interfaces.Post;

public interface IPostPrivacyService
{
    Task<bool> CanViewPostAsync(Guid postId, Guid requestingUserId);
}
