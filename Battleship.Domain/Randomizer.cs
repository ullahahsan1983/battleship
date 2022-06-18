namespace Battleship.Domain;

public static class Randomizer
{
    public static int FromRange(int lowerRange, int upperRange)
        => Random.Shared.Next(lowerRange, upperRange + 1);
    
    public static T? FromArray<T>(T[] array)
    {
        if (array == null || array.Length == 0)
            return default;
        if (array.Length == 1)
            return array[0];
        
        return array[Random.Shared.Next(0, array.Length)];
    }

    public static T? FromList<T>(List<T> list)
    {
        if (list == null || list.Count == 0)
            return default;
        if (list.Count == 1)
            return list[0];

        return list[Random.Shared.Next(0, list.Count)];
    }
}