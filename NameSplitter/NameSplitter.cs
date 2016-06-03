using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;


namespace NameSplitter
{
    public class NameSplitter
    {
        private Name name;

        private NameWithCredentials nameWithCredentials;
        
        public virtual bool LastNameFirst { get; set; }

        public virtual bool ConsiderMiddleName { get; set; }

        public virtual bool ConsiderCredentials { get; set; }

        #region Constructors

        public NameSplitter(bool lastNameFirst = false, bool couldHaveMiddleName = false, bool couldHaveCredentials = false)
            : this("", lastNameFirst, couldHaveMiddleName, couldHaveCredentials) { }

        public NameSplitter(string fullNameString, bool lastNameFirst = false, bool couldHaveMiddleName = false, bool couldHaveCredentials = false)
            : this(new Name(fullNameString), lastNameFirst, couldHaveMiddleName, couldHaveCredentials) { }

        public NameSplitter(Name nameToSplit, bool lastNameFirst = false, bool couldHaveMiddleName = false, bool couldHaveCredentials = false)
        {
            if (nameToSplit.GetType() == typeof(Name))
            {
                this.name = nameToSplit;
                this.nameWithCredentials = new NameWithCredentials();
            }
            else
            {
                this.name = new Name();
                this.nameWithCredentials = nameToSplit.ToNameWithCredentials();
            }

            this.LastNameFirst = lastNameFirst;
            this.ConsiderMiddleName = couldHaveMiddleName;
            this.ConsiderCredentials = couldHaveCredentials;

            // build collections for credential type patterns
            CompiledRegex.BuildCredentialTypeRegex();

            SetNewName(nameToSplit);
        }

        #endregion

        protected Name GetName()
        {
            if (ConsiderCredentials)
                return nameWithCredentials;
            else
                return name;
        }

        protected void SetNewName(string fullName="")
        {
            if (ConsiderCredentials)
                SetNewName(new NameWithCredentials(fullName));
            else
                SetNewName(new Name(fullName));        
        }

        protected void SetNewName(Name name)
        {
            if (ConsiderCredentials)
           {
               if (name.GetType() == typeof(NameWithCredentials))
                   nameWithCredentials = name as NameWithCredentials;
               else
                   nameWithCredentials = name.ToNameWithCredentials();
           }else
           {
                // consider only name properties
                this.name.FullName = name.FullName ?? String.Empty;
                this.name.Suffix = name.Suffix ?? String.Empty;
                this.name.Prefix = name.Prefix ?? String.Empty;
                this.name.Middle = name.Middle ?? String.Empty;
                this.name.Last = name.Last ?? String.Empty;
                this.name.First = name.First ?? String.Empty;
            }
        }

        ///// <summary>
        ///// Makes a last-name-first string into a Name object that has a first-name-first name string and determined Last name.
        ///// </summary>
        ///// <param name="lastNameFirstNameStr">the raw last-name-first name string (w/o credentials)</param>
        ///// <returns>Name object with the reversed name string as the FullName attribute and the last name as the Last attribute.</returns>
        public static Name ReverseFullNameOrder(string lastNameFirstString)
        {
            int firstCommaLocation = lastNameFirstString.IndexOf(',');

            // if there is no comma, return unaltered
            if(firstCommaLocation == -1)
                return new Name(lastNameFirstString);
            else if (firstCommaLocation == lastNameFirstString.Length - 1)
                return new Name(lastNameFirstString.Substring(0, firstCommaLocation));

            Name orderedName = new Name(); /// name to return

            // separate last name
            orderedName.Last = lastNameFirstString.Substring(0, firstCommaLocation).Trim();
            string firstPart = lastNameFirstString.Substring(firstCommaLocation + 1).Trim(); // full name is everything but the last name.
            firstPart = firstPart.Replace(",", ""); // remove commas

        
            // look for additional suffixes in the first part
            foreach (string part in firstPart.Split(' ').Select(p=>p.Trim()).ToArray())
            {
                if (Suffix.IsValidSuffix(part, true, true) && firstPart.Split(' ').Count() > 2)
                {
                    orderedName.Suffix = part;
                    int start = firstPart.IndexOf(part);
                    firstPart = firstPart.Substring(0, start).Trim();
                }
            }

            orderedName.FullName = (firstPart + " " + orderedName.Last + " " + orderedName.Suffix).Trim();

            return orderedName;
        }

