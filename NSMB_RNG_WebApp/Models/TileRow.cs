using System;

namespace NSMB_RNG_WebApp.Models
{
	public static class TileRow
	{
		static char[] letters = new char[] { 'B', 'E', 'I', 'C', 'P', 'S' };

		public static bool Validate(string row, int length)
		{
			if (row.Length != length)
				return false;
			foreach (char c in row.ToUpper())
			{
				if (Array.IndexOf(letters, c) == -1)
					return false;
			}

			return true;
		}
	}
}
