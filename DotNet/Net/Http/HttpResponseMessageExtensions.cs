using System.Net;
using System.Net.Http.Headers;

namespace AndrejKrizan.DotNet.Net.Http;

public static class HttpResponseMessageExtensions
{
    extension(HttpResponseMessage responseMessage)
    {
        /// <exception cref="ArgumentException"></exception>
        public string GetFileName()
        {
         ContentDispositionHeaderValue contentDisposition = responseMessage.Content.Headers.ContentDisposition
                ?? throw new ArgumentException("The HTTP response message is missing the content disposition header.", nameof(responseMessage));
            string fileName = contentDisposition.FileNameStar
                ?? WebUtility.UrlDecode(contentDisposition.FileName)?.Replace("\"", string.Empty)
                ?? throw new ArgumentException("The HTTP response message is missing the file name values inside the disposition header.", nameof(responseMessage));
            return fileName;
        }

        /// <exception cref="ArgumentException"></exception>
        public string GetMediaType()
        {
            MediaTypeHeaderValue contentTypeHeader = responseMessage.Content.Headers.ContentType
                ?? throw new ArgumentException("The HTTP response message is missing the content type header.", nameof(responseMessage));
            string mediaType = contentTypeHeader.MediaType
                ?? throw new ArgumentException("The HTTP response message is missing the media type value inside the content type header.", nameof(responseMessage));
            return mediaType;
        }
    }
}
