using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NameSplitter
{
    public abstract class NameExt
    {
        protected string[] validTypes;

        protected Dictionary<string, string> mapsToValidType;

        protected string value;

        public string Value
        {
            get { return value; }
            set
            {
                if (IsValidType(value))
                    this.value = value;
                else if (mapsToValidType.Keys.Contains(value))
                    this.value = mapsToValidType[value];
                else
                    this.value = String.Empty;
            }
        }

        public NameExt()
        {
            mapsToValidType = new Dictionary<string, string>();
        }

        public bool IsValidType(string strToCompare, bool ignoreCase=false, bool considerMaps=false)
        {
            Dictionary<string, string> maps;

            if (ignoreCase)
            {
                strToCompare = strToCompare.ToLower();
                maps = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            }
            else
                maps = new Dictionary<string, string>();

            foreach (KeyValuePair<string, string> map in mapsToValidType)
                maps[map.Key] = map.Value;
 
            foreach(string type in validTypes)
            {
                string thisType = type;

                if(ignoreCase)
                    thisType = thisType.ToLower();
                
                if(String.Compare(strToCompare, thisType) == 0)
                    return true;
            }

            if(considerMaps && maps.Keys.Contains(strToCompare))
                return true;
            else
                return false;
        }

        public string[] GetValidTypes()
        {
            return validTypes;
        }

        public string GetValidMatch(string strToCompare, bool considerMaps=false)
        {
            foreach (string validType in validTypes)
            {
                if (String.Compare(strToCompare, validType, true) == 0)
                    return validType;
            }

            if (considerMaps)
            {
                foreach (KeyValuePair<string, string> map in mapsToValidType)
                    if (String.Compare(strToCompare, map.Key, true) == 0)
                        return map.Value;
            }

            return String.Empty;
        }

         

    }
}
