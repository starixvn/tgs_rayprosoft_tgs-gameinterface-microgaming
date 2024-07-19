using Microsoft.AspNetCore.Mvc;
using Tgs.GameInterface.MicroGaming.Core.ApplicationServices;
using Tgs.ServiceUtility;

namespace Tgs.GameInterface.MicroGaming.Controllers;

[ApiController]
[Route("[controller]")]
[Produces("application/json")]
[Consumes("application/json")]
public class MicroGamingController : ControllerBase
{
	[HttpPost("PlayerCreate")]
	public ValueTask PlayerCreateAsync(
		[FromServices] ICommandService<PlayerCreateCommand> service,
		[FromBody] PlayerCreateCommand command)
		=> service.ExecuteAsync(command, HttpContext.RequestAborted);

	[HttpPost("PlayerBalance")]
	public ValueTask<PlayerBalanceResult> PlayerBalanceAsync(
		[FromServices] IGetService<PlayerBalanceQuery, PlayerBalanceResult> service,
		[FromBody] PlayerBalanceQuery query)
		=> service.GetAsync(query, HttpContext.RequestAborted);

	[HttpPost("GameUrl")]
	public ValueTask<string> GameUrlAsync(
		[FromServices] IGetService<GameUrlQuery, string> service,
		[FromBody] GameUrlQuery query)
		=> service.GetAsync(query, HttpContext.RequestAborted);

	[HttpPost("FundOut")]
	public ValueTask<FundResult> FundOutAsync(
		[FromServices] IGetService<FundOutQuery, FundResult> service,
		[FromBody] FundOutQuery query)
		=> service.GetAsync(query, HttpContext.RequestAborted);

	[HttpPost("FundIn")]
	public ValueTask<FundResult> FundInAsync(
		[FromServices] IGetService<FundInQuery, FundResult> service,
		[FromBody] FundInQuery query)
		=> service.GetAsync(query, HttpContext.RequestAborted);

	[HttpPost("FundStatus")]
	public ValueTask<bool> FundStatusAsync(
		[FromServices] IGetService<FundStatusQuery, bool> service,
		[FromBody] FundStatusQuery query)
		=> service.GetAsync(query, HttpContext.RequestAborted);

	[HttpPost("GameRecordBetLog")]
	public IAsyncEnumerable<GameRecordResult> GameRecordBetLogAsync(
		[FromServices] IQueryService<GameRecordBetLogQuery, GameRecordResult> service,
		[FromBody] GameRecordBetLogQuery query)
		=> service.QueryAsync(query, HttpContext.RequestAborted);
}
