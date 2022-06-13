using Battleship.Core;
using Battleship.Core.Abstractions;

namespace Battleship.Domain;

/// <summary>
/// Game board provider, desgined as a single running instance
/// </summary>
public class GameBoardProvider : IGameBoardProvider
{
    private GameBoard? _gameBoard;

    public GameBoard CreateNew()
    {
        _gameBoard = new GameBoard { Id = Guid.NewGuid() };

        return _gameBoard;
    }

    public GameBoard GetCurrent()
        => _gameBoard ?? CreateNew();
}
