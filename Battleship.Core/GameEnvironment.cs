using Battleship.Core.Abstractions;

namespace Battleship.Core;

public class GameEnvironment : IGameEnvironment
{
    public int BattleGridSize => 10;

    public int NoOfDestroyers => 2;

    public int NoOfBattleships => 1;
}