        /// <summary>
        /// Turns a raw Name string (either first/last or last/first) that may contain credentials into a NameWithCredentials Object.
        /// </summary>
        /// <param name="rawNameWithCred">The raw name string, either first/last or last/first order, that could contain credentials</param>
        /// <returns>NameWithCredentials objects will have the raw, unadulterated name string as the FullName attribute and all of the found credentials will be added to the objects.</returns>
        public static CredentialType[] SeparateNameAndFindCredentials(string rawNameWithCred, out Name name, bool lastNameIsFirst=false, bool considerMiddleName =false)
        {

            if (CompiledRegex.backAndFrontSlashPattern.IsMatch(rawNameWithCred))
                rawNameWithCred = CompiledRegex.backAndFrontSlashPattern.Replace(rawNameWithCred, " ");

            // EDIT v4.0.2
            rawNameWithCred = rawNameWithCred.Replace(".", "");
            rawNameWithCred = rawNameWithCred.Replace("(", " (");
            rawNameWithCred = rawNameWithCred.Trim();
            // END EDIT

            // determine the index of the first occurance of a credential
            int firstCredLoc = NameSplitter.FindFirstCredentialIndex(rawNameWithCred, considerMiddleName);

            // if index is -1, there are no credentials found in the string. return at this point.
            if (firstCredLoc == -1)
            {
                name = new Name(rawNameWithCred);
                name = RemoveCredResiduals(name, lastNameIsFirst);
                return null;
            }
            
            // Remove the name from the raw string.
           //  Remove residual credentials 
            name = new Name(rawNameWithCred.Substring(0, firstCredLoc));
            name = RemoveCredResiduals(name, lastNameIsFirst);
            string rawCredentialString = rawNameWithCred.Substring(firstCredLoc);
            rawCredentialString = CompiledRegex.periodsAndCommas.Replace(rawCredentialString, " ").PadLeft(' ').PadRight(' ');

            List<CredentialType> credTypeCollection = new List<CredentialType>();


            int i = 0;
            foreach (Tuple<Regex, CredentialType> pairedNeedle in CompiledRegex.credentialTypes)
            {
                if(pairedNeedle.Item1.IsMatch(rawCredentialString))
                {
                    if(!credTypeCollection.Contains(pairedNeedle.Item2))
                        credTypeCollection.Add(pairedNeedle.Item2);

                    rawCredentialString = pairedNeedle.Item1.Replace(rawCredentialString, "");
                }
            }

            credTypeCollection.Sort();
            return credTypeCollection.ToArray();
        }

