namespace AndrejKrizan.DotNet.Statistics;
public static class StatisticalFunctions
{
    /// <summary>Works in θ(size) time and space.</summary>
    public static HashSet<int> SampleUniques(int size, int min, int max)
    {
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
