using Microsoft.AspNetCore.Mvc;
using NSMB_RNG;
using NSMB_RNG_WebApp.Models;
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
		public ActionResult<List<uint>> Seeds(string row1)
		{
			// Validate and parse the tile pattern
			if (string.IsNullOrEmpty(row1) || row1.Length != 7)
				return BadRequest("Invalid tile pattern.");
			int[]? tiles = TileRow.Parse(row1);
			if (tiles == null)
				return BadRequest("Invalid tile pattern.");

			// Grab the possible seeds (this method does more work than we need, but the code already exists)
			TilesFor12.SeedFinder sf = new TilesFor12.SeedFinder(tiles);
			sf.WaitForInit();
			if (sf.error)
				throw new Exception("error");

			return new ActionResult<List<uint>>(sf.lookupResults);
		}
	}
}
