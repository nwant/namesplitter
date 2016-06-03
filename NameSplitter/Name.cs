using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NameSplitter
{
    public class Name
    {
        private Prefix prefix;

        public string Prefix 
        {
            get { return prefix.Value; }
            set { prefix = new Prefix(value); }
        }

        private Suffix suffix;

        public string Suffix 
        {
            get { return suffix.Value; }
            set { suffix = new Suffix(value); }
        }

        private StringBuilder firstName;
        private StringBuilder middleName;
        private StringBuilder lastName;
        private StringBuilder fullName;
        private StringBuilder nickName;

        /// <summary>
        /// A first name.
        /// </summary>
        public string First { get { return firstName.ToString(); } set { firstName.Clear(); firstName.Append(value); } }

        /// <summary>
        /// The middle name.
        /// </summary>
        public string Middle { get { return middleName.ToString(); } set { middleName.Clear(); middleName.Append(value); } }

        /// <summary>
        /// The last name.
        /// </summary>
        public string Last { get { return lastName.ToString(); } set { lastName.Clear(); lastName.Append(value); } }

        /// <summary>
        /// The holder for the raw name string. (e.g "John Smith, MD, PhD")
        /// </summary>
        public string FullName { get { return fullName.ToString(); } set { fullName.Clear(); fullName.Append(value); } }

        /// <summary>
        /// The nickname
        /// </summary>
        public string NickName { get { return nickName.ToString(); } set { nickName.Clear(); nickName.Append(value); } }

        /// <summary>
        /// Constructs a Name object where every property is an empty string
        /// </summary>
        public Name() :
            this("") { }

        /// <summary>
        /// Constructs a Name object in which the provided full name string is set.
        /// 
        /// </summary>
        /// <param name="fullName"></param>
        public Name(string fullName)
        {
            Prefix = "";
            firstName = new StringBuilder("");
            middleName = new StringBuilder("");
            lastName = new StringBuilder("");
            Suffix = "";
            nickName = new StringBuilder("");
            this.fullName = new StringBuilder(fullName); 
        }

        /// <summary>
        /// Copy Constructor
        /// </summary>
        /// <param name="name"></param>
        public Name(Name name)
        {
            this.Prefix = name.Prefix;
            this.Suffix = name.Suffix;
            fullName = new StringBuilder(name.FullName);
            firstName = new StringBuilder(name.First);
            middleName = new StringBuilder(name.Middle);
            lastName = new StringBuilder(name.Last);
            nickName = new StringBuilder(name.NickName);
        }

        public NameWithCredentials ToNameWithCredentials()
        {
            NameWithCredentials returnNameWithCredentials = new NameWithCredentials(FullName);
            returnNameWithCredentials.First = First ?? String.Empty;
            returnNameWithCredentials.Middle = Middle ?? String.Empty;
            returnNameWithCredentials.Last = Last ?? String.Empty;
            returnNameWithCredentials.Suffix = Suffix ?? String.Empty;
            returnNameWithCredentials.Prefix = Prefix ?? String.Empty;
            returnNameWithCredentials.NickName = NickName ?? String.Empty;
            return returnNameWithCredentials;
        }

        public override string ToString()
        {
            return FullName;
        }

        public override bool Equals(object obj)
        {
            if (obj.GetType() == typeof(Name) && obj.GetType() != typeof(NameWithCredentials))
            {
                Name n = (Name)obj;
                if (String.Compare(n.First, First) == 0 && String.Compare(n.FullName, FullName) == 0 && String.Compare(n.Middle, Middle) == 0 && String.Compare(n.Last, Last) == 0 && String.Compare(n.Suffix, Suffix) == 0 && String.Compare(n.Prefix, Prefix) == 0 && String.Compare(n.NickName, NickName) == 0)
                    return true;
            }

            return false;
        }
    }
}
