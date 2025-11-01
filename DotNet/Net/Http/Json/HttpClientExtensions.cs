using System.Net.Http.Json;

using AndrejKrizan.DotNet.Net.Http.Json.ResponseResults;

namespace AndrejKrizan.DotNet.Net.Http.Json;

public static class HttpClientExtensions
{
    #region Post

    public static async Task<ResponseResult> TryPostJsonAsync(this HttpClient httpClient, string? uri, CancellationToken cancellationToken = default)
    {
        HttpResponseMessage httpResponseMessage = await httpClient.PostAsync(uri, content: null, cancellationToken);
        ResponseResult responseResult = await httpResponseMessage.GetJsonResultAsync(cancellationToken);
        return responseResult;
    }

    public static async Task<ResponseResult> TryPostJsonAsync<TRequest>(this HttpClient httpClient, string? uri, TRequest request, CancellationToken cancellationToken = default)
    {
        HttpResponseMessage httpResponseMessage = await httpClient.PostAsJsonAsync(uri, request, cancellationToken);
        ResponseResult responseResult = await httpResponseMessage.GetJsonResultAsync(cancellationToken);
        return responseResult;
    }

    public static async Task<ResponseResult<TResponse>> TryPostJsonAsync<TRequest, TResponse>(this HttpClient httpClient, string? uri, TRequest request, CancellationToken cancellationToken = default)
    {
        HttpResponseMessage httpResponseMessage = await httpClient.PostAsJsonAsync(uri, request, cancellationToken);
        ResponseResult<TResponse> responseResult = await httpResponseMessage.GetJsonResultAsync<TResponse>(cancellationToken);
        return responseResult;
    }


    /// <exception cref="HttpRequestException"></exception>
    public static async Task PostJsonAsync(this HttpClient httpClient, string? uri, CancellationToken cancellationToken = default)
    {
        ResponseResult responseResult = await httpClient.TryPostJsonAsync(uri, cancellationToken);
        responseResult.EnsureIsSuccessful();
    }

    /// <exception cref="HttpRequestException"></exception>
    public static async Task PostJsonAsync<TRequest>(this HttpClient httpClient, string? uri, TRequest request, CancellationToken cancellationToken = default)
    {
        ResponseResult responseResult = await httpClient.TryPostJsonAsync(uri, request, cancellationToken);
        responseResult.EnsureIsSuccessful();
    }

    /// <exception cref="HttpRequestException"></exception>
    public static async Task<TResponse> PostJsonAsync<TRequest, TResponse>(this HttpClient httpClient, string? uri, TRequest request, CancellationToken cancellationToken = default)
    {
        ResponseResult<TResponse> responseResult = await httpClient.TryPostJsonAsync<TRequest, TResponse>(uri, request, cancellationToken);
        responseResult.EnsureIsSuccessful();
        TResponse response = responseResult.Content!;
        return response;
    }

    #endregion

    #region Get

    public static async Task<ResponseResult<TResponse>> TryGetJsonAsync<TResponse>(this HttpClient httpClient, string? uri, CancellationToken cancellationToken = default)
    {
        HttpResponseMessage httpResponseMessage = await httpClient.GetAsync(uri, cancellationToken);
        ResponseResult<TResponse> responseResult = await httpResponseMessage.GetJsonResultAsync<TResponse>(cancellationToken);
        return responseResult;
    }

    /// <exception cref="HttpRequestException"></exception>
    public static async Task<TResponse> GetJsonAsync<TResponse>(this HttpClient httpClient, string? uri, CancellationToken cancellationToken = default)
    {
        ResponseResult<TResponse> responseResult = await httpClient.TryGetJsonAsync<TResponse>(uri, cancellationToken);
        responseResult.EnsureIsSuccessful();
        TResponse response = responseResult.Content!;
        return response;
    }

    #endregion

    #region Put

    public static async Task<ResponseResult> TryPutJsonAsync(this HttpClient httpClient, string? uri, CancellationToken cancellationToken = default)
    {
        HttpResponseMessage httpResponseMessage = await httpClient.PutAsync(uri, content: null, cancellationToken);
        ResponseResult responseResult = await httpResponseMessage.GetJsonResultAsync(cancellationToken);
        return responseResult;
    }

