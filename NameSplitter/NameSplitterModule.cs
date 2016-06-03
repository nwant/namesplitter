using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Office = Microsoft.Office.Core;
using Excel = Microsoft.Office.Interop.Excel;

namespace NameSplitter
{
    internal class NameSplitterModule : NameSplitter
    {
        private Office.CommandBarButton button;

        private ExcelColumn fullNameColumn;

        private ExcelColumn lastNameColumn;

        private ExcelColumn firstNameColumn;

        private ExcelColumn middleNameColumn;

        private ExcelColumn nickNameColumn;

        private ExcelColumn prefixColumn;

        private ExcelColumn suffixColumn;

        private ExcelColumn credentialsStringColumn;

        private List<ExcelColumn> credentialColumns;

        private bool includePrefixColumn = false;

        private bool includeSuffixColumn = false;

        private bool includeMiddleNameColumn = false;

        private bool includeNickNameColumn = false;

        public bool includeCredentialsStringColumn = false;

        public bool IncludeCredentialsColumns { get; set; }

        public bool IncludeCredentialsString { get; set; }

        public override bool ConsiderCredentials
        {
            get
            {
                return base.ConsiderCredentials;
            }
            set
            {
                IncludeCredentialsColumns = IncludeCredentialsString = base.ConsiderCredentials = value;
            }
        }

        public NameSplitterModule()
        {
            AddUIControls();
            this.LastNameFirst = false;
            this.ConsiderMiddleName = false;
            this.ConsiderCredentials = false;
            this.IncludeCredentialsColumns = false;
            this.IncludeCredentialsString = false;
            this.includePrefixColumn = false;
            this.includeSuffixColumn = false;
            this.includeNickNameColumn = false;
            this.includeMiddleNameColumn = false;
            this.includeCredentialsStringColumn = false;
            this.credentialColumns = new List<ExcelColumn>();
            this.SetNewName("");
        }

        internal void AddUIControls()
        {
            Office.CommandBar commandBar = Globals.ThisAddIn.Application.CommandBars["Column"];

            Office.CommandBarButton oldButton = commandBar.FindControl(Tag: "SPLIT_NAME") as Office.CommandBarButton;
            if (oldButton != null)
                oldButton.Delete();

            button = (Office.CommandBarButton)commandBar.Controls.Add(Office.MsoControlType.msoControlButton, Missing.Value, Missing.Value, Missing.Value, true);
            button.Caption = "Split Name";
            button.BeginGroup = true;
            button.Tag = "SPLIT_NAME";
            button.Click += new Office._CommandBarButtonEvents_ClickEventHandler(onButtonClick);

        }

