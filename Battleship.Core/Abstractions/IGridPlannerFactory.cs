namespace Battleship.Core.Abstractions;

public interface IGridPlannerFactory
{
    public IGridPlanner Create(Region region);
}