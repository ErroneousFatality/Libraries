namespace AndrejKrizan.DotNet.Extensions
{
    public static class StreamExtensions
    {
        public static async Task<byte[]> ToBytesAsync(this Stream stream, CancellationToken cancellationToken = default)
        {
            if (stream is MemoryStream memoryStream)
            {
                return memoryStream.ToArray();
            }
            using (memoryStream = new MemoryStream())
            {
                await stream.CopyToAsync(memoryStream, cancellationToken);
                return memoryStream.ToArray();
            }
        }

        public static async Task<string> ReadAsync(this Stream stream, CancellationToken cancellationToken = default)
        {
            using StreamReader reader = new(stream);
            return await reader.ReadToEndAsync(cancellationToken);
        }
    }
}
