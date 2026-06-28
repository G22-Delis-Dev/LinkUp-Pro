using LinkUpPro.Application.Common;
using LinkUpPro.Application.DTOs.Battleship;

namespace LinkUpPro.Application.Interfaces.Battleship;

public interface IBattleshipAttackService
{
    Task<ServiceResponse<AttackResultDto>> AttackAsync(AttackDto dto);
}
