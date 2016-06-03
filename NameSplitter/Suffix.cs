using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NameSplitter
{
    public class Suffix: NameExt
    {
        private static readonly string[] validSuffixes =
        {
            "II",
            "III",
            "IV",
            "V",
            "VI",
            "Jr",
            "Sr",
            "Esq"
        };

        private static readonly Dictionary<string, string> mapsToValidSuffix = new Dictionary<string, string>()
        {
                {"Jnr", "Jr"},
                {"Junior", "Jr"},
                {"Snr", "Sr"},
                //{"II", "Jr"},
                {"Senior", "Sr"},
                {"Esquire", "Esq"}
        };

        public Suffix(string value)
        {
            this.validTypes = Suffix.validSuffixes;
            this.mapsToValidType = Suffix.mapsToValidSuffix;
            this.Value = value;
        }

        public static string[] GetValidSuffixes()
        {
            return Suffix.validSuffixes;
        }

        public static bool IsValidSuffix(string strToCompare, bool ignoreCase = false, bool considerMaps = false)
        {
            Suffix suffix = new Suffix("");
            return suffix.IsValidType(strToCompare, ignoreCase, considerMaps);
        }

        public static string GetValidSuffixMatch(string strToCompare, bool considerMaps=false)
        {
            Suffix suffix = new Suffix("");
            return suffix.GetValidMatch(strToCompare, considerMaps);
        }

    }
}
