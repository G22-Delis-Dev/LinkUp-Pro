using AutoMapper;
using LinkUpPro.Application.DTOs.Notification;
using LinkUpPro.Domain.Entities.Notification;

namespace LinkUpPro.Application.Mappings;

public class NotificationProfile : Profile
{
    public NotificationProfile()
    {
        CreateMap<Notification, NotificationDto>();
    }
}
