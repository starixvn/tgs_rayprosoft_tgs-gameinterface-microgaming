using System.Net;
using System.Net.Http.Json;
using System.Text.Json.Nodes;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using RichardSzalay.MockHttp;

namespace Tgs.GameInterface.MicroGaming.Core.UnitTest;

public class GameApiClient_PlayersSessionsUnitTest
{
	[Fact]
	public async Task Normal_Process()
	{
		// Arrange
		var stsApiBaseUrl = new Uri("https://sts-demo.k2net.io");
		var clientId = "abc123";
		var clientSecret = "01J2WPK2XEN8CK2MR2PB5N39JX";
		var apiBaseUrl = new Uri("https://api-demo.k2net.io/api/v1");
		var agentCode = "01J2X89FF0E6C5SDXQWF6J8SM7";
		var playerId = "4ucmqplxb029";
		var langCode = "zh-cn";
		var platform = GamePlatform.Mobile;
		var contentCode = "SMG_108Heroes";

		var handler = new MockHttpMessageHandler();
		handler
			.Expect(HttpMethod.Post, new Uri(apiBaseUrl, $"/agents/{agentCode}/players/{playerId}/sessions").ToString())
			.WithHeaders(new Dictionary<string, string>
			{
				["Authorization"] = "Bearer 01J2X8HBTR42XAH5NAPTRSDPHQ",
			})
			.Respond(HttpStatusCode.Created, JsonContent.Create(JsonNode.Parse("""
			{
				"url": "https://mnetroul.pauyolima.com/platform/default.aspx?applicationid=163&productId=34117&gameid=108heroesDesktop&brand=Brand&loginType=VanguardSessionToken&languageCode=en-us&externalToken=TOKEN&host=desktop&clienttypeid=70&launchHash=o3jpeLjuDgMfFBpyVxhTI4mMrFjEmv-MBZSbphOWByc",
				"gameURL": "https://mnetroul.pauyolima.com/platform/default.aspx?applicationid=163&productId=34117&gameid=108heroesDesktop&brand=Brand&loginType=VanguardSessionToken&languageCode=en-us&externalToken=TOKEN&host=desktop&clienttypeid=70&launchHash=o3jpeLjuDgMfFBpyVxhTI4mMrFjEmv-MBZSbphOWByc"
			}
			""")));

		var fakerHttpClient = handler.ToHttpClient();
		var fakerTokenRepo = Substitute.For<IStsTokenRepository>();
		fakerTokenRepo
			.GetTokenAsync(
				apiBaseUrl: stsApiBaseUrl,
				clientId: clientId,
				clientSecret: clientSecret,
				cancellationToken: default)
			.Returns("01J2X8HBTR42XAH5NAPTRSDPHQ");
		var fakerDateTimeConverter = Substitute.For<IDateTimeConverter>();
		var fakerLogger = NullLogger<GameApiClient>.Instance;
		var sut = new GameApiClient(fakerLogger, fakerTokenRepo, fakerDateTimeConverter, fakerHttpClient);

		// Act
		var actual = await sut
			.PlayersSessionsAsync(
				stsApiBaseUrl: stsApiBaseUrl,
				clientId: clientId,
				clientSecret: clientSecret,
				apiBaseUrl: apiBaseUrl,
				agentCode: agentCode,
				playerId: playerId,
				langCode: langCode,
				platform: platform,
				contentCode: contentCode,
				cancellationToken: default)
			.ConfigureAwait(true);

		// Assert
		Assert.Equal("https://mnetroul.pauyolima.com/platform/default.aspx?applicationid=163&productId=34117&gameid=108heroesDesktop&brand=Brand&loginType=VanguardSessionToken&languageCode=en-us&externalToken=TOKEN&host=desktop&clienttypeid=70&launchHash=o3jpeLjuDgMfFBpyVxhTI4mMrFjEmv-MBZSbphOWByc", actual);
	}
}
