using Battleship.Core;
using Battleship.Core.Abstractions;

namespace Battleship.Domain;

public class GridPlannerFactory : IGridPlannerFactory
{
    public IGridPlanner Create(Region region) 
        => new GridPlanner(region);
}