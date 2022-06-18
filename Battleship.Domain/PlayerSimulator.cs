using Battleship.Core;
using Battleship.Core.Abstractions;

namespace Battleship.Domain;

public class PlayerSimulator : IPlayerSimulator
{
    public Coordinate SelectAttackSpot(Region region, Slot[] knownSpots)
    {
        var damagedCoords = knownSpots.Where(e => e.State == SlotState.Damaged)
            .Select(e => e.Coordinate)
            .ToArray();

        var knownCoords = knownSpots.Select(e => e.Coordinate).ToArray();

        if (damagedCoords.Any())
        {
            var adjacentCoords = damagedCoords
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

            // Remove already revealed ones
            adjacentCoords.RemoveAll(c => knownCoords.Contains(c));

            // Remove out-of-boundary ones
            adjacentCoords.RemoveAll(c => c.RowIndex < 0 || c.RowIndex >= region.Rows || c.ColIndex < 0 || c.ColIndex >= region.Cols);

            if (adjacentCoords.Any()) 
                return adjacentCoords.FirstOrDefault();
        }

        var rowIndices = Enumerable.Range(0, region.Rows).ToArray();
        var colIndices = Enumerable.Range(0, region.Cols).ToArray();

        var rowsUntouched = rowIndices
            .Except(knownSpots.Select(e => e.Coordinate.RowIndex).Distinct())
            .ToArray();
        var colsUntouched = colIndices
            .Except(knownSpots.Select(e => e.Coordinate.ColIndex).Distinct())
            .ToArray();

        Coordinate Pick(int[] r, int[] c)
            => new(Randomizer.FromArray(r), Randomizer.FromArray(c));
        
        // If both are available, pick from the mix
        if (rowsUntouched.Any() && colsUntouched.Any())
        {
            return Pick(rowsUntouched, colsUntouched);
        }
        // or from any column of untouched rows
        else if (rowsUntouched.Any())
        {
            return Pick(rowsUntouched, colIndices);
        }
        // or from any row of untouched columns
        else if (colsUntouched.Any())
        {
            return Pick(rowIndices, colsUntouched);
        }

        // Best randomization pattern fails, so pick from specific avaiable slots
        var coords = new List<Coordinate>();
        for(int i = 0; i < rowIndices.Length; i++)
            for(int j = 0; j < colIndices.Length; j++)
                if (!knownCoords.Contains(new(i,j)))
                    coords.Add(new(i,j));

        if (!coords.Any())
            throw new InvalidAttackException("No empty slots available");

        return Randomizer.FromList(coords);
    }
}