using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NSMB_RNG;
using NSMB_RNG_WebApp.Models;
using System;
using System.ComponentModel;
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
			await LogToCosmos(parsedResult);
			return Accepted();
		}

		private DateTime Nowish()
		{
			// Now, but only with second precision.
			DateTime now = DateTime.UtcNow;
			return now.Date + new TimeSpan(now.Hour, now.Minute, now.Second);
		}

		private void LogToFile(RngParamsResult rngParamsResult)
		{
			string fileName = "resultLog.json";
			if (System.IO.File.Exists(fileName))
				System.IO.File.AppendAllText(fileName, ",\n");

			JObject json = JObject.FromObject(rngParamsResult);
			json["submissionTime"] = Nowish();
			System.IO.File.AppendAllText(fileName, json.ToString(Formatting.None));
		}

		private async Task<Microsoft.Azure.Cosmos.Container> GetCosmosContainer()
		{
			string? DbName = Environment.GetEnvironmentVariable("CosmosDbName");
			string? ContainerName = Environment.GetEnvironmentVariable("CosmosContainerName");
			string? PrimaryKey = Environment.GetEnvironmentVariable("CosmosPrimaryKey");
			string? EndpointUri = Environment.GetEnvironmentVariable("CosmosEndpointUri");
			if (DbName == null || ContainerName == null || PrimaryKey == null || EndpointUri == null)
				throw new Exception("Unable to retrieve environment variables required for CosmosDB connection.");

			CosmosClient cosmosClient = new CosmosClient(EndpointUri, PrimaryKey);
			var dResult = await cosmosClient.CreateDatabaseIfNotExistsAsync(DbName, ThroughputProperties.CreateManualThroughput(400));
			if ((dResult.StatusCode & (System.Net.HttpStatusCode.OK | System.Net.HttpStatusCode.Created)) == 0)
				throw new Exception("Failed to conenct to CosmosDB, or to create/get database.");
			Database database = dResult.Database;

			var props = new ContainerProperties(ContainerName, "/mac");
			props.IndexingPolicy.ExcludedPaths.Add(new ExcludedPath() { Path = "/*" });
			props.IndexingPolicy.IncludedPaths.Clear();
			props.IndexingPolicy.IncludedPaths.Add(new IncludedPath() { Path = "/mac/?" });
			props.IndexingPolicy.IncludedPaths.Add(new IncludedPath() { Path = "/_ts/?" });
			var cResult = await database.CreateContainerIfNotExistsAsync(props);
			if ((cResult.StatusCode & (System.Net.HttpStatusCode.OK | System.Net.HttpStatusCode.Created)) == 0)
				throw new Exception("Failed to create/get container.");
			return cResult.Container;
		}

		private async Task LogToCosmos(RngParamsResult rngParamsResult)
		{
			Microsoft.Azure.Cosmos.Container container = await GetCosmosContainer();

			JObject item = JObject.FromObject(rngParamsResult);
			item["id"] = Guid.NewGuid().ToString(); // a required property per CosmosDB
			// We do not add a submissionTime property because CosmosDB automatically timestamps all items with the _ts property.

			var response = await container.CreateItemAsync(item);
			if (response.StatusCode != System.Net.HttpStatusCode.Created)
				throw new Exception("Failed to add item to ComsosDB.");
		}
	}
}