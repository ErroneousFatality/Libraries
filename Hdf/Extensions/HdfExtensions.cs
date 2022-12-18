namespace AndrejKrizan.Hdf.Extensions
{
    internal static class HdfExtensions
    {
        public static bool IsValidHdfId(this long response)
            => response >= HdfConstants.ResponseErrorCode;
        public static bool IsValidHdfResponse(this int response)
            => response >= HdfConstants.ResponseErrorCode;

        public static long ValidateHdfId(this long response, string action)
            => response.IsValidHdfId()
                ? response
                : throw new ApplicationException($"Failed to {action}. Response: {response}.");
        public static int ValidateHdfResponse(this int response, string action)
            => response.IsValidHdfResponse()
                ? response
                : throw new ApplicationException($"Failed to {action}. Response: {response}.");

        public static long ValidateHdfId(this long response, Func<string> actionFunc)
            => response.IsValidHdfId()
                ? response
                : throw new ApplicationException($"Failed to {actionFunc()}. Response: {response}.");
        public static int ValidateHdfResponse(this int response, Func<string> actionFunc)
            => response.IsValidHdfResponse()
                ? response
                : throw new ApplicationException($"Failed to {actionFunc()}. Response: {response}.");

    }
}
