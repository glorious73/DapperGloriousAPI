using System.Text.Json.Nodes;

namespace Utility.Http;

public interface IHttpUtility
{
    string BaseUrl { get; set; }
    Task<string> GetAsync(string path);
    Task<string> PostAsync(string path, JsonNode body);
    Task<string> PutAsync(string path, JsonNode body);
    Task<string> DeleteAsync(string path);
}