using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;

namespace NSMB_RNG_WebApp
{
	internal static class Config
	{
		public const string ApiRotuePrefix = "/asp/";

		public static string LookupPath = "";
	}

	public class Program
	{
		public static void Main(string[] args)
		{
			var jsonConfig = System.Text.Json.JsonDocument.Parse(File.ReadAllText("config.json")).RootElement;
			Config.LookupPath = jsonConfig.GetProperty("lookupPath").GetString()!;
			if (!Directory.Exists(Config.LookupPath))
				throw new FileNotFoundException("Cannot find lookup directory specified in config.json.");
			if (jsonConfig.TryGetProperty("7z_PATH", out var config7z))
			{
				if (!File.Exists(config7z.GetString()))
					throw new FileNotFoundException("Cannot find 7z file specified in config.json.");
				Environment.SetEnvironmentVariable("7z_PATH", jsonConfig.GetProperty("7z_PATH").GetString());
			}

			var builder = WebApplication.CreateBuilder(args);

			// Add services to the container. (default comment)
			builder.Services.AddControllers();

			var app = builder.Build();

			// Configure the HTTP request pipeline.
			if (!app.Environment.IsDevelopment())
			{
				// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
				app.UseHsts();
			}
			app.UseHttpsRedirection();

			// This is needed for attribute routes to actually be used.
			app.MapControllers();

			app.Run();
		}
	}
}