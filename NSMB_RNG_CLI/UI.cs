using System;

namespace NSMB_RNG_CLI
{
	internal static class UI
	{
		public static bool AskYesNo()
		{
			string? input = Console.ReadLine();
			// We accept anything other than a no as a yes.
			if (string.IsNullOrEmpty(input) || !input.ToLower().StartsWith('n'))
				return true;
			else
				return false;
		}

		public static int GetUserMenuSelection(string menu, int maxOption)
		{
			while (true)
			{
				Console.Write(menu);
				string? userInput = Console.ReadLine();
				if (!int.TryParse(userInput, out int menuOption))
					Console.WriteLine("ERROR: Input was not an integer.");
				else if (menuOption > maxOption || menuOption < 0)
					Console.WriteLine("ERROR: Input is not a valid selection.");
				else
					return menuOption;
			}
		}

		public static DateTime GetDateTimeFromUser()
		{
			while (true)
			{
				Console.Write("Enter the date/time that RNG was initialized, in your local format: ");
				if (!DateTime.TryParse(Console.ReadLine(), out DateTime dt))
					Console.WriteLine("Bad date/time format.");
				else if (dt.Year < 2000 || dt.Year > 2099)
					Console.WriteLine("Year must be a valid DS year. (2000-2099)");
				else
				{
					// Show the long date/time to the user for verification.
					Console.WriteLine("You entered: " + dt.ToLongDateString() + " " + dt.ToLongTimeString());
					Console.Write("Is this correct? [y/n]: ");
					if (UI.AskYesNo())
						return dt;
				}
			}
		}


	}
}
