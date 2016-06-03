using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Office = Microsoft.Office.Core;
using Excel = Microsoft.Office.Interop.Excel;

namespace NameSplitter
{
    public static class SelectionManager
    {
        public static int GetColumnNumber()
        {
            if (GetColumnCount() == 1)
            {
                Excel.Range thisSelection = Globals.ThisAddIn.Application.Selection;
                return thisSelection.Column;
            }
            else
                return -1;
        }

        public static int GetColumnCount()
        {
            Excel.Application thisApp = Globals.ThisAddIn.Application as Excel.Application;
            Excel.Range thisSelection = thisApp.Selection as Excel.Range;

            return thisSelection.Columns.Count;
        }
        
    }
}
