using FluentValidation;
using LinkUpPro.Application.ViewModels.Friendship;

namespace LinkUpPro.Application.Validators.FriendRequest;

public class SendFriendRequestViewModelValidator : AbstractValidator<SendFriendRequestViewModel>
{
    public SendFriendRequestViewModelValidator()
    {
        RuleFor(x => x.ReceiverId)
            .NotEmpty().WithMessage("El ID del usuario destinatario es requerido.");
    }
}
