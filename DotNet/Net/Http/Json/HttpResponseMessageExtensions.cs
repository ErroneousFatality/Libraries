using System.Net;
using System.Net.Http.Json;

using AndrejKrizan.DotNet.Net.Http.Json.ResponseResults;

namespace AndrejKrizan.DotNet.Net.Http.Json;

public static class HttpResponseMessageExtensions
{
    public static async Task<ResponseResult> GetJsonResultAsync(this HttpResponseMessage response, CancellationToken cancellationToken = default)
    {
        ResponseResult result;
        if (response.IsSuccessStatusCode)
        {
            result = ResponseResult.CreateSuccessful(response.StatusCode);
        }
        else
        {
            ErrorResponse error = (await response.Content.ReadFromJsonAsync<ErrorResponse>(cancellationToken))!;
            result = ResponseResult.CreateErroneous(error, response.StatusCode);
        }
        return result;
    }

    public static async Task<ResponseResult<TContent>> GetJsonResultAsync<TContent>(this HttpResponseMessage response, CancellationToken cancellationToken = default)
    {
        ResponseResult<TContent> result;
        if (response.IsSuccessStatusCode)
        {
            TContent? content = response.StatusCode == HttpStatusCode.NoContent
                ? default
                : (await response.Content.ReadFromJsonAsync<TContent>(cancellationToken))!;
            result = ResponseResult<TContent>.CreateSuccessful(content, response.StatusCode);
        }
        else
        {
            ErrorResponse error = (await response.Content.ReadFromJsonAsync<ErrorResponse>(cancellationToken))!;
            result = ResponseResult<TContent>.CreateErroneous(error, response.StatusCode);
        }
        return result;
    }
}