using Battleship.Core;
using Battleship.Domain;
using System;
using System.Linq;
using Xunit;

namespace Battleship.UnitTests;

public class PlayerSimulatorTests
{
    [Fact]
    public void WhenSeekingFirstTarget_ThenReturnsRandomCoordinateWithinBoundary()
    {
        Repeat(() =>
        {
            var simulator = new PlayerSimulator();

            var coord1 = simulator.SelectAttackSpot(new Region(5, 10), Array.Empty<Slot>());

            Assert.InRange(coord1.RowIndex, 0, 4);
            Assert.InRange(coord1.ColIndex, 0, 9);

            var coord2 = simulator.SelectAttackSpot(new Region(5, 10), Array.Empty<Slot>());

            Assert.InRange(coord2.RowIndex, 0, 4);
            Assert.InRange(coord2.ColIndex, 0, 9);

            // Should be random on each attempt
            Assert.True(coord2.RowIndex != coord1.RowIndex || coord2.ColIndex != coord1.ColIndex, "Expected random selection");
        });
    }

    // Repeat the test to rule out possibility of random correctness
    void Repeat(Action testAction, int count = 10)
    {
        for(int i = 0; i < count; i++)
            testAction();
    }

    [Fact]
    public void WhenSeekingSubsequentTarget_ThenReturnsRandomCoordinateExcludingPreviousOnes()
    {
        Repeat(() =>
        {
            var simulator = new PlayerSimulator();

            var coord1 = simulator.SelectAttackSpot(new Region(1, 3), new Slot[]
            {
                new(1, new(0, 0), true, SlotState.Empty),
                new(2, new(0, 1), true, SlotState.Empty),
            });

            // Should be from unknown coords
            Assert.Equal(new(0, 2), coord1);

            var coord2 = simulator.SelectAttackSpot(new Region(2, 2), new Slot[]
            {
                new(1, new(0, 0), true, SlotState.Empty),
                new(4, new(1, 1), true, SlotState.Empty),
            });

            // Should be from unknown coords
            Assert.True(coord2 == new Coordinate(0, 1) || coord2 == new Coordinate(1, 0));
        });
    }

    [Fact]
    public void WhenSeekingSubsequentTargetWithDamagedSpots_ThenReturnsAdjacentCoordinate()
    {
        Repeat(() =>
        {
            var simulator = new PlayerSimulator();

            var coord1 = simulator.SelectAttackSpot(new Region(1, 4), new Slot[]
            {
                new(2, new(0, 1), true, SlotState.Empty),
                new(3, new(0, 2), true, SlotState.Damaged),
            });

            // Should bew adjacent to damaged spot
            Assert.Equal(new(0, 3), coord1);

            var coord2 = simulator.SelectAttackSpot(new Region(3, 3), new Slot[]
            {
                new(2, new(0, 1), true, SlotState.Empty),
                new(5, new(1, 1), true, SlotState.Damaged),
            });

            // Should be from unrevealed adjacent coords
            Assert.True(coord2 == new Coordinate(1, 0)
                || coord2 == new Coordinate(1, 2)
                || coord2 == new Coordinate(2, 1));
        });
    }
}