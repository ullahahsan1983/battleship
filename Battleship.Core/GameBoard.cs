namespace Battleship.Core;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
public class GameBoard
{
    public Guid Id { get; set; }
    public Region Size { get; set; }
    public Fleet Player1 { get; set; }
    public Fleet Player2 { get; set; }

    public Slot[] Grid1 { get; set; }
    public Slot[] Grid2 { get; set; }

    public string? Winner { get; set; }
    public string? LastAttacker { get; set; }
    public AttackState LastAttack { get; set; }
    public int Explored { get; set; }
    public GameState? State { get; set; }
}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.