        public static Name SplitName(string rawNameString, bool considerMiddle = false)
        {
            Name nameToReturn = new Name();
            nameToReturn.FullName = rawNameString;

            // remove all forbidden words
            if (CompiledRegex.wordsToRemove.IsMatch(rawNameString))
                rawNameString = CompiledRegex.wordsToRemove.Replace(rawNameString, " ").Trim();


            if (CompiledRegex.excessiveSpacesAndCommasAndSlashes.IsMatch(rawNameString))
                rawNameString = CompiledRegex.excessiveSpacesAndCommasAndSlashes.Replace(rawNameString, " ");
            if (CompiledRegex.hyphenWithOrWithoutSpaces.IsMatch(rawNameString))
                rawNameString = CompiledRegex.hyphenWithOrWithoutSpaces.Replace(rawNameString, "-");
            if (CompiledRegex.periodsWithoutSpacesAfter.IsMatch(rawNameString))
                //rawNameString = CompiledRegex.periodsWithoutSpacesAfter.Replace(rawNameString, ". "); // MODIFIED FOR v4.0.2
                rawNameString = CompiledRegex.periodsWithoutSpaceOrLettersAfter.Replace(rawNameString, ". ");
          

            // TO DO : Create Compiled Regex for looking for "ATTN" at the beginning of the name.
            
            #region Case 1 : 1 name word

            if (!rawNameString.Contains(' '))
            {
                // assume name is last name
                nameToReturn.Last = rawNameString;

                if (nameToReturn.Last.Length == 1)
                    nameToReturn.Last = nameToReturn.Last.ToUpper();
                else if (nameToReturn.Last.Length != 2)
                    nameToReturn.Last = System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(nameToReturn.Last.ToString());
                return nameToReturn;
            }

            #endregion Case 1

            // split into nameparts
            var result = rawNameString.Split(' ').Where(w => w != "").Select(w=> w.Trim());
            string[] nameWords = result.ToArray();

            #region Case-Handling and period removal
            
            for (int i = 0; i < nameWords.Length; i++)
            {
                if (!CompiledRegex.stPattern.IsMatch(nameWords[i]))
                    nameWords[i] = CompiledRegex.periodsAndCommas.Replace(nameWords[i], " ").Trim();

                // if a word contains an appostrophe, and the appostrophe is after the first character, make the first letter and the letter after the apostrophe upper and the rest lower, else make 1st character upper and preserve user-defined casing.
                if(nameWords[i].Contains('\''))
                {
                    int apostropheIndex = nameWords[i].IndexOf('\'');
                    if(apostropheIndex == 1 && nameWords[i].Length > 2)
                        nameWords[i] = char.ToUpper(nameWords[i][0]) + "'" + char.ToUpper(nameWords[i][2]) + nameWords[i].Substring(3).ToLower();
                    else
                        nameWords[i] = char.ToUpper(nameWords[i][0]) + nameWords[i].Substring(1).ToLower();
                }// if word is one character, make uppercase.  If word is two characters, make first upper and preserve user defined casing for 2nd 
                // word.  for all other cases, make first character upper, and all other characters lower.
                else if (nameWords[i].Length != 2)
                {
                    nameWords[i] = System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(nameWords[i].ToLower());

                    // exceptions 
                    if (nameWords[i].StartsWith("Mc")) 
                        nameWords[i] = "Mc" + Char.ToUpper(nameWords[i][2]) + nameWords[i].Substring(3);
                }
                else
                    nameWords[i] = char.ToUpper(nameWords[i][0]) + nameWords[i][1].ToString();

            }

            // EDIT v4.0.2
            string nickname = String.Empty;
            nameWords = FindNickname(nameWords, out nickname);
            nameToReturn.NickName = nickname;


            int numberOfWords = nameWords.Length;

            #endregion Case-Handling and period removal

            #region Case 2: 2 name words

            if (numberOfWords == 2)
            {
                nameToReturn.First = nameWords[0];
                nameToReturn.Last = nameWords[1];
                return nameToReturn;
            }

            #endregion Case 2

            #region Case 3 : 3 name words and no middle name
            else if (numberOfWords == 3 && !considerMiddle)
            {
                if (Prefix.IsValidPrefix(nameWords[0], true, true)) // determine if the first word is a prefix
                {
                    nameToReturn.Prefix = Prefix.GetValidPrefixMatch(nameWords[0], true);
                    nameToReturn.First = nameWords[1];
                    nameToReturn.Last = nameWords[2]; 
                }
                else if (CompiledRegex.laPattern.IsMatch(nameWords[0]) || nameWords[0].Length == 1 || (CompiledRegex.maryOrLuOrLeeOrJoPattern.IsMatch(nameWords[0]) && CompiledRegex.constFirstNameExt.IsMatch(nameWords[1])))// if first name is La, if the first word is an inital, or if the 2nd word is a first name extention, make 1st and 2nd word first name.  
                {
                    nameToReturn.First = nameWords[0] + " " + nameWords[1];
                    nameToReturn.Last = nameWords[2];
                }
                else if (CompiledRegex.lePattern.IsMatch(nameWords[1]) || CompiledRegex.constLastNameExtPattern.IsMatch(nameWords[1]) || nameWords[1] == "St.") // if the 2nd to last word is Le or other last name ext consider Le to be a last name ext.
                {
                    nameToReturn.First = nameWords[0];
                    nameToReturn.Last = nameWords[1] + " " + nameWords[2];
                }
                else if (Suffix.IsValidSuffix(nameWords[2], true, true)) // determine if the last word is a suffix
                {
                    nameToReturn.First = nameWords[0];
                    nameToReturn.Last = nameWords[1];
                    nameToReturn.Suffix = Suffix.GetValidSuffixMatch(nameWords[2], true);
                }
                else if (CompiledRegex.wmPattern.IsMatch(nameWords[0])) // if first word is wm, make william first name and the 2nd word the middle and nickname
                {
                    nameToReturn.First = "William";
                    nameToReturn.NickName = nameWords[1];
                    nameToReturn.Last = nameWords[2];
                }
                else
                {
                    nameToReturn.First = nameWords[0];
                    nameToReturn.Last = nameWords[1] + " " + nameWords[2];
                }

            }

            #endregion Case 3

            #region Case 4: 3 name words and middle name. 
            else if (numberOfWords == 3 && considerMiddle)
            {
                // first "if" statement updated for v4.0.2
                if (Prefix.IsValidPrefix(nameWords[0], true, true)) // determine if the first word is a prefix
                {
                    nameToReturn.Prefix = Prefix.GetValidPrefixMatch(nameWords[0], true);
                    nameToReturn.First = nameWords[1];
                    nameToReturn.Last = nameWords[2];
                }
                else if (Suffix.IsValidSuffix(nameWords[2], true, false)  ) // determine if the last word is a suffix.  EDIT v4.0.2 to for "John R Senior, MD", where Senior is last name.
                {
                    nameToReturn.First = nameWords[0];
                    nameToReturn.Last = nameWords[1];
                    nameToReturn.Suffix = Suffix.GetValidSuffixMatch(nameWords[2], true);
                }
                else if (CompiledRegex.laPattern.IsMatch(nameWords[0]) || nameWords[0].Length == 1 || (CompiledRegex.maryOrLuOrLeeOrJoPattern.IsMatch(nameWords[0]) && CompiledRegex.constFirstNameExt.IsMatch(nameWords[1]) && !Prefix.IsValidPrefix(nameWords[0], true, true)))// if first name is La, if the first word is an inital, or if the 2nd word is a first name extention AND first word is not a prefix, make 1st and 2nd word first name.  
                {
                    nameToReturn.First = nameWords[0] + " " + nameWords[1];
                    nameToReturn.Last = nameWords[2];
                }
                else if (CompiledRegex.lePattern.IsMatch(nameWords[1]) || (CompiledRegex.possibleLastNameExtPattern.IsMatch(nameWords[1])) || (CompiledRegex.constLastNameExtPattern.IsMatch(nameWords[1]) && !Prefix.IsValidPrefix(nameWords[0], true, true)) || nameWords[1] == "St.") // if the 2nd second to last word is Le or other last name ext consider Le to be a last name ext.
                {
                    nameToReturn.First = nameWords[0];
                    nameToReturn.Last = nameWords[1] + " " + nameWords[2];
                }
                else if (CompiledRegex.wmPattern.IsMatch(nameWords[0])) // if first word is wm, make william first name and the 2nd word the middle and nickname
                {
                    nameToReturn.First = "William";
                    nameToReturn.NickName = nameToReturn.Middle = nameWords[1]; 
                    nameToReturn.Last = nameWords[2];
                }
                else
                {
                    nameToReturn.First = nameWords[0];
                    nameToReturn.Middle = nameWords[1];
                    nameToReturn.Last = nameWords[2];
                }
            }
            #endregion Case 4

            #region Case 5 : 3+ name words without middle.

            else if (numberOfWords >= 3 && !considerMiddle)
            {
                int indexToMoveTo = 0;
                bool moveToLoop = false;

                // look for prefixes with spaces
                if (Prefix.IsValidPrefix(nameWords[0] + " " + nameWords[1], true, true))
                {
                    nameToReturn.Prefix = Prefix.GetValidPrefixMatch(nameWords[0] + " " + nameWords[1], true);
                    indexToMoveTo = 2;
                }
                else if (Prefix.IsValidPrefix(nameWords[0], true, true)) // look for a prefix w/o spaces.
                {
                    nameToReturn.Prefix = Prefix.GetValidPrefixMatch(nameWords[0], true);
                    indexToMoveTo = 1;
                }

                if (CompiledRegex.laPattern.IsMatch(nameWords[indexToMoveTo]) || nameWords[indexToMoveTo].Length == 1)  // if the first word is "La", or the first word is one character, the second word should be part of the first name.
                {
                    if (!moveToLoop)
                    {
                        // EDIT v4.0.2 - Replaced first line w/ second line
                        //nameToReturn.First = nameWords[indexToMoveTo] + " " + nameWords[indexToMoveTo + 1];
                        if (nameWords.Length == 3 || (nameWords.Length == 4 && Suffix.IsValidSuffix(nameWords[nameWords.Length - 1], true, true)))
                        {
                            nameToReturn.First = nameWords[indexToMoveTo];
                            nameToReturn.Last = nameWords[++indexToMoveTo];
                        }
                        else
                        {
                            nameToReturn.First = nameWords[indexToMoveTo] + " " + nameWords[++indexToMoveTo];
                        }
                        // END EDIT
                        moveToLoop = true;
                    }
                }
                else if (CompiledRegex.wmPattern.IsMatch(nameWords[indexToMoveTo]))
                {
                    if (!moveToLoop)
                    {
                        nameToReturn.First = "William";
                        nameToReturn.NickName = nameWords[indexToMoveTo + 1];
                        indexToMoveTo++;
                    }
                }
                else
                    nameToReturn.First = nameWords[indexToMoveTo];

                indexToMoveTo++;

                for (int i = indexToMoveTo; i < nameWords.Length; i++)
                {
                    if (i == (nameWords.Length - 1)) // see if last word is a suffix.
                    {
                        if (Suffix.IsValidSuffix(nameWords[i], true, true))
                            nameToReturn.Suffix = Suffix.GetValidSuffixMatch(nameWords[i], true);
                        else
                            // EDIT v4.0.2 -- Replace first line with second line.
                            //nameToReturn.Last += " " + nameWords[i];
                            nameToReturn.Last += (nameToReturn.Last == String.Empty) ? nameWords[i] : " " + nameWords[i];
                            // END EDIT
                        break;
                    }
                    else if (nameWords[i].Length == 1)
                        nameToReturn.First += " " + nameWords[i];
                    else if (CompiledRegex.maryOrLuOrLeeOrJoPattern.IsMatch(nameToReturn.First) && CompiledRegex.constFirstNameExt.IsMatch(nameWords[i]) && nameToReturn.Middle == "")
                        nameToReturn.First += " " + nameWords[i];
                    else
                    {
                        if(nameToReturn.Last == "")
                            nameToReturn.Last = nameWords[i];
                        else
                            nameToReturn.Last += " " + nameWords[i];
                    }
                }

                if (Suffix.IsValidSuffix(nameWords[numberOfWords - 1], true, true))
                    nameToReturn.Suffix = Suffix.GetValidSuffixMatch(nameWords[numberOfWords - 1], true);
            }
            #endregion Case 5

            #region Case 6: 4+ name words with middle
            else
            {
                int indexToMoveTo = 0;

                // look for prefixes with spaces
                if (Prefix.IsValidPrefix(nameWords[0] + " " + nameWords[1], true, true))
                {
                    nameToReturn.Prefix = Prefix.GetValidPrefixMatch(nameWords[0] + " " + nameWords[1], true);
                    indexToMoveTo = 2;
                }
                // look for a prefix
                else if (Prefix.IsValidPrefix(nameWords[0], true, true))
                {
                    nameToReturn.Prefix = Prefix.GetValidPrefixMatch(nameWords[0], true);
                    indexToMoveTo = 1;
                }

                bool firstNameIsInitial = false;

                // Determine first name and exeptions to first name.
                if (CompiledRegex.laPattern.IsMatch(nameWords[indexToMoveTo])) // if the first word is "La"the second word should be part of the first name.
                {
                    nameToReturn.First = nameWords[indexToMoveTo] + " " + nameWords[++indexToMoveTo]; 
                }
                else if (nameWords[indexToMoveTo].Length == 1)
                {
                    // MODIFIED FOR v4.0.2
                    if (Suffix.IsValidSuffix(nameWords[numberOfWords - 1], true, true) && (numberOfWords - 2) - indexToMoveTo == 1)
                    {
                        nameToReturn.First = nameWords[indexToMoveTo];
                        nameToReturn.Last = nameWords[++indexToMoveTo];
                    }
                    else
                    {
                        nameToReturn.First = nameWords[indexToMoveTo] + " " + nameWords[++indexToMoveTo];
                        firstNameIsInitial = true;
                    }
                }
                else if (CompiledRegex.wmPattern.IsMatch(nameWords[indexToMoveTo]))
                {
                    nameToReturn.First = "William";
                    nameToReturn.NickName = nameToReturn.Middle = nameWords[++indexToMoveTo];
                }
                    // EDIT v4.0.2
                else if (CompiledRegex.maryOrLuOrLeeOrJoPattern.IsMatch(nameWords[indexToMoveTo]) && CompiledRegex.constFirstNameExt.IsMatch(nameWords[indexToMoveTo + 1]))
                {
                    nameToReturn.First = nameWords[indexToMoveTo] + " " + nameWords[++indexToMoveTo];
                }
                // END EDIT


                else
                    nameToReturn.First = nameWords[indexToMoveTo];

                indexToMoveTo++; 
                bool addToLastName = false;
                for (int i = indexToMoveTo; i < nameWords.Length; i++)
                {
                    if (i == (nameWords.Length - 1) ) // see if last word is a suffix.
                    {
                        if (Suffix.IsValidSuffix(nameWords[i], true, true))
                            nameToReturn.Suffix = Suffix.GetValidSuffixMatch(nameWords[i], true);
                        else
                        {
                            if(nameToReturn.Last == "")
                                nameToReturn.Last = nameWords[i];
                            else
                                nameToReturn.Last += " " + nameWords[i];
                        }
                        break;
                    }

                    if (addToLastName)
                    {
                        if (nameToReturn.Last == "")
                            nameToReturn.Last = nameWords[i];
                        else
                            nameToReturn.Last += " " + nameWords[i];
                        continue;
                    }
                    
                    if (nameWords[i].Length == 1) // if any words contain only one letter, append word to middle name if first word was not an initial.  If the first word was an initial, append word to first name.
                    {
                       
                        if (firstNameIsInitial)
                        {
                            nameToReturn.First += " " + nameWords[i];
                            firstNameIsInitial = false;
                        }
                        else
                        {
                            if (nameToReturn.Middle == "")
                                nameToReturn.Middle = nameWords[i];
                            else
                                nameToReturn.Middle += " " + nameWords[i];

                            if (nameWords[i+1].Length != 1)
                                addToLastName = true;
                            else
                                addToLastName = false;
                        }
                    }
                    else if (CompiledRegex.lePattern.IsMatch(nameWords[i]) && (i == numberOfWords - 2)) // if "Le" is 2nd to last word, consider "Le" to be part of last name.
                    {
                        if (nameToReturn.Last == "")
                            nameToReturn.Last = nameWords[i];
                        else
                            nameToReturn.Last += " " + nameWords[i];
                    }
                    else if (CompiledRegex.maryOrLuOrLeeOrJoPattern.IsMatch(nameToReturn.First) && CompiledRegex.constFirstNameExt.IsMatch(nameWords[i]) && nameToReturn.Middle == "")
                    {
                        nameToReturn.First += " " + nameWords[i];
                    }
                    else if (CompiledRegex.possibleLastNameExtPattern.IsMatch(nameWords[i]))
                    {
                        if ((i == 1 && nameToReturn.Suffix == "") || (i == 2 && nameToReturn.Suffix != ""))
                            nameToReturn.Middle = nameWords[i];
                        else
                        {
                            if (nameToReturn.Last == "")
                                nameToReturn.Last = nameWords[i];
                            else
                                nameToReturn.Last += " " + nameWords[i];

                            addToLastName = true;
                        }
                    }
                    else if (CompiledRegex.constLastNameExtPattern.IsMatch(nameWords[i]))
                    {
                        if (nameToReturn.Last != "")
                        {
                            if (nameToReturn.Middle == "")
                                nameToReturn.Middle = nameToReturn.Last;
                            else
                                nameToReturn.Middle += " " + nameToReturn.Last;
                        }
                        nameToReturn.Last = nameWords[i];
                        // EDIT v4.0.2
                        addToLastName = true;
                        // END EDIT
                    }
                    else if ((i == nameWords.Length - 2) && (Suffix.IsValidSuffix(nameWords[i + 1], true, true)))
                    {
                        if (nameToReturn.Last == "")
                            nameToReturn.Last = nameWords[i];
                        else
                            nameToReturn.Last += " " + nameWords[i];
                        // EDIT v4.0.2
                        addToLastName = true;
                        // END EDIT
                    }
                    else
                    {
                        if (nameToReturn.Middle == "")
                            nameToReturn.Middle = nameWords[i];
                        else
                            nameToReturn.Middle += " " + nameWords[i];
                        // EDIT v4.0.2
                        //addToLastName = true;
                        //// END EDIT
                    }
                      
                }
            }
            #endregion Case 6

            if (nameToReturn.Prefix == "Sr")
            {
                nameToReturn.Suffix = nameToReturn.Prefix;
                nameToReturn.Prefix = "";
                nameToReturn.FullName = nameToReturn.FullName.Substring(3) + ", Sr";
            }

            return nameToReturn;
        }

