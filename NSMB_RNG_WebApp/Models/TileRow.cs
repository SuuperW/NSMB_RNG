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

		public static int[]? Parse(string row)
		{
			row = row.ToUpper();
			int[] bytes = new int[row.Length];
			for (int i = 0; i < bytes.Length; i++)
			{
				bytes[i] = Array.IndexOf(letters, row[i]);
				if (bytes[i] == -1)
					return null;
			}
			return bytes;
		}
	}
}
