namespace Battleship.Core;

public enum SlotState
{
    // Masked
    Hidden,
    Empty,
    Occupied, 
    // Hitted
    Damaged, 
    // Occupier Destroyed
    Destroyed 
}

public enum AttackState { Missed, Hit, Invalid }