        private static string [] FindNickname(string[] nameWords, out string nickname)
        {
            nickname = nameWords.
                Where(i => CompiledRegex.wordInParenthesisPattern.IsMatch(i) || CompiledRegex.wordInQuotesPattern.IsMatch(i)).
                FirstOrDefault();

            if (nickname != null)
            {
                nickname = Regex.Replace(nickname, "[)(\"]", "");

                return nameWords.
                Where(i => !CompiledRegex.wordInParenthesisPattern.IsMatch(i) && !CompiledRegex.wordInQuotesPattern.IsMatch(i)).
                ToArray();
            }
            else
            {
                nickname = String.Empty;
                return nameWords;
            }
            
        }

        /// <summary>
        /// Finds the index number where the first credential appears in the namestring
        /// </summary>
        /// <param name="rawStringWithCredentials">The raw name string that may contain credentials</param>
        /// <returns>The 0-initialized index number of where the first credential location is in raw name string.  -1 if no credentials were found in string.</returns>
        private static int FindFirstCredentialIndex(string rawStringWithCredentials, bool considerMiddleName)
        {
            int index = -1;

            // EDIT v4.0.2
            // do not consider the first two name parts if middle name is not enabled, and do not consider the first 3 name parts.
            int start = (considerMiddleName 
                        && rawStringWithCredentials.Trim().Count(c => c == ' ') >= 3 
                        && rawStringWithCredentials.Split(' ').Take(2).ToString().Replace(".", "").Length == 1 
                        && !rawStringWithCredentials.Split(' ').Take(2).ToString().Trim().EndsWith(",")) ? IndexOfNth(rawStringWithCredentials, ' ' , 3) : IndexOfNth(rawStringWithCredentials, ' ', 2);

            // if there is less than 3 words, don't consider any namewords
            if (start == -1)
                return -1;
            // 

            foreach (Tuple<Regex, CredentialType> pairedNeedle in CompiledRegex.credentialTypes)
            {
                if(pairedNeedle.Item1.IsMatch(rawStringWithCredentials))
                {
                    Match thisMatch = pairedNeedle.Item1.Match(rawStringWithCredentials);
                    if (thisMatch.Value != String.Empty && (index == -1 || thisMatch.Index < index) && thisMatch.Index >= start)  // EDIT v4.0.2  -- added "&& thisMatch.Index >= start" condition
                        index = thisMatch.Index;
                }
            }
                
            return index;
        }