        internal void SplitNameColumn()
        {
            if (!ReadNameColumnData())
            {
                MessageBox.Show("Could not read selected column.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!BuildColumns())
            {
                MessageBox.Show("Attempt to build columns failed", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!AppendNewColumns())
            {
                MessageBox.Show("Unable add new columns to worksheet.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool ReadNameColumnData()
        {
            try
            {
                fullNameColumn = ExcelHelper.GetColumn(ExcelHelper.GetSelectedColumnNumber()).ToExcelColumn();
                return true;
            }
            catch
            {
                return false;
            }
        }

        private bool BuildColumns()
        {
            try
            {
                int nameCount = fullNameColumn.GetRowCount();
                lastNameColumn = new ExcelColumn("Last Name", nameCount);  // build last name
                firstNameColumn = new ExcelColumn("First Name", nameCount); // build first name
                middleNameColumn = new ExcelColumn("Middle Name", nameCount);
                nickNameColumn = new ExcelColumn("Nickname", nameCount);
                prefixColumn = new ExcelColumn("Prefix", nameCount); ;
                suffixColumn = new ExcelColumn("Suffix", nameCount); ;
                credentialsStringColumn = new ExcelColumn("Credentials", nameCount);

                foreach (string cred in Credential.GetValidCredentialTypeNameList().ToArray())  // create a column object for every valid credential type.
                    this.credentialColumns.Add(new ExcelColumn(cred, nameCount));

                Name name;
                NameWithCredentials nwc;

                foreach (string rawName in fullNameColumn.GetData())
                {
                    base.SetNewName(rawName);
                    name = base.GetName();
                    
                    // extract any credentials
                    if (ConsiderCredentials)
                    {
                        nwc = GetName() as NameWithCredentials;
                        CredentialType[] creds = NameSplitter.SeparateNameAndFindCredentials(rawName, out name, LastNameFirst, ConsiderMiddleName);
                        base.SetNewName(name);

                        // reverse order if necessary
                        if (LastNameFirst)
                        {
                            name = NameSplitter.ReverseFullNameOrder(base.GetName().FullName);
                            nwc = base.GetName() as NameWithCredentials;
                            nwc.FullName = name.FullName;
                            nwc.Last = name.Last;

                            //split name
                            name = NameSplitter.SplitName(name.FullName, ConsiderMiddleName);

                            // if lastname was first, make sure the last name matches the last name from splitting the string.  If not, modify before setting name.
                            if (String.Compare(nwc.Last, name.Last) != 0)
                            {
                                int lastNameIndex = name.Last.IndexOf(nwc.Last);

                                if (lastNameIndex != -1)
                                {
                                    string appendToMiddle = name.Last.Substring(0, lastNameIndex + 1);
                                    name.Middle += " " + appendToMiddle;
                                    name.Last = nwc.Last;
                                }
                            }

                            nwc = name.ToNameWithCredentials();
                            if (creds != null)
                                nwc.AddCredentialTypes(creds);
                            base.SetNewName(nwc);
                        }
                        else
                        {
                            name = base.GetName();
                            name = NameSplitter.SplitName(name.FullName, ConsiderMiddleName);
                            nwc = name.ToNameWithCredentials();
                            if(creds != null)
                                nwc.AddCredentialTypes(creds);
                            base.SetNewName(nwc);
                        }

                        AddNameToColumns();

                        // add to credentials columns, if necessary.
                        if (IncludeCredentialsColumns)
                        {
                            nwc = (NameWithCredentials)GetName();

                            CredentialType[] types = nwc.GetCredentialTypes();
                            foreach (ExcelColumn column in credentialColumns)
                            {
                                if (nwc.GetCredentialTypeNames().Contains(column.GetHeader()))
                                    column.Push(column.GetHeader());
                                else
                                    column.Push("");
                            }
                        }
                    }
                    else  // if there are no credentials
                    {
                        name = base.GetName();

                        if (LastNameFirst)
                        {
                            name = NameSplitter.ReverseFullNameOrder(rawName);
                            string lastName = name.Last;

                            name = NameSplitter.SplitName(name.FullName, ConsiderMiddleName);

                            if (String.Compare(lastName, name.Last) != 0)
                            {
                                int lastNameIndex = name.Last.IndexOf(lastName);

                                if (lastNameIndex != -1)
                                {
                                    string appendToMiddle = name.Last.Substring(0, lastNameIndex + 1);
                                    name.Middle += " " + appendToMiddle;
                                    name.Last = lastName;
                                }
                            }

                            base.SetNewName(name);
                        }
                        else
                        {
                            name = base.GetName();
                            name = NameSplitter.SplitName(name.FullName, ConsiderMiddleName);
                            base.SetNewName(name);
                        }
                         
                         AddNameToColumns();
                    }
                }

                this.SetNewName("");

                return true;
            }
            catch(Exception e)
            {
                MessageBox.Show(e.Message);
                return false;
            }
        }

        private bool AppendNewColumns()
        {
            try
            {
                // order: fullname, last, first, middle, suffix, prefix, credentials string...credentials columns
                int rowNumber = ExcelHelper.GetSelectedColumnNumber();  // this is the full name column.

                ExcelHelper.InsertNewColumn(lastNameColumn.ToDynamicExcelColumn(), rowNumber++);
             
                ExcelHelper.InsertNewColumn(firstNameColumn.ToDynamicExcelColumn(), rowNumber++);
           
                if (includeMiddleNameColumn)
                    ExcelHelper.InsertNewColumn(middleNameColumn.ToDynamicExcelColumn() , rowNumber++);

                if (includeNickNameColumn)
                    ExcelHelper.InsertNewColumn(nickNameColumn.ToDynamicExcelColumn(), rowNumber++);

                if (includeSuffixColumn)
                    ExcelHelper.InsertNewColumn(suffixColumn.ToDynamicExcelColumn(), rowNumber++);

                if (includePrefixColumn)
                    ExcelHelper.InsertNewColumn(prefixColumn.ToDynamicExcelColumn(), rowNumber++); 

                if (includeCredentialsStringColumn)
                    ExcelHelper.InsertNewColumn(credentialsStringColumn.ToDynamicExcelColumn(), rowNumber++);

                if (IncludeCredentialsColumns)
                {
                    rowNumber = ExcelHelper.GetUsedColumnCount();

                    foreach (ExcelColumn column in credentialColumns)
                    {
                        string data = column.GetData().Aggregate((a, b) => a + b);

                        if (data.Length != 0)
                            ExcelHelper.InsertNewColumn(column.ToDynamicExcelColumn(), rowNumber++);
                    }
                }
                credentialColumns.Clear();
                return true;
            }
            catch
            {
                return false;
            }
        }

        private void AddNameToColumns()
        {
            Name name = GetName();

            prefixColumn.Push(name.Prefix);
            firstNameColumn.Push(name.First);
            lastNameColumn.Push(name.Last);
            nickNameColumn.Push(name.NickName);
            suffixColumn.Push(name.Suffix);

            if (ConsiderMiddleName)
            {
                middleNameColumn.Push(name.Middle);
                if (name.Middle != "")
                    includeMiddleNameColumn = true;
            }

            if (name.Suffix != "")
                includeSuffixColumn = true;

            if (name.Prefix != "")
                includePrefixColumn = true;

            if (name.NickName != "")
                includeNickNameColumn = true;

            if (ConsiderCredentials)
            {
                NameWithCredentials nwc = GetName() as NameWithCredentials;
                credentialsStringColumn.Push(nwc.GetCredentialString());

                if (nwc.GetCredentialString() != "")
                    includeCredentialsStringColumn = true;
            }            
        }

        private void onButtonClick(Office.CommandBarButton ctrl, ref bool CancelDefault)
        {
            int columnCount = ExcelHelper.GetSelectedColumnCount();

            if (columnCount > 1)
                MessageBox.Show("NameSplitter only allows the assignment of one name column.", "Illegal Operation.", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
            else
            {
                if (MessageBox.Show("Split column with NameSplitter?", "Verification Required", MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.OK)
                    SplitNameColumn();
            }
        }
    }
}
