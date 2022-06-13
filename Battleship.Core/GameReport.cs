namespace Battleship.Core;

public class GameReport
{
    public Guid TrackingId { get; set; }
    public Slot[]? Grid1 { get; set; }
    public Slot[]? Grid2 { get; set; }
    public string? Winner { get; set; }
    public string? LastAttacker { get; set; }
    public AttackState LastAttack { get; set; }
    public GameState? State { get; set; }
}

public class AttackReport
{
    public Slot Target { get; set; }

    public AttackState Result { get; set; }

    public Slot CounterTarget { get; set; }

    public AttackState CounterResult { get; set; }

    public GameState? GameState { get; set; }
}