        private static Name RemoveCredResiduals(Name name, bool lastNameIsFirst)
        {
            if (name.FullName.Contains(','))
            {
                string junkCredentials = "";
                int start = -1;
                if (lastNameIsFirst && name.FullName.Count(w => w == ',') > 1)
                    start = name.FullName.TakeWhile(c => ((2 - (c == ',' ? 1 : 0))) > 0).Count(); // get the index of the second occurance of a comma.
                // EDIT v4.0.2
                else if (lastNameIsFirst)
                {
                    name = new Name(name.FullName.Trim());
                    start = -1;
                }
                // END EDIT
                else
                    start = name.FullName.IndexOf(',');
                    

                if(start != -1)
                {
                    junkCredentials = name.FullName.Substring(start);
                    name = new Name(name.FullName.Substring(0, start));

                    if(name.Suffix == "")
                    {
                        foreach(string junkCred in junkCredentials.Replace(",", "").Replace(" ", "").Split(' '))
                        {
                            if(Suffix.IsValidSuffix(junkCred, true, true)) // determine the first suffix occurance.
                            {
                                name.Suffix = Suffix.GetValidSuffixMatch(junkCred, true);
                                name.FullName += ", " + name.Suffix;
                                break;
                            }
                        }
                    }
                }
            }

            return name;
        }

        private static int IndexOfNth(string str, char c, int n)
        {
            int s = -1;
            for (int i = 0; i < n; i++)
            {
                s = str.IndexOf(c, s + 1);

                if (s == -1) break;
            }

            return s;
        }
 
    }
}