    public static async Task<ResponseResult> TryPutJsonAsync<TRequest>(this HttpClient httpClient, string? uri, TRequest request, CancellationToken cancellationToken = default)
    {
        HttpResponseMessage httpResponseMessage = await httpClient.PutAsJsonAsync(uri, request, cancellationToken);
        ResponseResult responseResult = await httpResponseMessage.GetJsonResultAsync(cancellationToken);
        return responseResult;
    }

    public static async Task<ResponseResult<TResponse>> TryPutJsonAsync<TRequest, TResponse>(this HttpClient httpClient, string? uri, TRequest request, CancellationToken cancellationToken = default)
    {
        HttpResponseMessage httpResponseMessage = await httpClient.PutAsJsonAsync(uri, request, cancellationToken);
        ResponseResult<TResponse> responseResult = await httpResponseMessage.GetJsonResultAsync<TResponse>(cancellationToken);
        return responseResult;
    }


    /// <exception cref="HttpRequestException"></exception>
    public static async Task PutJsonAsync(this HttpClient httpClient, string? uri, CancellationToken cancellationToken = default)
    {
        ResponseResult responseResult = await httpClient.TryPutJsonAsync(uri, cancellationToken);
        responseResult.EnsureIsSuccessful();
    }

    /// <exception cref="HttpRequestException"></exception>
    public static async Task PutJsonAsync<TRequest>(this HttpClient httpClient, string? uri, TRequest request, CancellationToken cancellationToken = default)
    {
        ResponseResult responseResult = await httpClient.TryPutJsonAsync(uri, request, cancellationToken);
        responseResult.EnsureIsSuccessful();
    }

    /// <exception cref="HttpRequestException"></exception>
    public static async Task<TResponse> PustJsonAsync<TRequest, TResponse>(this HttpClient httpClient, string? uri, TRequest request, CancellationToken cancellationToken = default)
    {
        ResponseResult<TResponse> responseResult = await httpClient.TryPutJsonAsync<TRequest, TResponse>(uri, request, cancellationToken);
        responseResult.EnsureIsSuccessful();
        TResponse response = responseResult.Content!;
        return response;
    }

    #endregion

    #region Delete
    public static async Task<ResponseResult> TryDeleteJsonAsync(this HttpClient httpClient, string? uri, CancellationToken cancellationToken = default)
    {
        HttpResponseMessage httpResponseMessage = await httpClient.DeleteAsync(uri, cancellationToken);
        ResponseResult responseResult = await httpResponseMessage.GetJsonResultAsync(cancellationToken);
        return responseResult;
    }

    public static async Task<ResponseResult<TResponse>> TryDeleteJsonAsync<TResponse>(this HttpClient httpClient, string? uri, CancellationToken cancellationToken = default)
    {
        HttpResponseMessage httpResponseMessage = await httpClient.DeleteAsync(uri, cancellationToken);
        ResponseResult<TResponse> responseResult = await httpResponseMessage.GetJsonResultAsync<TResponse>(cancellationToken);
        return responseResult;
    }


    /// <exception cref="HttpRequestException"></exception>
    public static async Task DeleteJsonAsync(this HttpClient httpClient, string? uri, CancellationToken cancellationToken = default)
    {
        ResponseResult responseResult = await httpClient.TryDeleteJsonAsync(uri, cancellationToken);
        responseResult.EnsureIsSuccessful();
    }

    /// <exception cref="HttpRequestException"></exception>
    public static async Task<TResponse> DeleteJsonAsync<TResponse>(this HttpClient httpClient, string? uri, CancellationToken cancellationToken = default)
    {
        ResponseResult<TResponse> responseResult = await httpClient.TryDeleteJsonAsync<TResponse>(uri, cancellationToken);
        responseResult.EnsureIsSuccessful();
        TResponse response = responseResult.Content!;
        return response;
    }

    #endregion
}
