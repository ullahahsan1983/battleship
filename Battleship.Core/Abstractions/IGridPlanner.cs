namespace Battleship.Core.Abstractions;

public interface IGridPlanner
{
    Coordinate[] BookHorizontally(int span);
    Coordinate[] BookVertically(int span);
    bool IsAvailable(Coordinate coordinate, int span, bool vertical);
}