namespace NameSplitter
{
    partial class ResearchRibbon : Microsoft.Office.Tools.Ribbon.RibbonBase
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        public ResearchRibbon()
            : base(Globals.Factory.GetRibbonFactory())
        {
            InitializeComponent();
        }

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            Microsoft.Office.Tools.Ribbon.RibbonDropDownItem ribbonDropDownItemImpl1 = this.Factory.CreateRibbonDropDownItem();
            Microsoft.Office.Tools.Ribbon.RibbonDropDownItem ribbonDropDownItemImpl2 = this.Factory.CreateRibbonDropDownItem();
            this.researchTab = this.Factory.CreateRibbonTab();
            this.nameSplitterGroup = this.Factory.CreateRibbonGroup();
            this.nameOrderComboBox = this.Factory.CreateRibbonComboBox();
            this.middleNameCheckBox = this.Factory.CreateRibbonCheckBox();
            this.credentialsCheckBox = this.Factory.CreateRibbonCheckBox();
            this.nameSplitterSeparator = this.Factory.CreateRibbonSeparator();
            this.credentialsStringCheckBox = this.Factory.CreateRibbonCheckBox();
            this.credentialsColumnsCheckBox = this.Factory.CreateRibbonCheckBox();
            this.researchTab.SuspendLayout();
            this.nameSplitterGroup.SuspendLayout();
            // 
            // researchTab
            // 
            this.researchTab.Groups.Add(this.nameSplitterGroup);
            this.researchTab.Label = "RESEARCH";
            this.researchTab.Name = "researchTab";
            // 
            // nameSplitterGroup
            // 
            this.nameSplitterGroup.Items.Add(this.nameOrderComboBox);
            this.nameSplitterGroup.Items.Add(this.middleNameCheckBox);
            this.nameSplitterGroup.Items.Add(this.credentialsCheckBox);
            this.nameSplitterGroup.Items.Add(this.nameSplitterSeparator);
            this.nameSplitterGroup.Items.Add(this.credentialsStringCheckBox);
            this.nameSplitterGroup.Items.Add(this.credentialsColumnsCheckBox);
            this.nameSplitterGroup.Label = "NameSplitter";
            this.nameSplitterGroup.Name = "nameSplitterGroup";
            // 
            // nameOrderComboBox
            // 
            ribbonDropDownItemImpl1.Label = "First/Last";
            ribbonDropDownItemImpl1.Tag = "first";
            ribbonDropDownItemImpl2.Label = "Last/First";
            ribbonDropDownItemImpl2.Tag = "last";
            this.nameOrderComboBox.Items.Add(ribbonDropDownItemImpl1);
            this.nameOrderComboBox.Items.Add(ribbonDropDownItemImpl2);
            this.nameOrderComboBox.Label = "Name Order";
            this.nameOrderComboBox.Name = "nameOrderComboBox";
            this.nameOrderComboBox.Text = null;
            this.nameOrderComboBox.TextChanged += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.nameOrderComboBox_TextChanged);
            // 
            // middleNameCheckBox
            // 
            this.middleNameCheckBox.Label = "Middle Name";
            this.middleNameCheckBox.Name = "middleNameCheckBox";
            this.middleNameCheckBox.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.middleNameCheckBox_Click);
            // 
            // credentialsCheckBox
            // 
            this.credentialsCheckBox.Label = "Credentials";
            this.credentialsCheckBox.Name = "credentialsCheckBox";
            this.credentialsCheckBox.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.credentialsCheckBox_Click);
            // 
            // nameSplitterSeparator
            // 
            this.nameSplitterSeparator.Name = "nameSplitterSeparator";
            // 
            // credentialsStringCheckBox
            // 
            this.credentialsStringCheckBox.Label = "Credentials String";
            this.credentialsStringCheckBox.Name = "credentialsStringCheckBox";
            this.credentialsStringCheckBox.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.credentialsStringCheckBox_Click);
            // 
            // credentialsColumnsCheckBox
            // 
            this.credentialsColumnsCheckBox.Label = "Credentials Columns";
            this.credentialsColumnsCheckBox.Name = "credentialsColumnsCheckBox";
            this.credentialsColumnsCheckBox.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.credentialsColumnsCheckBox_Click);
            // 
            // ResearchRibbon
            // 
            this.Name = "ResearchRibbon";
            this.RibbonType = "Microsoft.Excel.Workbook";
            this.Tabs.Add(this.researchTab);
            this.Load += new Microsoft.Office.Tools.Ribbon.RibbonUIEventHandler(this.ResearchRibbon_Load);
            this.researchTab.ResumeLayout(false);
            this.researchTab.PerformLayout();
            this.nameSplitterGroup.ResumeLayout(false);
            this.nameSplitterGroup.PerformLayout();

        }

        #endregion

        internal Microsoft.Office.Tools.Ribbon.RibbonTab researchTab;
        internal Microsoft.Office.Tools.Ribbon.RibbonGroup nameSplitterGroup;
        internal Microsoft.Office.Tools.Ribbon.RibbonCheckBox middleNameCheckBox;
        internal Microsoft.Office.Tools.Ribbon.RibbonCheckBox credentialsCheckBox;
        internal Microsoft.Office.Tools.Ribbon.RibbonSeparator nameSplitterSeparator;
        internal Microsoft.Office.Tools.Ribbon.RibbonCheckBox credentialsStringCheckBox;
        internal Microsoft.Office.Tools.Ribbon.RibbonCheckBox credentialsColumnsCheckBox;
        internal Microsoft.Office.Tools.Ribbon.RibbonComboBox nameOrderComboBox;
    }

    partial class ThisRibbonCollection
    {
        internal ResearchRibbon ResearchRibbon
        {
            get { return this.GetRibbon<ResearchRibbon>(); }
        }
    }
}
