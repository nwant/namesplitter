using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Office = Microsoft.Office.Core;
using Excel = Microsoft.Office.Interop.Excel;

namespace NameSplitter
{
    public static class ExcelHelper 
    {
        private static string[] alphabet = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" };

        public static string GetColumnLetter(int columnNumber)
        {
            StringBuilder sb = new StringBuilder();

            if (columnNumber <= 26)
                sb.Append(alphabet[columnNumber - 1]);
            else
            {
                sb.Append(alphabet[(int)Math.Floor((double)columnNumber / 26) - 1]);
                sb.Append(alphabet[(columnNumber % 26) - 1]);
            }

            return sb.ToString();
        }

        public static int GetUsedRowCount()
        {
            return WorkbookManager.GetUsedRowCount();
        }

        public static int GetUsedColumnCount()
        {
            return WorkbookManager.GetUsedColumnCount();
        }

        public static DynamicExcelColumn GetColumn(int column)
        {
            return WorkbookManager.GetColumn(column);
        }

        public static bool InsertNewColumn(DynamicExcelColumn column, int columnNumber)
        {
            return WorkbookManager.InsertNewColumn(column, columnNumber);
        }

        public static int GetSelectedColumnNumber()
        {
            return SelectionManager.GetColumnNumber();
        }

        public static int GetSelectedColumnCount()
        {
            return SelectionManager.GetColumnCount();
        }

        
    }
}
