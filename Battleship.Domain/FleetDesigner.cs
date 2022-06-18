using Battleship.Core;
using Battleship.Core.Abstractions;

namespace Battleship.Domain;

public class FleetDesigner : IFleetDesigner
{
    private readonly IGridPlannerFactory _plannerFactory;

    public FleetDesigner(IGridPlannerFactory plannerFactory)
    {
        _plannerFactory = plannerFactory;
    }

    public Fleet CreateFleet(string name, Region region, VesselType[] vesselTypes)
    {
        if (vesselTypes is null || vesselTypes.Length == 0)
            throw new ArgumentNullException(nameof(vesselTypes));

        var planner = _plannerFactory.Create(region);

        var fleet = new Fleet { Name = name, Region = region };

        var vessels = new List<Vessel>();
        Array.ForEach(vesselTypes, v =>
        {
            var vessel = new Vessel
            {
                Type = v,
                IsVertical = Randomizer.FromRange(0,1) == 1,
                MaxHealth = (int)v,
                HealthBar = (int)v,
            };

            vessel.Coordinates = vessel.IsVertical 
                ? planner.BookVertically(vessel.MaxHealth) 
                : planner.BookHorizontally(vessel.MaxHealth);

            if (vessel.Coordinates == null || vessel.Coordinates.Length < vessel.MaxHealth)
                throw new InvalidOperationException($"Could not deploy vessel {vessel.Type}");

            vessels.Add(vessel);
        });

        fleet.Vessels = vessels.ToArray();

        return fleet;
    }
}
