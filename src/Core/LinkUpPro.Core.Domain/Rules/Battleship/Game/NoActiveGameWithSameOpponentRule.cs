namespace LinkUpPro.Domain.Rules.Battleship.Game;

public class NoActiveGameWithSameOpponentRule(bool hasActiveGame) : Common.IBusinessRule
{
    public string Message => "Ya tienes un juego activo con este oponente.";
    public bool IsBroken() => hasActiveGame;
}