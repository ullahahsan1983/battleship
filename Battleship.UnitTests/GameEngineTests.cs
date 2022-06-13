using Battleship.Core;
using Battleship.Domain;
using Xunit;
using Moq;
using Battleship.Core.Abstractions;
using System.Linq;

namespace Battleship.UnitTests;

public class GameEngineTests
{
    [Fact]
    public void WhenStartNew_ThenReturnsInitializedGameBoard()
    {
        var board = new GameBoard();

        var engine = CreateEngine(board);

        engine.StartNewGame();

        Assert.NotNull(board.Player1);
        Assert.NotNull(board.Player1.Vessels);
        Assert.Equal(3, board.Player1.Vessels!.Length);

        Assert.NotNull(board.Player2);
        Assert.NotNull(board.Player2.Vessels);
        Assert.Equal(3, board.Player2.Vessels!.Length);

        Assert.NotNull(board.Grid1);
        Assert.NotNull(board.Grid2);
        Assert.Equal(GameState.Initiated, board.State);
    }

    [Fact]
    public void WhenLaunchAttack_ThenVerifyAttackTarget()
    {
        var board = new GameBoard();

        var engine = CreateEngine(board);

        engine.StartNewGame();

        void AssertAttack(Coordinate coord, SlotState expectedState, AttackState expectedResult)
        {
            var report = engine.LaunchAttackByPlayer(coord);

            Assert.NotNull(report);
            Assert.Equal(GameState.Running, board.State);

            Assert.Equal(coord, report.Target.Coordinate);
            Assert.True(report.Target.IsRevealed);
            Assert.Equal(expectedState, report.Target.State);

            Assert.Equal(expectedResult, report.Result);
        }

        // Attack on empty slot
        AssertAttack(new Coordinate(2, 3), SlotState.Empty, AttackState.Missed);

        // Attack on occupied slot
        AssertAttack(new Coordinate(1, 1), SlotState.Damaged, AttackState.Hit);
        // Attack on occupied slot     
        AssertAttack(new Coordinate(1, 2), SlotState.Damaged, AttackState.Hit);
        // Attack on occupied slot     
        AssertAttack(new Coordinate(1, 3), SlotState.Damaged, AttackState.Hit);
        // Attack on final occupied slot     
        AssertAttack(new Coordinate(1, 4), SlotState.Destroyed, AttackState.Hit);

        // Attack on already revealed spot     
        Assert.ThrowsAny<InvalidAttackException>(() => engine.LaunchAttackByPlayer(new Coordinate(2, 3)));

        Assert.ThrowsAny<InvalidAttackException>(() => engine.LaunchAttackByPlayer(new Coordinate(1, 4)));
    }

    GameEngine CreateEngine(GameBoard board)
    {
        var mockBoardProvider = new Mock<IGameBoardProvider>();
        mockBoardProvider.Setup(x => x.GetCurrent())
            .Returns(board);
        mockBoardProvider.Setup(x => x.CreateNew())
            .Returns(board);

        Vessel CreateVessel(VesselType type, Coordinate[] coordinates)
        {
            return new Vessel
            {
                Type = type,
                HealthBar = (int)type,
                MaxHealth = (int)type,
                Coordinates = coordinates,
                IsVertical = coordinates.DistinctBy(e => e.ColIndex).Count() == 1,
            };
        }

        var playerFleet = new Fleet
        {
            Name = "Player",
            Region = new Region(5, 10),
            Vessels = new Vessel[] {
                CreateVessel(VesselType.Destroyer, new Coordinate[] { new(1,7), new(2,7), new(3,7), new(4,7) }),
                CreateVessel(VesselType.Battleship, new Coordinate[] { new(1,1), new(1,2), new(1,3), new(1,4), new(1,5) }),
                CreateVessel(VesselType.Destroyer, new Coordinate[] { new(4,2), new(4,3), new(4,4), new(4,5) }),
            }
        };

        var computerFleet = new Fleet
        {
            Name = "Computer",
            Region = new Region(5, 10),
            Vessels = new Vessel[] {
                CreateVessel(VesselType.Battleship, new Coordinate[] { new(0, 8), new(1,8), new(2,8), new(3,8), new(4,8) }),
                CreateVessel(VesselType.Destroyer, new Coordinate[] { new(1,1), new(1,2), new(1,3), new(1,4) }),
                CreateVessel(VesselType.Destroyer, new Coordinate[] { new(4,3), new(4,4), new(4,5), new(4,6) }),
            }
        };

        var mockFleetDesginer = new Mock<IFleetDesigner>();
        mockFleetDesginer.Setup(x => x.CreateFleet(playerFleet.Name, It.IsAny<Region>(), It.IsAny<VesselType[]>()))
            .Returns(playerFleet);
        mockFleetDesginer.Setup(x => x.CreateFleet(computerFleet.Name, It.IsAny<Region>(), It.IsAny<VesselType[]>()))
            .Returns(computerFleet);

        return new GameEngine(
            new GameEnvironment(), 
            mockBoardProvider.Object, 
            mockFleetDesginer.Object,
            new PlayerSimulator());
    }
}
