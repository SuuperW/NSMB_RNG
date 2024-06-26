﻿using Azure.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NSMB_RNG_WebApp.Models;
using System;
using System.IO;
using System.Threading.Tasks;

namespace NSMB_RNG_WebApp.Controllers
{
	[ApiController]
	[Route($"{Config.ApiRotuePrefix}submit")]
	public class RngParamsController : ControllerBase
	{
		[HttpPost]
		[Route("result")]
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

		[HttpPost]
		[Route("session")]
		public async Task<IActionResult> CreateSession()
		{
			using var sr = new StreamReader(Request.Body);
			string jsonStr = await sr.ReadToEndAsync();
			ResultsSession? parsedSessionInfo;
			try
			{
				parsedSessionInfo = JObject.Parse(jsonStr).ToObject<ResultsSession>();
			}
			catch (Exception ex) when (ex is JsonSerializationException or JsonReaderException)
			{
				return BadRequest();
			}

			if (parsedSessionInfo == null)
				return BadRequest();

			LogToFile(parsedSessionInfo);
			await LogToCosmos(parsedSessionInfo);
			return Ok(parsedSessionInfo.Id);
		}

		private DateTime Nowish()
		{
			// Now, but only with second precision.
			DateTime now = DateTime.UtcNow;
			return now.Date + new TimeSpan(now.Hour, now.Minute, now.Second);
		}

		private void LogToFile(RngParamsResult rngParamsResult)
		{
			string fileName = $"results/{rngParamsResult.Session}";
			Directory.CreateDirectory("results");
			if (System.IO.File.Exists(fileName))
				System.IO.File.AppendAllText(fileName, ",\n");

			JObject json = JObject.FromObject(rngParamsResult);
			json["submissionTime"] = Nowish();
			System.IO.File.AppendAllText(fileName, json.ToString(Formatting.None));
		}
		private void LogToFile(ResultsSession session)
		{
			string fileName = $"sessions/{session.Id}";
			Directory.CreateDirectory("sessions");
			JObject json = JObject.FromObject(session);
			System.IO.File.WriteAllText(fileName, json.ToString(Formatting.None));
		}

		private Container GetCosmosContainer(string name)
		{
			string? DbName = Environment.GetEnvironmentVariable("CosmosDbName");
			string? EndpointUri = Environment.GetEnvironmentVariable("CosmosEndpointUri");
			if (DbName == null || EndpointUri == null)
				throw new Exception("Unable to retrieve environment variables required for CosmosDB connection.");

			string? PrimaryKey = Environment.GetEnvironmentVariable("CosmosPrimaryKey");
			CosmosClient cosmosClient;
			if (PrimaryKey != null)
				// The option to access by primary key is provided to support dev environment.
				cosmosClient = new CosmosClient(EndpointUri, PrimaryKey);
			else
				// This should be used in production. It'll use the system-assigned managed identity.
				cosmosClient = new CosmosClient(EndpointUri, new DefaultAzureCredential());

			return cosmosClient.GetContainer(DbName, name);
		}

		private async Task LogToCosmos(RngParamsResult rngParamsResult)
		{
			Container container = GetCosmosContainer("submissions");

			JObject item = JObject.FromObject(rngParamsResult);
			item["id"] = Guid.NewGuid().ToString(); // a required property per CosmosDB
			// We do not add a submissionTime property because CosmosDB automatically timestamps all items with the _ts property.

			var response = await container.CreateItemAsync(item);
			if (response.StatusCode != System.Net.HttpStatusCode.Created)
				throw new Exception("Failed to add item to ComsosDB.");
		}
		private async Task LogToCosmos(ResultsSession session)
		{
			Container container = GetCosmosContainer("sessions");

			// item must contain id property, per ComsosDB. ResultsSession already does, so we don't need to add it here.
			JObject item = JObject.FromObject(session);

			var response = await container.CreateItemAsync(item);
			if (response.StatusCode != System.Net.HttpStatusCode.Created)
				throw new Exception("Failed to add item to ComsosDB.");
		}
	}
}