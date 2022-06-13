using System.Linq;

namespace Battleship.Core;

public class Fleet
{
    public string Name { get; set; }    

    public Vessel[]? Vessels { get; set; }

    public Region Region { get; set; }

    public bool IsDefeated => Vessels?.All(e => e.IsDestroyed) ?? false;

    public Vessel? FindVessel(Coordinate? coordinate)
    {
        if (coordinate is null) return null;

        return Vessels?.FirstOrDefault(v => v.Coordinates != null && v.Coordinates.Contains(coordinate.Value));
    }
}
