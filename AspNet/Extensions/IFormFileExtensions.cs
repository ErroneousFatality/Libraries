using Microsoft.AspNetCore.Http;

namespace AndrejKrizan.AspNet.Extensions;

public static class IFormFileExtensions
{
    public static async Task<byte[]> ToBytesAsync(this IFormFile file, CancellationToken cancellationToken = default)
    {
        byte[] code;
        using (MemoryStream memoryStream = new())
        {
            await file.CopyToAsync(memoryStream, cancellationToken);
            code = memoryStream.ToArray();
        }
        return code;
    }
}
