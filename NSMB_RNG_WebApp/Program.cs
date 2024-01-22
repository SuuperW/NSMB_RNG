using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;

namespace NSMB_RNG_WebApp
{
	internal static class Config
	{
		public const string ApiRotuePrefix = "/asp/";
	}

	public class Program
	{
		public static void Main(string[] args)
		{
			// Set environment variables based on config file.
			var jsonConfig = System.Text.Json.JsonDocument.Parse(File.ReadAllText("config.json")).RootElement;
			foreach (var prop in jsonConfig.EnumerateObject())
			{
				string val = prop.Value.GetString() ?? "";
				Environment.SetEnvironmentVariable(prop.Name, val);
			}
			string? lookupPath = Environment.GetEnvironmentVariable("lookupPath");
			if (!string.IsNullOrEmpty(lookupPath) && !Directory.Exists(lookupPath))
				throw new DirectoryNotFoundException($"Cannot find lookup directory specified in config.json: {lookupPath}");
			string? config7z = Environment.GetEnvironmentVariable("7z_PATH");
			if (!string.IsNullOrEmpty(config7z) && !File.Exists(config7z))
				throw new FileNotFoundException($"Cannot find 7z file specified in config.json: {config7z}");


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

			// While running in dev mode, we access the app through a proxy which hosts the SPA.
			// In production, we access the ASP.NET app directly. Thus, ASP.NET needs to serve the SPA's files.
			app.UseStaticFiles(); // defaults to serving from wwwroot
			// In order to make the SPAs routing work, we fall back to index.html for URLs unknown to ASP.
			// The SPA will then look at the requested URL and apply its own routing.
			IFileProvider? staticFileProvider = null;
			// In dev mode SPA files will be in ClientApp/dist. In release/prod they should be copied into wwwroot.
			if (app.Environment.IsDevelopment())
			{
				string devDistDir = Path.Combine(builder.Environment.ContentRootPath, "ClientApp/dist");
				Directory.CreateDirectory(devDistDir); // It may not yet exist. Angular app is build after ASP app starts.
				staticFileProvider = new PhysicalFileProvider(devDistDir);
			}
			StaticFileOptions sfOptions = new StaticFileOptions() { FileProvider = staticFileProvider };
			app.MapFallbackToFile("index.html", sfOptions);

			// This is needed for attribute routes to actually be used.
			app.MapControllers();

			app.Run();
		}
	}
}