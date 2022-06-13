namespace Battleship.Core;

public record struct Slot(int Id, Coordinate Coordinate, bool IsRevealed, SlotState State);