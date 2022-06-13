namespace Battleship.Core.Abstractions;

public interface IGameBoardProvider
{
    GameBoard CreateNew();
    GameBoard GetCurrent();
}