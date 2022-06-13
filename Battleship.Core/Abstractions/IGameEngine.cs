namespace Battleship.Core.Abstractions;

public interface IGameEngine
{
    bool IsRunning { get; }

    void StartNewGame();
    AttackReport LaunchAttackByComputer();
    AttackReport LaunchAttackByPlayer(Coordinate coordinate);
    GameReport GetReport();
}