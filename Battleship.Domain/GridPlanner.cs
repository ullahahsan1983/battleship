using Battleship.Core;
using Battleship.Core.Abstractions;

namespace Battleship.Domain;

public class GridPlanner : IGridPlanner
{
    private readonly bool[,] _cells;
    private readonly Region _region;

    public GridPlanner(Region region)
    {
        _region = region;
        _cells = new bool[region.Rows, region.Cols];
    }

    public Coordinate[] BookHorizontally(int span)
    {
        return Book(span, _region.Rows, _region.Cols, false);
    }

    public Coordinate[] BookVertically(int span)
    {
        return Book(span, _region.Cols, _region.Rows, true);
    }

    Coordinate[] Book(int span, int scanBoundary, int spanBoundary, bool vertical)
    {
        var entrySpots = new List<Coordinate>();

        // Find a valid span-entry point, forward-only algorithm
        for (var i = 0; i < scanBoundary; i++)
            for (var j = 0; j <= spanBoundary - span; j++)
            {
                var (x, y) = vertical ? (j, i) : (i, j);
                if (!IsAvailable(new Coordinate(x, y), span, vertical))
                    break;
                entrySpots.Add(new Coordinate(x, y));
            }

        if (!entrySpots.Any()) return Array.Empty<Coordinate>();

        // Random pick
        var entrySpot = Randomizer.FromList(entrySpots);

        var coordinates = new Coordinate[span];
        for (var k = 0; k < span; k++)
        {
            var (x, y) = vertical ? (entrySpot.RowIndex + k, entrySpot.ColIndex) : (entrySpot.RowIndex, entrySpot.ColIndex + k);

            // Mark the cell as occupied
            _cells[x, y] = true;

            // Record projected coordinate
            coordinates[k] = new Coordinate(x, y);
        }

        return coordinates;
    }

    public bool IsAvailable(Coordinate coordinate, int span, bool vertical)
    {
        // Not a valid coordinate
        if (coordinate.RowIndex >= _region.Rows || coordinate.ColIndex >= _region.Cols) return false;

        bool visit(int baseIndex, int boundIndex, int rowIncrement, int colIncrement)
        {
            // If span exceeds the boundary, report negative
            if (baseIndex + span > boundIndex) return false;

            // If a blocking cell exists along the path, report negative
            for (var k = 0; k < span; k++)
                if (_cells[coordinate.RowIndex + k * rowIncrement, coordinate.ColIndex + k * colIncrement])
                    return false;

            return true;
        }

        return vertical ? visit(coordinate.RowIndex, _region.Rows, 1, 0) : visit(coordinate.ColIndex, _region.Cols, 0, 1);
    }
}
