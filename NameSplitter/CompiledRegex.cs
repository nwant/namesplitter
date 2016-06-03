using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace NameSplitter
{
    public static class CompiledRegex
    {
        /// Matches "Wm", "Wm." , case insensitive
        /// </summary>
        public static readonly Regex wmPattern = new Regex(@"^wm(?=\.|$)", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        /// <summary>
        /// Matches "La", case insensitive.  The word matched with this pattern should be added to the last name if it is the second word (excluding any prefix) or added to the middle name if it is the 3rd word or later (excluding any prefix).
        /// </summary>
        public static readonly Regex laPattern = new Regex(@"^La$", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        /// <summary>
        /// Matches "Le", case insensitive.  The words matched to this should be added to the last name it is the second to last word or if the middle name is not be be considered, else append the word to the middle name.
        /// </summary>
        public static readonly Regex lePattern = new Regex(@"^Le$", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        /// <summary>
        /// Matches "St", "Saint".  Acts as a possible last name extension.  THis is a separate pattern because of the period, which must be checked separately.
        /// </summary>
        public static readonly Regex stPattern = new Regex(@"^(Saint|St\.?)$", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        public static readonly Regex maryOrLuOrLeeOrJoPattern = new Regex(@"^(Mary|Lu|Lee?|Jo)$", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        /// <summary>
        /// Constant FirstNameExts.   consider these extentions of first name if the first name matches the regex pattern above.  Matches "Jo" and "Ann"
        /// </summary>
        public static readonly Regex constFirstNameExt = new Regex(@"^(Jo|Anne?|Beth|Jane|Lou|Ellen|Kay|Kate|Lynn)$", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        /// <summary>
        /// Potential last name extensions. Matches "Al", "El", "De", "Del", "Di", "La", case insensitive.  These are considered part of the last name if there are 4+ words and the 3rd or later word (excluding the suffix) matches or the middle name is disabled.  If it is 2nd word and the middle name is disabled, the match will be considered the middle name.
        /// </summary>
        public static readonly Regex possibleLastNameExtPattern = new Regex(@"^(Al|El|Del?|Den|Di|L(a|o)|Da|Dela)$", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        /// <summary>
        /// Matches "Van", "Ver, "St.", "Vonder", "Vander", "Von" case insensitive.  These are always consered part of the last name.
        /// </summary>
        public static readonly Regex constLastNameExtPattern = new Regex(@"^(V(a|o)n(-.*?)?(de(r|n))?|St\.|Ver|Mc)$", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        public static readonly Regex wordInQuotesPattern = new Regex("^\".*\"$", RegexOptions.Compiled);

        public static readonly Regex wordInParenthesisPattern = new Regex(@"^\(.*\)$", RegexOptions.Compiled);

        /// <summary>
        /// Name words to ALWAYS remove.  Matches FACOG and FACS, "Mr/Mrs", ATTN
        /// </summary>
        public static readonly Regex wordsToRemove = new Regex(@"(^|[\s,])(Mr\.?\s?Mr?s\.?|ATTN|FACOG|FACS)(?=[\s,]|$)", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public static readonly Regex backAndFrontSlashPattern = new Regex(@"[\\/]", RegexOptions.Compiled);

        public static readonly Regex excessiveSpacesAndCommasAndSlashes = new Regex(@"[\s,\\/](?=$)", RegexOptions.Compiled);

        public static readonly Regex hyphenWithOrWithoutSpaces = new Regex(@"\s?-\s?", RegexOptions.Compiled);

        public static readonly Regex periodsWithoutSpacesAfter = new Regex(@"\.[^\s]", RegexOptions.Compiled);

        public static readonly Regex periodsWithoutSpaceOrLettersAfter = new Regex(@"\.[^\sA-Za-z]", RegexOptions.Compiled);

        public static readonly Regex periodsAndCommas = new Regex(@"[.,]", RegexOptions.Compiled);

        public static Tuple<Regex, CredentialType>[] credentialTypes;

        //public static Tuple<Regex, string>[] suffixTypes;

        public static void BuildCredentialTypeRegex()
        {
            CredentialType[] types = Credential.GetValidCredentialTypes().ToArray();
            Dictionary<string, CredentialType> maps = Credential.GetCredentialMaps();

            credentialTypes = new Tuple<Regex,CredentialType>[types.Length + maps.Count];
            int i = 0;
            foreach(CredentialType type in types)
            {
                credentialTypes[i] = new Tuple<Regex, CredentialType>(new Regex(@"(\s|,)" + Credential.GetName(type).Replace(" ", @"\s") + @"(?=[\s,]|$)", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.RightToLeft), type);
                
                // make sure mb bs doesn't convert to bs
                if (type == CredentialType.BS)
                {
                    credentialTypes[i] = new Tuple<Regex, CredentialType>(new Regex(@"(?<!MB)(\s,?)BS(?=[\s,]|$)", RegexOptions.Compiled | RegexOptions.IgnoreCase), type); 
                }
               
                i++;
            }
            
            foreach(KeyValuePair<string, CredentialType> map in maps)
            {
                credentialTypes[i] = new Tuple<Regex, CredentialType>(new Regex(@"(\s|,)" + map.Key.Replace(" ", @"\s") + @"(?=[\s,]|$)", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.RightToLeft), map.Value);
                i++;
            }
        }

        //public static void BuildSuffixTypeRegex()
        //{
        //    string[] types = Suffix.GetValidSuffixes();
        //    Dictionary<string, string> maps = Suffix.
        //}
    }
}
