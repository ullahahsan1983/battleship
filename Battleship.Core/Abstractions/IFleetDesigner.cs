namespace Battleship.Core.Abstractions;

public interface IFleetDesigner
{
    Fleet CreateFleet(string name, Region region, VesselType[] vesselTypes);
}
