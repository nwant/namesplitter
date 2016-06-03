using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Office.Tools.Ribbon;

namespace NameSplitter
{
    public partial class ResearchRibbon
    {
        private void ResearchRibbon_Load(object sender, RibbonUIEventArgs e)
        {
            nameOrderComboBox.Text = "First/Last";
            credentialsStringCheckBox.Checked = false;
            credentialsStringCheckBox.Enabled = false;
            credentialsCheckBox.Checked = false;
            credentialsColumnsCheckBox.Enabled = false;
        }

        private void credentialsCheckBox_Click(object sender, RibbonControlEventArgs e)
        {
            credentialsColumnsCheckBox.Enabled = credentialsColumnsCheckBox.Checked = credentialsCheckBox.Checked;
            credentialsStringCheckBox.Enabled = credentialsStringCheckBox.Checked = credentialsCheckBox.Checked;
            Globals.ThisAddIn.nameSplitterModule.ConsiderCredentials = credentialsCheckBox.Checked;
        }

        private void middleNameCheckBox_Click(object sender, RibbonControlEventArgs e)
        {
            Globals.ThisAddIn.nameSplitterModule.ConsiderMiddleName = middleNameCheckBox.Checked;
        }

        private void credentialsStringCheckBox_Click(object sender, RibbonControlEventArgs e)
        {
            Globals.ThisAddIn.nameSplitterModule.IncludeCredentialsString = credentialsStringCheckBox.Checked;
        }

        private void credentialsColumnsCheckBox_Click(object sender, RibbonControlEventArgs e)
        {
            Globals.ThisAddIn.nameSplitterModule.IncludeCredentialsColumns = credentialsColumnsCheckBox.Checked;
        }

        private void nameOrderComboBox_TextChanged(object sender, RibbonControlEventArgs e)
        {
            if (String.Compare(nameOrderComboBox.Text, "First/Last") == 0)
                Globals.ThisAddIn.nameSplitterModule.LastNameFirst = false;
            else
                Globals.ThisAddIn.nameSplitterModule.LastNameFirst = true;
        }
    }
}
