namespace AndrejKrizan.Hdf.Extensions;

internal static class LongExtensions
{
    public static bool IsValidHdfId(this long response)
        => response >= HdfConstants.ResponseErrorCode;

    public static long ValidateHdfId(this long response, string action)
        => response.IsValidHdfId()
            ? response
            : throw new Exception($"Failed to {action}. Response: {response}.");

    public static long ValidateHdfId(this long response, Func<string> actionFunc)
        => response.IsValidHdfId()
            ? response
            : throw new Exception($"Failed to {actionFunc()}. Response: {response}.");
}
