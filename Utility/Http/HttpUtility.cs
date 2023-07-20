using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Utility.Http;

public class HttpUtility : IHttpUtility
{
    public string BaseUrl { get; set; }

    public HttpUtility(){ }

    public async Task<string> GetAsync(string path)
    {
        var client = InitClient();
        var response = await client.GetAsync(path);
        return await ReturnResult(response, path, "GET");
    }

    public async Task<string> PostAsync(string path, JsonNode body)
    {
        var client = InitClient();
        var content = new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json");
        var response = await client.PostAsync(path, content);
        return await ReturnResult(response, path, "POST");
    }

    public async Task<string> PutAsync(string path, JsonNode body)
    {
        var client = InitClient();
        var content = new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json");
        var response = await client.PutAsync(path, content);
        return await ReturnResult(response, path, "PUT");
    }
    
    public async Task<string> DeleteAsync(string path)
    {
        var client = InitClient();
        var response = await client.DeleteAsync(path);
        return await ReturnResult(response, path, "DELETE");
    }

    private HttpClient InitClient()
    {
        var client = new HttpClient();
        client.BaseAddress = new Uri(BaseUrl);
        return client;
    }

    private async Task<string> ReturnResult(HttpResponseMessage? response, string path, string type)
    {
        if (!response.IsSuccessStatusCode)
            throw new Exception($"HTTP ${type} Request to {path.Split("?")[0]} failed with {response.StatusCode} code. {await response.Content.ReadAsStringAsync() ?? ""}");
        var content = await response.Content.ReadAsStringAsync();
        return content;   
    }
}