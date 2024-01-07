using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NSMB_RNG_WebApp.Models;
using System;
using System.IO;
using System.Threading.Tasks;

namespace NSMB_RNG_WebApp.Controllers
{
	[ApiController]
	[Route($"{Config.ApiRotuePrefix}submitResults")]
	public class RngParamsController : ControllerBase
	{
		[HttpPost]
		public async Task<IActionResult> LogResults()
		{
			// ASP.NET does not properly handle the Required attribute on non-nullable types.
			// So, we use Newtonsoft to handle serialization.
			using var sr = new StreamReader(Request.Body);
			string jsonStr = await sr.ReadToEndAsync();
			RngParamsResult? parsedResult;
			try
			{
				parsedResult = JObject.Parse(jsonStr).ToObject<RngParamsResult>();
			}
			catch (Exception ex) when (ex is JsonSerializationException or JsonReaderException)
			{
				return BadRequest();
			}

			if (parsedResult == null)
				return BadRequest();

			LogToFile(parsedResult);
			return Accepted();
		}

		private void LogToFile(RngParamsResult rngParamsResult)
		{
			string fileName = "resultLog.json";
			if (System.IO.File.Exists(fileName))
				System.IO.File.AppendAllText(fileName, ",\n");

			System.IO.File.AppendAllText(
				fileName, 
				JObject.FromObject(rngParamsResult).ToString(Formatting.None)
			);
		}
	}
}