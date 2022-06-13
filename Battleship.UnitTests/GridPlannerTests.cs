using Battleship.Core;
using Battleship.Domain;
using Xunit;

namespace Battleship.UnitTests;

public class GridPlannerTests
{
    [Fact]
    public void WhenBookHorizontally_ThenReturnsHorizontalContiguousSpan()
    {
        var planner = new GridPlanner(new Region(5, 10));
        var spots = planner.BookHorizontally(4);

        Assert.Equal(4, spots.Length);

        var entrySpot = spots[0];
        Assert.Equal(entrySpot.RowIndex, spots[1].RowIndex);
        Assert.Equal(entrySpot.ColIndex + 1, spots[1].ColIndex);

        Assert.Equal(entrySpot.RowIndex, spots[2].RowIndex);
        Assert.Equal(entrySpot.ColIndex + 2, spots[2].ColIndex);

        Assert.Equal(entrySpot.RowIndex, spots[3].RowIndex);
        Assert.Equal(entrySpot.ColIndex + 3, spots[3].ColIndex);
    }

    [Fact]
    public void WhenBookVertically_ThenReturnsVerticalContiguousSpan()
    {
        var planner = new GridPlanner(new Region(5, 10));
        var spots = planner.BookVertically(5);

        Assert.Equal(5, spots.Length);

        var entrySpot = spots[0];
        Assert.Equal(entrySpot.ColIndex, spots[1].ColIndex);
        Assert.Equal(entrySpot.RowIndex + 1, spots[1].RowIndex);

        Assert.Equal(entrySpot.ColIndex, spots[2].ColIndex);
        Assert.Equal(entrySpot.RowIndex + 2, spots[2].RowIndex);

        Assert.Equal(entrySpot.ColIndex, spots[3].ColIndex);
        Assert.Equal(entrySpot.RowIndex + 3, spots[3].RowIndex);

        Assert.Equal(entrySpot.ColIndex, spots[4].ColIndex);
        Assert.Equal(entrySpot.RowIndex + 4, spots[4].RowIndex);
    }

    [Fact]
    public void WhenBookMultipleTimes_ThenCheckForReliablity()
    {
        var planner = new GridPlanner(new Region(5, 10));

        var placement1 = planner.BookVertically(5);
        Assert.Equal(5, placement1.Length);

        var placement2 = planner.BookHorizontally(4);
        Assert.Equal(4, placement2.Length);

        var placement3 = planner.BookHorizontally(4);
        Assert.Equal(4, placement3.Length);
    }
}
