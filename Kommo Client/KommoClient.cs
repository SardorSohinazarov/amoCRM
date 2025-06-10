using System.Net.Http.Headers;
using System.Text.Json;
using System.Text;

namespace Kommo_Client;

public sealed class KommoClient : IDisposable
{
    private readonly HttpClient _http;
    private readonly string _baseUri; // e.g. https://subdomain.kommo.com/api/v4

    public KommoClient(string subdomain, string accessToken, HttpClient? httpClient = null)
    {
        if (string.IsNullOrWhiteSpace(subdomain)) throw new ArgumentException("Subdomain is required", nameof(subdomain));
        if (string.IsNullOrWhiteSpace(accessToken)) throw new ArgumentException("Access‑token is required", nameof(accessToken));

        _http = httpClient ?? new HttpClient();
        _baseUri = $"https://{subdomain}.kommo.com/api/v4";

        // Static headers
        _http.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
    }

    public async Task<AddLeadResponse> AddLeadAsync(string name, decimal price, long? contactId = null, CancellationToken cancellationToken = default)
    {
        var leadData = new
        {
            name = name,
            price = price,
            _embedded = contactId.HasValue
                ? new { contacts = new[] { new { id = contactId.Value } } }
                : null
        };

        var leadsPayload = new
        {
            add = new[] { leadData }
        };

        var json = JsonSerializer.Serialize(new[] { leadData });
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _http.PostAsync($"{_baseUri}/leads", content, cancellationToken);
        response.EnsureSuccessStatusCode();

        var responseContent = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<AddLeadResponse>(responseContent);
    }

    public async Task<UpdateLeadResponse> UpdateLeadAsync(long leadId, string? name = null, decimal? price = null, CancellationToken cancellationToken = default)
    {
        if (leadId <= 0) throw new ArgumentException("Lead ID must be a positive number", nameof(leadId));

        var updateData = new Dictionary<string, object>
        {
            { "id", leadId }
        };

        if (!string.IsNullOrWhiteSpace(name))
            updateData["name"] = name;

        if (price.HasValue)
            updateData["price"] = price.Value;

        var json = JsonSerializer.Serialize(new[] { updateData });
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _http.PatchAsync($"{_baseUri}/leads", content, cancellationToken);
        response.EnsureSuccessStatusCode();

        var responseContent = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<UpdateLeadResponse>(responseContent);
    }

    public void Dispose() => _http.Dispose();
}

#region Helper DTOs
// Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
public class Embedded
{
    public List<Lead> leads { get; set; }
}

public class Links
{
    public Self self { get; set; }
}

public class AddLeadResponse
{
    public Links _links { get; set; }
    public Embedded _embedded { get; set; }
}

public class Self
{
    public string href { get; set; }
}

public class Lead
{
    public int id { get; set; }
    public int updated_at { get; set; }
    public Links _links { get; set; }
}

public class UpdateLeadResponse
{
    public Links _links { get; set; }
    public Embedded _embedded { get; set; }
}
#endregion
