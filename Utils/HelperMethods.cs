using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DestinationsApp.Utils
{
    public static class HelperMethods
    {
        public static string GetFlagEmoji(string iso2)
        {
            if (string.IsNullOrEmpty(iso2) || iso2.Length != 2)
                return string.Empty;

            iso2 = iso2.ToUpper();
            int baseCodePoint = 0x1F1E6;

            char firstChar = (char)(baseCodePoint + iso2[0] - 'A');
            char secondChar = (char)(baseCodePoint + iso2[1] - 'A');

            return $"{firstChar}{secondChar}";
        }
    }
}
