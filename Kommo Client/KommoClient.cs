using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text;
using System.Text.Json.Serialization;

namespace Kommo_Client
{
    /// <summary>
    /// Minimal Kommo API client.  
    ///  – Inject a valid <c>accessToken</c> (Bearer) that you obtained via OAuth2.  
    ///  – Provide the Kommo <c>subdomain</c> (e.g. "mycompany" for mycompany.kommo.com).  
    /// </summary>
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

        /// <summary>
        /// Creates a new <strong>Lead</strong> in Kommo that represents an incoming order.
        /// </summary>
        /// <param name="order">Your domain model describing the order.</param>
        /// <returns>ID of the created lead (order) inside Kommo.</returns>
        public async Task AddLeadAsync(string name, decimal price, long? contactId = null)
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

            var response = await _http.PostAsync($"{_baseUri}/leads", content);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine("✅ Lead qo‘shildi: " + responseContent);
        }

        public void Dispose() => _http.Dispose();

        public async Task<string> GetAccessTokenAsync(string clientId, string clientSecret, string code, string redirectUri, string subdomain)
        {
            using var client = new HttpClient();

            var requestBody = new Dictionary<string, string>
            {
                { "client_id", clientId },
                { "client_secret", clientSecret },
                { "grant_type", "authorization_code" },
                { "code", code },
                { "redirect_uri", redirectUri }
            };

            var request = new HttpRequestMessage(HttpMethod.Post, $"https://{subdomain}.kommo.com/oauth2/access_token")
            {
                Content = new FormUrlEncodedContent(requestBody)
            };

            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            return content; // JSON ichida: access_token, refresh_token, expires_in, token_type va boshqalar
        }
    }

    #region Helper DTOs

    public sealed record OrderDto
    (
        string? ExternalId,
        string Name,
        decimal Amount,
        long ContactId,
        long StatusId = 0
    )
    {
        /// <summary>
        /// Map domain values to Kommo custom fields if you have any.
        /// </summary>
        public IEnumerable<object>? ToCustomFields()
        {
            //  Example: return new[]
            //  {
            //      new { field_id = 123456, values = new[]{ new{ value = ExternalId } } }
            //  };
            return null;
        }
    }

    internal sealed class CreateLeadResponse
    {
        [JsonPropertyName("_embedded")]
        public LeadWrapper? _embedded { get; init; }

        internal sealed class LeadWrapper
        {
            [JsonPropertyName("leads")]
            public List<Lead>? Leads { get; init; }
        }
        internal sealed class Lead
        {
            [JsonPropertyName("id")]
            public long Id { get; init; }
        }
    }

    #endregion
}
