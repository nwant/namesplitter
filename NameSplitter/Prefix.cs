using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NameSplitter
{
    public class Prefix : NameExt
    {
        private static readonly string[] validPrefixes=
        {
            "1SG",
            "1LT",
            "2LT",
            "Sr",
            "ADM",
            "BG",
            "Brig Gen",
            "Brother",
            "Atty",
            "Capt",
            "CDR",
            "CMSgt",
            "Col",
            "Cpl",
            "CPT",
            "CSM",
            "CW3",
            "Dr",
            "ENS",
            "Gen",
            "HM",
            "HM1",
            "HM2",
            "HM3",
            "HMCM",
            "HMCS",
            "HMC",
            "LCDR",
            "LCpl",
            "LCpt",
            "LT",
            "LTC",
            "LTG",
            "LTJG",
            "Lt Gen",
            "Lt Col",
            "Maj",
            "Maj Gen",
            "Mg",
            "Mr",
            "Mrs",
            "Ms",
            "MSG",
            "MSgt",
            "PO3",
            "Pvt",
            "Rev",
            "Sister",
            "Sgt",
            "SFC",
            "SrA",
            "SSgt"
        };

        private static readonly Dictionary<string, string> mapsToValidPrefix = new Dictionary<string, string>()
        {
            {"Reverend", "Rev"},
            {"Mister", "Mr"},
            {"Miss", "Ms"},
            {"Sergeant", "Sgt"},
            {"Sergt", "Sgt"},
            {"Private", "Pvt"},
            {"Captain", "Capt"},
            {"Corporal", "Cpl"},
            {"Attorney", "Atty"},
            {"Br", "Brother"},
            {"Capt_AF", "Capt"},
            {"Col_AF", "Col"}
        };

        public Prefix(string value) 
        {

            this.validTypes = Prefix.validPrefixes;
            this.mapsToValidType = Prefix.mapsToValidPrefix;
            this.Value = value;
        }

        public static string[] GetValidPrefixes()
        {
            return Prefix.validPrefixes;
        }

        public static bool IsValidPrefix(string strToCompare, bool ignoreCase = false, bool considerMaps = false)
        {
            Prefix prefix = new Prefix("");
            return prefix.IsValidType(strToCompare, ignoreCase, considerMaps);
        }

        public static string GetValidPrefixMatch(string strToCompare, bool considerMaps = false)
        {
            Prefix prefix = new Prefix("");
            return prefix.GetValidMatch(strToCompare, considerMaps);
        }
    }
}
