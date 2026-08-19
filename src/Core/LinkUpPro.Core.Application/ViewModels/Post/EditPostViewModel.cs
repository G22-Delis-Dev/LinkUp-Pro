using LinkUpPro.Domain.Enums.Post;

namespace LinkUpPro.Application.ViewModels.Post;

public class EditPostViewModel
{
    public Guid Id { get; set; }
    public string Content { get; set; } = null!;
    public PostPrivacy Privacy { get; set; }
    public bool AllowComments { get; set; }
}
