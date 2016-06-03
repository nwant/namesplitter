using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NameSplitter
{
    public class  NameWithCredentials : Name  
    {
        private List<Credential> credentials;

        public NameWithCredentials()
            : this("") { }

        public NameWithCredentials(string fullName) 
            : base(fullName)
        {
            credentials = new List<Credential>();
        }

        /// <summary>
        /// Copy Constructor
        /// </summary>
        /// <param name="nameWithCredentials"></param>
        public NameWithCredentials(NameWithCredentials nameWithCredentials)
        {
            this.Prefix = nameWithCredentials.Prefix;
            this.FullName = nameWithCredentials.FullName;
            this.First = nameWithCredentials.First;
            this.Middle = nameWithCredentials.Middle;
            this.Last = nameWithCredentials.Last;
            this.NickName = nameWithCredentials.NickName;
            this.Suffix = nameWithCredentials.Suffix;
            this.AddCredentialTypes(nameWithCredentials.GetCredentialTypes());
        }

        public void AddCredential(Credential credential)
        {
            credentials.Add(credential);
        }

        public void AddCredentialType(CredentialType type)
        {
            Credential newCredential = new Credential(type);
            if(!credentials.Contains(newCredential))
                credentials.Add(newCredential);
        }

        public void AddCredentialTypes(CredentialType[] types)
        {
            foreach (CredentialType type in types)
                AddCredentialType(type);
        }

        public void RemoveCredentialType(CredentialType type)
        {
            Credential credToRemove = new Credential(type);
            if(credentials.Contains(credToRemove))
                credentials.Remove(credToRemove);
        }

        public void RemoveAllCredentials()
        {
            credentials.Clear();
        }

        public CredentialType[] GetCredentialTypes()
        {
            List<CredentialType> allCredentialTypes = new List<CredentialType>();
            foreach (Credential credential in credentials)
                allCredentialTypes.Add(credential.Type);

            allCredentialTypes.Sort();

            return allCredentialTypes.ToArray();
        }

        public string[] GetCredentialTypeNames()
        {
            List<string> names = new List<string>();
            foreach (Credential credential in credentials)
                names.Add(credential.GetName());
            return names.ToArray();
        }

        public string GetCredentialString()
        {
            string[] credentialNames = GetCredentialTypeNames();
            return String.Join("; ", credentialNames);
        }

        public Name ToName()
        {
            Name returnName = new Name(FullName);
            returnName.First = First ?? String.Empty;
            returnName.Middle = Middle ?? String.Empty;
            returnName.Last = Last ?? String.Empty;
            returnName.Suffix = Suffix ?? String.Empty;
            returnName.Prefix = Prefix ?? String.Empty;
            returnName.NickName = NickName ?? String.Empty;
            return returnName;
        }

        public override string ToString()
        {
            return FullName + ", " + GetCredentialString();
        }

        public override bool Equals(object obj)
        {
            if (obj.GetType() == typeof(NameWithCredentials))
            {
                NameWithCredentials n = (NameWithCredentials)obj;
                if (String.Compare(n.First, First) == 0 && String.Compare(n.FullName, FullName) == 0 && String.Compare(n.Middle, Middle) == 0 && String.Compare(n.Last, Last) == 0 && String.Compare(n.Suffix, Suffix) == 0 && String.Compare(n.Prefix, Prefix) == 0 && String.Compare(n.NickName, NickName) == 0 && n.GetCredentialTypes().SequenceEqual(GetCredentialTypes()) )
                    return true;
            }

            return false;
        }

    }
}
