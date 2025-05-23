namespace AndrejKrizan.Hdf.Extensions;

internal static class IntExtensions
{
    public static bool IsValidHdfResponse(this int response)
        => response >= HdfConstants.ResponseErrorCode;

    public static int ValidateHdfResponse(this int response, string action)
        => response.IsValidHdfResponse()
            ? response
            : throw new Exception($"Failed to {action}. Response: {response}.");

    public static int ValidateHdfResponse(this int response, Func<string> actionFunc)
        => response.IsValidHdfResponse()
            ? response
            : throw new Exception($"Failed to {actionFunc()}. Response: {response}.");
}
