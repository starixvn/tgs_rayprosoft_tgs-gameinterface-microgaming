using System.Diagnostics;
using System.Net;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Web;
using Microsoft.Extensions.Logging;
using Tgs.GameInterface.MicroGaming.Core.Extensions;
using Tgs.GameInterface.MicroGaming.Core.Models;

namespace Tgs.GameInterface.MicroGaming.Core;

internal class GameApiClient(
	ILogger<GameApiClient> logger,
	IStsTokenRepository tokenRepository,
	HttpClient httpClient) : IGameApiClient
{
	private readonly DateTimeConverter _dateTimeConverter = new();

	public async ValueTask PlayersAsync(Uri stsApiBaseUrl, string clientId, string clientSecret, Uri apiBaseUrl, string agentCode, string playerId, CancellationToken cancellationToken = default)
	{
		var token = await tokenRepository.GetTokenAsync(stsApiBaseUrl, clientId, clientSecret, cancellationToken).ConfigureAwait(false);

		var apiUrl = new Uri(apiBaseUrl, $"/agents/{agentCode}/players");

		var parameters = new Dictionary<string, string>
		{
			["playerId"] = playerId,
		};

		await PostAsync(
			logPrefix: nameof(PlayersAsync),
			apiUrl: apiUrl,
			accessToken: token,
			parameters: parameters,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);
	}

	public async ValueTask<(bool isPlayerExist, decimal balance)> PlayersDetailsAsync(Uri stsApiBaseUrl, string clientId, string clientSecret, Uri apiBaseUrl, string agentCode, string playerId, CancellationToken cancellationToken = default)
	{
		var token = await tokenRepository.GetTokenAsync(stsApiBaseUrl, clientId, clientSecret, cancellationToken).ConfigureAwait(false);

		var apiUrl = new Uri(apiBaseUrl, $"/agents/{agentCode}/players/{playerId}");

		var (status, response) = await GetWithParametersAsync(
			logPrefix: nameof(PlayersAsync),
			apiUrl: apiUrl,
			accessToken: token,
			parameters: [],
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		if (status == HttpStatusCode.NotFound)
			return (false, 0m);

		if (status != HttpStatusCode.OK)
			throw new HttpRequestException(HttpRequestError.InvalidResponse, "/players/#playerId api response not OK", statusCode: status);

		var jsonNode = JsonNode.Parse(response)!;
		var balanceNode = jsonNode["balance"]!.AsObject();
		var detailsNode = balanceNode["details"]!.AsObject();
		var walletBalanceNode = detailsNode["walletBalance"]!.AsObject();
		return (true, walletBalanceNode["total"]!.GetValue<decimal>());
	}

	public async ValueTask<string> PlayersSessionsAsync(Uri stsApiBaseUrl, string clientId, string clientSecret, Uri apiBaseUrl, string agentCode, string playerId, string langCode, GamePlatform platform, string contentCode, CancellationToken cancellationToken = default)
	{
		var token = await tokenRepository.GetTokenAsync(stsApiBaseUrl, clientId, clientSecret, cancellationToken).ConfigureAwait(false);

		var apiUrl = new Uri(apiBaseUrl, $"/agents/{agentCode}/players/{playerId}/sessions");

		var parameters = new Dictionary<string, string>
		{
			["langCode"] = langCode,
			["platform"] = platform.ToString(),
			["contentCode"] = contentCode,
		};

		var response = await PostAsync(
			logPrefix: nameof(PlayersSessionsAsync),
			apiUrl: apiUrl,
			accessToken: token,
			parameters: parameters,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		var jsonNode = JsonNode.Parse(response)!;

		return jsonNode["gameURL"]!.GetValue<string>()!;
	}

	public async IAsyncEnumerable<BetsResponseItem> BetsAsync(Uri stsApiBaseUrl, string clientId, string clientSecret, Uri apiBaseUrl, string agentCode, int limit, string startingAfter, [EnumeratorCancellation] CancellationToken cancellationToken = default)
	{
		var token = await tokenRepository.GetTokenAsync(stsApiBaseUrl, clientId, clientSecret, cancellationToken).ConfigureAwait(false);

		var apiUrl = new Uri(apiBaseUrl, $"/agents/{agentCode}/bets");

		var parameters = new Dictionary<string, string>
		{
			["limit"] = limit.ToString(),
			["startingAfter"] = startingAfter,
		};

		var (status, response) = await GetWithParametersAsync(
			logPrefix: nameof(BetsAsync),
			apiUrl: apiUrl,
			accessToken: token,
			parameters: parameters,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		if (status != HttpStatusCode.OK)
			throw new HttpRequestException(HttpRequestError.InvalidResponse, "/bets api response not OK", statusCode: status);

		var jsonNode = JsonNode.Parse(response)!;

		foreach (var jsonObj in jsonNode.AsArray())
		{
			if (jsonObj is null)
				continue;

			var createdDateUtc = _dateTimeConverter.StringConvertToDatetime(jsonObj["createdDateUTC"]!.GetValue<string>()!);
			var gameStartTimeUtc = _dateTimeConverter.StringConvertToDatetime(jsonObj["gameStartTimeUTC"]!.GetValue<string>()!);
			var gameEndTimeUtc = _dateTimeConverter.StringConvertToDatetime(jsonObj["gameEndTimeUTC"]!.GetValue<string>()!);
			yield return new BetsResponseItem(
				BetUid: jsonObj["betUID"]!.GetValue<string>()!,
				CreatedDateUtc: createdDateUtc,
				GameStartTimeUtc: gameStartTimeUtc,
				GameEndTimeUtc: gameEndTimeUtc,
				GameCode: jsonObj["gameCode"]!.GetValue<string>(),
				PlayerId: jsonObj["playerId"]!.GetValue<string>(),
				Currency: jsonObj["currency"]!.GetValue<string>(),
				BetAmount: jsonObj["betAmount"]!.GetValue<decimal>(),
				PayoutAmount: jsonObj["payoutAmount"]!.GetValue<decimal>(),
				BetStatus: Enum.Parse<BetStatus>(jsonObj["betStatus"]!.GetValue<string>()),
				ExternalTransactionId: jsonObj["externalTransactionId"]!.GetValue<string>(),
				AdditionalData: jsonObj.ToJsonString());
		}
	}

	public async ValueTask<WalletTransactionsResponse> WalletTransactionsAsync(Uri stsApiBaseUrl, string clientId, string clientSecret, Uri apiBaseUrl, string agentCode, string playerId, string type, decimal amount, string externalTransactionId, CancellationToken cancellationToken = default)
	{
		var token = await tokenRepository.GetTokenAsync(stsApiBaseUrl, clientId, clientSecret, cancellationToken).ConfigureAwait(false);

		var apiUrl = new Uri(apiBaseUrl, $"/agents/{agentCode}/WalletTransactions");

		var parameters = new Dictionary<string, string>
		{
			["playerId"] = playerId,
			["type"] = type,
			["amount"] = amount.ToString("0.##"),
			["externalTransactionId"] = externalTransactionId,
		};

		var response = await PostAsync(
			logPrefix: nameof(WalletTransactionsAsync),
			apiUrl: apiUrl,
			accessToken: token,
			parameters: parameters,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		var jsonNode = JsonNode.Parse(response)!;

		if (!jsonNode["status"]!.GetValue<string>()!.Equals("succeeded", StringComparison.OrdinalIgnoreCase))
			throw new Exception("WalletTransactions call status not succeeded");

		return new WalletTransactionsResponse(
			Id: jsonNode["id"]!.GetValue<string>()!,
			ExternalTransactionId: jsonNode["externalTransactionId"]!.GetValue<string>()!,
			Amount: jsonNode["amount"]!.GetValue<decimal>()!,
			Resp: response);
	}

	public async ValueTask<bool> WalletTransactionsDetailsAsync(Uri stsApiBaseUrl, string clientId, string clientSecret, Uri apiBaseUrl, string agentCode, string walletTransactionId, CancellationToken cancellationToken = default)
	{
		var token = await tokenRepository.GetTokenAsync(stsApiBaseUrl, clientId, clientSecret, cancellationToken).ConfigureAwait(false);

		var apiUrl = new Uri(apiBaseUrl, $"/agents/{agentCode}/WalletTransactions/{walletTransactionId}");

		var (status, response) = await GetWithParametersAsync(
			logPrefix: nameof(WalletTransactionsDetailsAsync),
			apiUrl: apiUrl,
			accessToken: token,
			parameters: [],
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		if (status == HttpStatusCode.NotFound)
			return false;

		if (status != HttpStatusCode.OK)
			throw new HttpRequestException(HttpRequestError.InvalidResponse, "/WalletTransactions/#Id api response not OK", statusCode: status);

		return true;
	}

	async ValueTask<(HttpStatusCode, string)> GetWithParametersAsync(string logPrefix, Uri apiUrl, string accessToken, IEnumerable<KeyValuePair<string, string>> parameters, CancellationToken cancellationToken)
	{
		var builder = new UriBuilder(apiUrl);
		var query = HttpUtility.ParseQueryString(builder.Query);
		foreach (var param in parameters)
			query[param.Key] = param.Value;
		builder.Query = query.ToString();
		apiUrl = builder.Uri;

		var request = new HttpRequestMessage(HttpMethod.Get, apiUrl);
		request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

		var response = await httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);

		var sw = Stopwatch.StartNew();

		var responseContent = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

		sw.Stop();

		var apiLogEvent = new
		{
			Path = request?.RequestUri?.AbsoluteUri.NoLongerThan(512) ?? string.Empty,
			Method = request?.Method.ToString().NoLongerThan(8) ?? string.Empty,
			Prefix = logPrefix,
			ServerName = Environment.MachineName.NoLongerThan(128),
			Parameters = JsonSerializer.Serialize(parameters),
			RequestHeaders = request?.Headers + request?.Content?.Headers?.ToString(),
			ResponseHeaders = response.Headers.ToString(),
			ResponseContent = responseContent.NoLongerThan(4096),
			ResponseCode = (short)response.StatusCode,
			ExecutionTimeMs = (int)sw.ElapsedMilliseconds,
			RecordedOn = DateTimeOffset.UtcNow
		};
		logger.LogInformation("{apiLogEvent}", JsonSerializer.Serialize(apiLogEvent));

		return (response.StatusCode, responseContent);
	}

	async ValueTask<string> PostAsync(string logPrefix, Uri apiUrl, string accessToken, IDictionary<string, string> parameters, CancellationToken cancellationToken)
	{
		var requestContent = new FormUrlEncodedContent(parameters);

		var request = new HttpRequestMessage(HttpMethod.Post, apiUrl);
		request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
		request.Content = requestContent;

		var response = await httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);

		var sw = Stopwatch.StartNew();

		var responseContent = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

		sw.Stop();

		var apiLogEvent = new
		{
			Path = request?.RequestUri?.AbsoluteUri.NoLongerThan(512) ?? string.Empty,
			Method = request?.Method.ToString().NoLongerThan(8) ?? string.Empty,
			Prefix = logPrefix,
			ServerName = Environment.MachineName.NoLongerThan(128),
			Parameters = JsonSerializer.Serialize(parameters),
			RequestHeaders = request?.Headers + request?.Content?.Headers?.ToString(),
			RequestContent = (await requestContent.ReadAsStringAsync(cancellationToken).ConfigureAwait(false)).NoLongerThan(4096),
			ResponseHeaders = response.Headers.ToString(),
			ResponseContent = responseContent.NoLongerThan(4096),
			ResponseCode = (short)response.StatusCode,
			ExecutionTimeMs = (int)sw.ElapsedMilliseconds,
			RecordedOn = DateTimeOffset.UtcNow
		};
		logger.LogInformation("{apiLogEvent}", JsonSerializer.Serialize(apiLogEvent));

		response.EnsureSuccessStatusCode();

		return responseContent;
	}
}
