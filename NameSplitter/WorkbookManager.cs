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
    public static class WorkbookManager
    {
        public static int GetUsedRowCount()
        {
            Excel.Worksheet activeSheet = GetActiveSheet();
            return activeSheet.UsedRange.Rows.Count;
        }

        public static int GetUsedColumnCount()
        {
            Excel.Worksheet activeSheet = GetActiveSheet();
            return activeSheet.UsedRange.Columns.Count;
        }

        public static DynamicExcelColumn GetColumn(int column)
        {
            Excel.Worksheet activeSheet = GetActiveSheet();
            Excel.Range usedRange = activeSheet.UsedRange;
            usedRange.Application.ScreenUpdating = false;
            usedRange.Application.EnableEvents = false;

            // get dimensions of the column.
            string columnLetter = ExcelHelper.GetColumnLetter(column);
            string firstCoord = columnLetter + "2";
            string lastCoord = columnLetter + GetUsedRowCount().ToString();
            string headerCoord = columnLetter + "1";

            usedRange = GetActiveSheet().get_Range(firstCoord, lastCoord);
            usedRange.Application.ScreenUpdating = true;
            usedRange.Application.EnableEvents = true;

            DynamicExcelColumn xlColumn = new DynamicExcelColumn(GetActiveSheet().get_Range(headerCoord, headerCoord).Value2);

            if (usedRange.Cells.Count == 1)
                xlColumn.Push(new List<string>(new string[] { usedRange.Cells.Value2 }));
            else
            {
                Array dataArray = (Array)usedRange.Cells.Value2;
                List<string> dataList = toStringArray(dataArray).ToList();
                foreach (string field in dataList)
                    xlColumn.Push(field ?? String.Empty);
            }

            return xlColumn;

        }

        public static bool InsertNewColumn(DynamicExcelColumn column, int columnNumber)
        {
            try
            {
                int numberOfRows = GetUsedRowCount();

                int[] lowerBounds = new int[] { 1, 1 };

                int[] lengths = new int[] { numberOfRows, 1 };

                object[,] newColumnArray = (object[,])Array.CreateInstance(typeof(object), lengths, lowerBounds);

                newColumnArray[1, 1] = column.Header;

                column.Insert("", 1);
                column.Insert("", 1);

                for (int i = 2; i <= newColumnArray.GetLength(0); i++)
                    newColumnArray[i, 1] = String.Copy(column.GetData()[i]);

                // get column range
                Excel.Range range = GetColumnRange(columnNumber + 1);
                range.EntireColumn.Insert(Excel.XlInsertShiftDirection.xlShiftToRight, Excel.XlInsertFormatOrigin.xlFormatFromRightOrBelow);
                range = GetColumnRange(columnNumber + 1);

                range.Application.ScreenUpdating = false;
                range.Application.EnableEvents = false;

                range = range.get_Resize(numberOfRows, 1);
                range.set_Value(Excel.XlRangeValueDataType.xlRangeValueDefault, newColumnArray);

                /* Re-enable event handling and screen updating again.*/
                range.Application.ScreenUpdating = true;
                range.Application.EnableEvents = true;

                return true;
            }
            catch
            {
                return false;
            }
        }

        private static Excel.Worksheet GetActiveSheet()
        {
            return Globals.ThisAddIn.Application.ActiveSheet;
        }

        private static string[] toStringArray(Array values)
        {
            string[] theArray = new string[values.Length];
            for (int i = 1; i <= values.Length; i++)
            {
                if (values.GetValue(i, 1) == null)
                    theArray[i - 1] = "";
                else
                    theArray[i - 1] = (string)values.GetValue(i, 1).ToString();
            }
            return theArray;
        }

        private static Excel.Range GetColumnRange(int columnNumber)
        {
            Excel.Worksheet activeWorksheet = Globals.ThisAddIn.Application.ActiveSheet;
            string columnLetter = ExcelHelper.GetColumnLetter(columnNumber);
            return activeWorksheet.get_Range( columnLetter + ":" + columnLetter, Missing.Value);
        }
    }
}
