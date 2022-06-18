using Battleship.Core;
using Battleship.Core.Abstractions;

namespace Battleship.Domain;

public class GameEngine : IGameEngine
{
    private readonly IFleetDesigner _fleetDesigner;
    private readonly IGameEnvironment _gameEnvironment;
    private readonly IGameBoardProvider _gameBoardProvider;
    private readonly IPlayerSimulator _playerSimulator;

    public GameEngine(
        IGameEnvironment gameEnvironment,
        IGameBoardProvider gameBoardProvider,
        IFleetDesigner fleetDesigner,
        IPlayerSimulator playerSimulator)
    {
        _gameEnvironment = gameEnvironment;
        _gameBoardProvider = gameBoardProvider;
        _fleetDesigner = fleetDesigner;
        _playerSimulator = playerSimulator;
    }

    public bool IsRunning => Guard().State == GameState.Running;

    /// <summary>
    /// Prepares the board for a new game
    /// </summary>
    public void StartNewGame()
    {
        var board = _gameBoardProvider.CreateNew();
        board.State = GameState.Initiated;

        var boardSize = new Region(_gameEnvironment.BattleGridSize, _gameEnvironment.BattleGridSize);
        board.Size = boardSize;

        var vesselTypes = Enumerable.Repeat(VesselType.Destroyer, _gameEnvironment.NoOfDestroyers)
            .Concat(Enumerable.Repeat(VesselType.Battleship, _gameEnvironment.NoOfBattleships))
            .ToArray();

        var fleetRegion = new Region(boardSize.Rows / 2, boardSize.Cols);
        board.Player1 = _fleetDesigner.CreateFleet("Player", fleetRegion, vesselTypes);

        board.Player2 = _fleetDesigner.CreateFleet("Computer", fleetRegion, vesselTypes);

        board.Grid1 = PrepareBattleGrid(board.Player1);
        board.Grid2 = PrepareBattleGrid(board.Player2);
    }

    public AttackReport LaunchAttackByPlayer(Coordinate coordinate)
    {
        var board = Guard();

        var report = new AttackReport();

        (report.Target, report.Result) = LaunchAttack(coordinate, board.Player1, board.Player2);        

        // Auto launch computer's turn if Game is yet alive
        if (board.State == GameState.Running)
        {
            var counterReport = LaunchAttackByComputer();
            (report.CounterTarget, report.CounterResult) = (counterReport.CounterTarget, counterReport.CounterResult);
        }

        report.GameState = board.State;
        report.Winner = board.Winner;

        return report;
    }

    public AttackReport LaunchAttackByComputer()
    {
        var board = Guard();

        var coordinate = _playerSimulator.SelectAttackSpot(
            board.Player1.Region,
            board.Grid1.Where(e => e.IsRevealed).ToArray());

        var (counterAttack, counterResult) = LaunchAttack(coordinate, board.Player2, board.Player1);

        return new AttackReport { CounterTarget = counterAttack, CounterResult = counterResult, GameState = board.State };
    }

    (Slot Target, AttackState Result) LaunchAttack(Coordinate coordinate, Fleet attacker, Fleet opponent)
    {
        var board = Guard();

        var targetGrid = attacker.Name == board.Player1.Name ? board.Grid2 : board.Grid1;

        if (coordinate.RowIndex >= opponent.Region.Rows || coordinate.ColIndex >= opponent.Region.Cols)
            throw new InvalidAttackException("Invalid coordinate");

        var slotIndex = coordinate.RowIndex * board.Size.Rows + coordinate.ColIndex;
        var targetSlot = targetGrid[slotIndex];

        if (targetSlot.IsRevealed)
            throw new InvalidAttackException("Invalid attack place, already explored");

        // Attack occurred
        targetSlot.IsRevealed = true;
        board.LastAttacker = attacker.Name;
        board.Explored++;
        board.State = GameState.Running;

        var vessel = opponent.FindVessel(coordinate);

        // Bulls-eye
        if (vessel != null)
        {
            board.LastAttack = AttackState.Hit;
            vessel.HealthBar--;

            targetSlot.State = vessel.IsDestroyed ? SlotState.Destroyed : SlotState.Damaged;

            if (opponent.IsDefeated)
            {
                board.Winner = attacker.Name;
                board.State = GameState.Finished;
            }
        }
        // Missed
        else
        {
            targetSlot.State = SlotState.Empty;
            board.LastAttack = AttackState.Missed;
        }

        targetGrid[slotIndex] = targetSlot;

        return (targetSlot, board.LastAttack);
    }

    public GameReport GetReport()
    {
        var board = Guard();

        var report = new GameReport
        {
            TrackingId = board.Id,
            LastAttack = board.LastAttack,
            LastAttacker = board.LastAttacker,
            Winner = board.Winner,
            State = board.State,
            Grid1 = board.Grid1,
            Grid2 = Mask(board.Grid2),
        };

        return report;
    }

    GameBoard Guard()
    {
        var board = _gameBoardProvider.GetCurrent();

        if (board == null)
            throw new InvalidOperationException("Game is not yet initialized");

        if (board.Grid1 == null || board.Grid2 == null)
            throw new InvalidOperationException("BattleGrid is not yet ready");

        return board;
    }

    Slot[] PrepareBattleGrid(Fleet fleet)
    {
        var (rowSize, colSize) = (_gameEnvironment.BattleGridSize / 2, _gameEnvironment.BattleGridSize);

        var slots = new Slot[rowSize * colSize];

        var occupied = fleet.Vessels!
            .SelectMany(e => e.Coordinates!)
            .ToArray()!;

        var sequence = 0;
        for (int i = 0; i < rowSize; i++)
            for (int j = 0; j < colSize; j++)
            {
                sequence++;
                slots[i * colSize + j] = new Slot(sequence, new Coordinate(i, j), false, occupied.Contains(new Coordinate(i, j)) ? SlotState.Occupied : SlotState.Empty);
            }

        return slots;
    }

    Slot[] Mask(Slot[] slots) => slots.Select(e => e with { State = SlotState.Hidden }).ToArray();
}

public class InvalidAttackException : Exception
{
    public InvalidAttackException(string message) : base(message) { }
}
