namespace Battleship.Core;

public class Vessel
{
    public int Id { get; set; }

    public VesselType Type { get; set; }

    public int HealthBar { get; set; }

    public bool IsVertical { get; set; }

    public int MaxHealth { get; set; }

    public bool IsDamaged => MaxHealth > HealthBar;

    public bool IsDestroyed => HealthBar == 0;

    public Coordinate[]? Coordinates { get; set; }
}
