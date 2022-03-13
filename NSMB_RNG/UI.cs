using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSMB_RNG
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
    }
}
