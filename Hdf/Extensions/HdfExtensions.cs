namespace AndrejKrizan.Hdf.Extensions
{
    internal static class HdfExtensions
    {
        public static bool IsValidHDFId(this long response)
            => response >= HdfConstants.ResponseErrorCode;
        public static bool IsValidHDFResponse(this int response)
            => response >= HdfConstants.ResponseErrorCode;

        public static long ValidateHDFId(this long response, string action)
            => response.IsValidHDFId()
                ? response
                : throw new ApplicationException($"Failed to {action}. Response: {response}.");
        public static int ValidateHDFResponse(this int response, string action)
            => response.IsValidHDFResponse()
                ? response
                : throw new ApplicationException($"Failed to {action}. Response: {response}.");

        public static long ValidateHDFId(this long response, Func<string> actionFunc)
            => response.IsValidHDFId()
                ? response
                : throw new ApplicationException($"Failed to {actionFunc()}. Response: {response}.");
        public static int ValidateHDFResponse(this int response, Func<string> actionFunc)
            => response.IsValidHDFResponse()
                ? response
                : throw new ApplicationException($"Failed to {actionFunc()}. Response: {response}.");

    }
}
