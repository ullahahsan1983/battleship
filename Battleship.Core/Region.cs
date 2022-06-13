namespace Battleship.Core;

public record struct Region(int Rows, int Cols);

public record struct Coordinate(int RowIndex, int ColIndex);