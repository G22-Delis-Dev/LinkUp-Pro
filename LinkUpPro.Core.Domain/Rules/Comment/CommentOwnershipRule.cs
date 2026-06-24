namespace LinkUpPro.Domain.Rules.Comment;

public class CommentOwnershipRule(Guid commentOwnerId, Guid currentUserId) : Common.IBusinessRule
{
    public string Message => "No tienes permisos para modificar este comentario.";
    public bool IsBroken() => commentOwnerId != currentUserId;
}