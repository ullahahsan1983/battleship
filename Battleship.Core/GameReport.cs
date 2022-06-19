namespace Battleship.Core;

public class GameReport
{
    public Guid TrackingId { get; set; }
    public Slot[]? Grid1 { get; set; }
    public Slot[]? Grid2 { get; set; }
    public int? Winner { get; set; }
    public string? LastAttacker { get; set; }
    public AttackState LastAttack { get; set; }
    public GameState? State { get; set; }
}