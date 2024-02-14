using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor.
// We disable this warning because these classes should only be constructed by Newtonsoft JSON serialization.
namespace NSMB_RNG_WebApp.Models
{
	public class ResultsSession
	{
		private string _mac;
		[JsonProperty(Required = Required.Always, NamingStrategyType = typeof(CamelCaseNamingStrategy))]
		public string Mac
		{
			get => _mac;
			set
			{
				if (value.Length != 12 && value.Length != 17)
					throw new Exception("Invalid MAC address received.");
				_mac = value.Length == 12 ? value : value.Replace(value.Substring(2, 1), "");
				if (!long.TryParse(_mac, System.Globalization.NumberStyles.AllowHexSpecifier, null, out long _))
					throw new Exception("Invalid MAC address received.");
				_mac = _mac.ToLower();
			}
		}

		[JsonProperty(Required = Required.Always, NamingStrategyType = typeof(CamelCaseNamingStrategy))]
		public bool Is3DS { get; set; }

		[JsonProperty(Required = Required.Always, NamingStrategyType = typeof(CamelCaseNamingStrategy))]
		public string GameVersion { get; set; }

		[JsonProperty(Required = Required.Always, NamingStrategyType = typeof(CamelCaseNamingStrategy))]
		public DateTime Datetime { get; set; }

		[JsonProperty(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
		public string Id { get; set; }

		[JsonConstructor]
		public ResultsSession()
		{
			Id = Guid.NewGuid().ToString();
		}
	}
}
