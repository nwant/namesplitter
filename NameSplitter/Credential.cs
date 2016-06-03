using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace NameSplitter
{
    public class Credential
    {
        private CredentialType type;

        public CredentialType Type
        {
            get { return type; }
        }

        private static readonly Dictionary<string, CredentialType> mapsToValidCredential = new Dictionary<string, CredentialType>()
        {
            {"ANP-BC", CredentialType.ANP},
            {"ANP-C", CredentialType.ANP},
            {"APRN-BC", CredentialType.APRN},
            {"BSC", CredentialType.BS},
            {"BSc", CredentialType.BS},
            {"CRNP", CredentialType.CNP},
            {"Ed.D", CredentialType.EdD},
            {"MB BCh", CredentialType.MD},
            {"MB, BCh", CredentialType.MD},
            {"MBBCh", CredentialType.MD},
            {"MBBS", CredentialType.MD},
            {"MBchB", CredentialType.MD},
            {"MSCE", CredentialType.MS},
            {"MSHS", CredentialType.MS},
            {"MSEd", CredentialType.MS},
            {"MSEE", CredentialType.MS},
            {"MSc", CredentialType.MS},
            {"MScN", CredentialType.MS},
            {"FNP", CredentialType.NP},
            {"NP-C", CredentialType.NP},
            {"DNP", CredentialType.NP},
            {"CNP", CredentialType.NP},
            {"PA-C", CredentialType.PA},
            {"MB BS", CredentialType.MD},
            {"MB BChir", CredentialType.MD},
            {"BM BCh", CredentialType.MD},
            {"MB ChB", CredentialType.MD},
            {"BM BS", CredentialType.MD},
            {"BM", CredentialType.MD},
            {"BMed", CredentialType.MD}
        };

        #region Constructors

        public Credential(int value) : this(value.ToString()) { }

        public Credential(string name)
        {
            if (!Enum.TryParse(name, true, out this.type))
                throw new ArgumentOutOfRangeException();
        }

        public Credential(CredentialType type)
        {
            this.type = type;
        }

        #endregion

        public int GetOrderNumber()
        {
            return (int)type;
        }

        public string GetName()
        {
            return Enum.GetName(typeof(CredentialType), (int)type);
        }

        public static CredentialType[] GetValidCredentialTypes()
        {
           return Enum.GetValues(typeof(CredentialType)).Cast<CredentialType>().ToArray();
        }

        public static Dictionary<string, CredentialType> GetCredentialMaps()
        {
            return mapsToValidCredential;
        }

        public static List<string> GetValidCredentialTypeNameList(bool considerMaps=false)
        {
            List<string> ret = new List<string>();
            foreach (string type in Enum.GetNames(typeof(CredentialType)))
                ret.Add(type);

            if (considerMaps)
                ret.AddRange(mapsToValidCredential.Keys);

            return ret;
        }

        public static bool IsValidCredential(string strToCompare, bool ignoreCase = false, bool considerMaps = false)
        {
            Dictionary<string, CredentialType> maps;

            if (ignoreCase)
            {
                strToCompare = strToCompare.ToLower();
                maps = new Dictionary<string, CredentialType>(StringComparer.OrdinalIgnoreCase);
            }
            else
                maps = new Dictionary<string, CredentialType>();

            foreach (KeyValuePair<string, CredentialType> map in mapsToValidCredential)
                maps[map.Key] = map.Value;

            foreach (string cred in Enum.GetNames(typeof(CredentialType)))
            {
                string thisCred = cred;

                if (ignoreCase)
                    thisCred = cred.ToLower();

                if (String.Compare(thisCred, strToCompare) == 0)
                    return true;
            }

            if (considerMaps && maps.Keys.Contains(strToCompare))
                return true;
            else
                return false;
        }

        public static string GetName(CredentialType type)
        {
            Credential credential = new Credential(type);
            return credential.GetName();
        }
    }
}
