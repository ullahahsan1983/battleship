namespace Battleship.Core.Abstractions;

public interface IPlayerSimulator
{
    Coordinate SelectAttackSpot(Region region, Slot[] revealedSlots);
}
