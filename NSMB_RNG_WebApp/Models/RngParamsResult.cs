﻿using System;
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

		private string _row1;
		[JsonProperty(Required = Required.Always, NamingStrategyType = typeof(CamelCaseNamingStrategy))]
		public string Row1
		{
			get => _row1;
			set
			{
				if (!TileRow.Validate(value, 7))
					throw new Exception("Invalid row1 received.");
				_row1 = value;
			}
		}

		private string _row2;
		[JsonProperty(Required = Required.Always, NamingStrategyType = typeof(CamelCaseNamingStrategy))]
		public string Row2
		{
			get => _row2;
			set
			{
				if (!TileRow.Validate(value, 11))
					throw new Exception("Invalid row2 received.");
				_row2 = value;
			}
		}

		[JsonProperty(Required = Required.Always, NamingStrategyType = typeof(CamelCaseNamingStrategy))]
		public int OffsetUsed { get; set; }

		[JsonProperty(Required = Required.Always, NamingStrategyType = typeof(CamelCaseNamingStrategy))]
		public string Session { get; set; }

		[JsonProperty(Required = Required.Always, NamingStrategyType = typeof(CamelCaseNamingStrategy))]
		public int SubmissionId { get; set; }

		[JsonConstructor]
		private RngParamsResult() { }
	}
}
