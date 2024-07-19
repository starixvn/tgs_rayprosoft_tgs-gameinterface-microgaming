using System.Net.Sockets;
using System.Net;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;

namespace Tgs.GameInterface.MicroGaming.Controllers;

[Route("[controller]")]
[ApiController]
public class InfoController(
	IConfiguration configuration) : ControllerBase
{
	[HttpGet]
	public async Task<object> GetAsync([FromServices] IWebHostEnvironment _hostingEnvironment)
	{
		var hostName = Dns.GetHostName();
		var hostEntry = await Dns.GetHostEntryAsync(hostName).ConfigureAwait(false);
		var hostIp = Array.Find(hostEntry.AddressList,
			x => x.AddressFamily == AddressFamily.InterNetwork);
		return new
		{
			Data = new
			{
				Environment.MachineName,
				HostName = hostName,
				HostIp = hostIp?.ToString() ?? string.Empty,
				Environment = _hostingEnvironment.EnvironmentName,
				OsVersion = $"{Environment.OSVersion}",
				Version = Assembly.GetExecutingAssembly().GetName().Version?.ToString(),
				ProcessCount = Environment.ProcessorCount,
				ReleaseNamespace = configuration.GetValue<string>("RELEASE_NAMESPACE"),
				PodIPAddress = configuration.GetValue<string>("POD_IP")
			}
		};
	}
}
