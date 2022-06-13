namespace Battleship.Core.Abstractions;

public interface IGameEnvironment
{
    public int BattleGridSize { get; }
    public int NoOfDestroyers { get; }
    public int NoOfBattleships { get; }
}