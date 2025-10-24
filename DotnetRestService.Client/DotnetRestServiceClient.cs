using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using DotnetRestService.API;
using DotnetRestService.API.Dtos;

namespace DotnetRestService.Client;

public class DotnetRestServiceClient : IDotnetRestServiceService
{
    private readonly HttpClient _httpClient;
    private readonly string _baseUrl;
    private readonly bool _ownsHttpClient;

    private DotnetRestServiceClient(HttpClient httpClient, string baseUrl, bool ownsHttpClient = false)
    {
        _httpClient = httpClient;
        _baseUrl = baseUrl.TrimEnd('/');
        _ownsHttpClient = ownsHttpClient;
    }

    public static DotnetRestServiceClient Of(string baseUrl)
    {
        var httpClient = new HttpClient();
        return new DotnetRestServiceClient(httpClient, baseUrl, ownsHttpClient: true);
    }

    public static DotnetRestServiceClient Of(HttpClient httpClient, string baseUrl)
    {
        return new DotnetRestServiceClient(httpClient, baseUrl, ownsHttpClient: false);
    }

    public async Task<CreateDotnetRestResponse> CreateDotnetRest(DotnetRestDto request)
    {
        var response = await _httpClient.PostAsJsonAsync($"{_baseUrl}/api/DotnetRestService", request);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<CreateDotnetRestResponse>() ?? throw new InvalidOperationException("Failed to deserialize response");
    }

    public async Task<GetDotnetRestsResponse> GetDotnetRests(GetDotnetRestsRequest request)
    {
        var queryString = $"?startPage={request.StartPage}&pageSize={request.PageSize}";
        var response = await _httpClient.GetAsync($"{_baseUrl}/api/DotnetRestService{queryString}");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<GetDotnetRestsResponse>() ?? throw new InvalidOperationException("Failed to deserialize response");
    }

    public async Task<GetDotnetRestResponse> GetDotnetRest(string id)
    {
        var response = await _httpClient.GetAsync($"{_baseUrl}/api/DotnetRestService/{id}");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<GetDotnetRestResponse>() ?? throw new InvalidOperationException("Failed to deserialize response");
    }

    public async Task<UpdateDotnetRestResponse> UpdateDotnetRest(DotnetRestDto request)
    {
        if (string.IsNullOrEmpty(request.Id))
            throw new ArgumentException("Id is required for update operation", nameof(request));
            
        var response = await _httpClient.PutAsJsonAsync($"{_baseUrl}/api/DotnetRestService/{request.Id}", request);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<UpdateDotnetRestResponse>() ?? throw new InvalidOperationException("Failed to deserialize response");
    }

    public async Task<DeleteDotnetRestResponse> DeleteDotnetRest(string id)
    {
        var response = await _httpClient.DeleteAsync($"{_baseUrl}/api/DotnetRestService/{id}");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<DeleteDotnetRestResponse>() ?? throw new InvalidOperationException("Failed to deserialize response");
    }

    public void SetAuthorizationToken(string token)
    {
        _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
    }

    public void Dispose()
    {
        if (_ownsHttpClient)
        {
            _httpClient?.Dispose();
        }
    }
}