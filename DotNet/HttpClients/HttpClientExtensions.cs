using System.Net.Http.Json;

namespace AndrejKrizan.DotNet.HttpClients;
public static class HttpClientExtensions
{
    /// <exception cref="HttpRequestException"></exception>
    public static async Task<TResponse> PostAndReadJsonAsync<TRequest, TResponse>(this HttpClient httpClient, string? requestUri, TRequest request, CancellationToken cancellationToken = default)
    {
        HttpResponseMessage httpResponse = await httpClient.PostAsJsonAsync(requestUri, request, cancellationToken);
        httpResponse.EnsureSuccessStatusCode();
        TResponse response = (await httpResponse.Content.ReadFromJsonAsync<TResponse>(cancellationToken))!;
        return response;
    }
}
