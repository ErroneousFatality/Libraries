using System.Net;
using System.Net.Http.Json;

using AndrejKrizan.DotNet.Net.Http.Json.ResponseResults;

namespace AndrejKrizan.DotNet.Net.Http.Json;

public static class HttpResponseMessageExtensions
{
    extension(HttpResponseMessage Response)
    {
        public async Task<ResponseResult> GetJsonResultAsync(CancellationToken cancellationToken = default)
        {
            ResponseResult result;
            if (Response.IsSuccessStatusCode)
            {
                result = ResponseResult.CreateSuccessful(Response.StatusCode);
            }
            else
            {
                ErrorResponse error = (await Response.Content.ReadFromJsonAsync<ErrorResponse>(cancellationToken))!;
                result = ResponseResult.CreateErroneous(error, Response.StatusCode);
            }
            return result;
        }

        public async Task<ResponseResult<TContent>> GetJsonResultAsync<TContent>(CancellationToken cancellationToken = default)
        {
            ResponseResult<TContent> result;
            if (Response.IsSuccessStatusCode)
            {
                TContent? content = Response.StatusCode == HttpStatusCode.NoContent
                    ? default
                    : (await Response.Content.ReadFromJsonAsync<TContent>(cancellationToken))!;
                result = ResponseResult<TContent>.CreateSuccessful(content, Response.StatusCode);
            }
            else
            {
                ErrorResponse error = (await Response.Content.ReadFromJsonAsync<ErrorResponse>(cancellationToken))!;
                result = ResponseResult<TContent>.CreateErroneous(error, Response.StatusCode);
            }
            return result;
        }


        /// <exception cref="HttpRequestException"></exception>
        public async Task<TContent> GetJsonContentAsync<TContent>(CancellationToken cancellationToken = default)
        {
            ResponseResult<TContent> result = await Response.GetJsonResultAsync<TContent>(cancellationToken);
            result.EnsureIsSuccessful();
            TContent content = result.Content!;
            return content;
        }

        /// <exception cref="HttpRequestException"></exception>
        public async Task EnsureIsSuccessfulAsync(CancellationToken cancellationToken = default)
        {
            ResponseResult result = await Response.GetJsonResultAsync(cancellationToken);
            result.EnsureIsSuccessful();
        }
    }
}