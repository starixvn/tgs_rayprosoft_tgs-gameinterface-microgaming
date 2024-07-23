using System.Text.Json.Nodes;

namespace Tgs.GameInterface.MicroGaming.Core;

internal class StsTokenRepository(HttpClient httpClient) : IStsTokenRepository
{
	public async ValueTask<string> GetTokenAsync(Uri apiBaseUrl, string clientId, string clientSecret, CancellationToken cancellationToken = default)
	{
		var requestContent = new FormUrlEncodedContent(new Dictionary<string, string>
		{
			["client_id"] = clientId,
			["client_secret"] = clientSecret,
			["grant_type"] = "client_credentials",
		});

		var apiUrl = new Uri(apiBaseUrl, "/connect/token");

		var request = new HttpRequestMessage(HttpMethod.Post, apiUrl)
		{
			Content = requestContent,
		};

		var response = await httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);

		response.EnsureSuccessStatusCode();

		var responseContent = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

		var jsonNode = JsonNode.Parse(responseContent);

		return jsonNode!["access_token"]!.GetValue<string>()!;
	}
}
