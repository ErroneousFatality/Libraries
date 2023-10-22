namespace AndrejKrizan.DotNet.Statistics;
public static class StatisticalFunctions
{
    /// <summary>Works in θ(size) time and space.</summary>
    /// <param name="size">Must be non-negative.</param>
    public static HashSet<int> SampleUniques(int size, int min, int max)
    {
        if (size < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(size), "The size must be a non-negative integer.");
        }
        if (min > max - size)
        {
            throw new ArgumentException("The range of available values (max - min) must can not be less than size.");
        }
        Random generator = new();
        HashSet<int> uniques = new(size);
        for (int index = max - size + 1; index <= max; index++)
        {
            int random = generator.Next(min, index);
            if (uniques.Contains(random))
            {
                uniques.Add(index);
            }
            else
            {
                uniques.Add(random);
            }
        }
        return uniques;
    }
}
