using Microsoft.AspNetCore.Mvc;
using NSMB_RNG;
using System;
using System.Collections.Generic;

namespace NSMB_RNG_WebApp.Controllers
{
	[ApiController]
	[Route($"{Config.ApiRotuePrefix}seeds")]
	public class SeedsController : ControllerBase
	{
		[HttpGet]
		[Route("{row1:alpha}")]
		public List<uint> Seeds(string row1)
		{
			if (string.IsNullOrEmpty(row1) || row1.Length != 7)
			{
				BadRequest("Invalid tile pattern.");
				return new List<uint>();
			}
			row1 = row1.ToUpper();

			int[] tiles = new int[7];
			try
			{
				char[] letters = new char[] { 'B', 'E', 'I', 'C', 'P', 'S' };
				for (int i = 0; i < tiles.Length; i++)
					tiles[i] = Array.IndexOf(letters, row1[i]);
			}
			catch
			{
				BadRequest("Invalid tile pattern.");
				return new List<uint>();
			}

			// How much processing should be done on server vs in browser?
			TilesFor12.SeedFinder sf = new TilesFor12.SeedFinder(tiles, Config.LookupPath);
			sf.WaitForInit();

			if (sf.error)
				throw new Exception("error");

			return sf.lookupResults;
		}
	}
}
