namespace Battleship.Core;

public class AttackReport
{
    public AttackResult? Result { get; set; }

    public AttackResult? CounterResult { get; set; }

    public GameState? GameState { get; set; }

    public int? Winner { get; set; }
}

public record class AttackResult(Slot Target, AttackState State, int? AffectedVessel = null);
