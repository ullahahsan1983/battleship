using Battleship.Core;
using Battleship.Core.Abstractions;

namespace Battleship.Domain;

public class PlayerSimulator : IPlayerSimulator
{
    public Coordinate SelectAttackSpot(Region region, Slot[] hotSpots)
    {
        var damagedSpots = hotSpots.Where(e => e.State == SlotState.Damaged)
            .Select(e => e.Coordinate)
            .ToList();

        if (damagedSpots.Any())
        {
            var adjacentSpots = damagedSpots
                .SelectMany(e =>
                {
                    return new[]
                    {
                        new Coordinate(e.RowIndex - 1, e.ColIndex),
                        new Coordinate(e.RowIndex + 1, e.ColIndex),
                        new Coordinate(e.RowIndex, e.ColIndex - 1),
                        new Coordinate(e.RowIndex, e.ColIndex + 1),
                    };
                })
                .ToList();

            // Remove already damaged ones
            adjacentSpots.RemoveAll(c => damagedSpots.Contains(c));

            // Remove out-of-boundary
            adjacentSpots.RemoveAll(c => c.RowIndex < 0 || c.RowIndex >= region.Rows || c.ColIndex < 0 || c.ColIndex >= region.Cols);

            if (adjacentSpots.Any()) 
                return adjacentSpots.FirstOrDefault();
        }

        var r = Random.Shared.Next(0, region.Rows - 1);
        var c = Random.Shared.Next(0, region.Cols - 1);

        return new Coordinate(r, c);
    }
}