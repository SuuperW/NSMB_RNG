using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor.
// We disable this warning because these classes should only be constructed by Newtonsoft JSON serialization.
namespace NSMB_RNG_WebApp.Models
{
	public class RngParams
	{
		[JsonProperty(Required = Required.Always, NamingStrategyType = typeof(CamelCaseNamingStrategy))]
		public ushort Timer0 { get; set; }

		[JsonProperty(Required = Required.Always, NamingStrategyType = typeof(CamelCaseNamingStrategy))]
		public ushort VCount { get; set; }

		[JsonProperty(Required = Required.Always, NamingStrategyType = typeof(CamelCaseNamingStrategy))]
		public ushort VFrame { get; set; }

		[JsonConstructor]
		private RngParams() { }
	}

	public class RngParamsResult
    {
		[JsonProperty(Required = Required.Always, NamingStrategyType = typeof(CamelCaseNamingStrategy))]
		public RngParams[] FoundParams { get; set; }

		[JsonProperty(Required = Required.Always, NamingStrategyType = typeof(CamelCaseNamingStrategy))]
		public DateTime Datetime { get; set; }

		[JsonProperty(Required = Required.Always, NamingStrategyType = typeof(CamelCaseNamingStrategy))]
		public string Row1 { get; set; }

		[JsonProperty(Required = Required.Always, NamingStrategyType = typeof(CamelCaseNamingStrategy))]
		public string Row2 { get; set; }

		[JsonProperty(Required = Required.Always, NamingStrategyType = typeof(CamelCaseNamingStrategy))]
		public string GameVersion { get; set; }

		[JsonProperty(Required = Required.Always, NamingStrategyType = typeof(CamelCaseNamingStrategy))]
		public int Count { get; set; }

		[JsonProperty(Required = Required.Always, NamingStrategyType = typeof(CamelCaseNamingStrategy))]
		public int OffsetUsed { get; set; }

		[JsonProperty(Required = Required.Always, NamingStrategyType = typeof(CamelCaseNamingStrategy))]
		public string Mac { get; set; }

		[JsonProperty(Required = Required.Always, NamingStrategyType = typeof(CamelCaseNamingStrategy))]
		public bool Is3DS { get; set; }

		[JsonConstructor]
		private RngParamsResult() { }
	}
}
