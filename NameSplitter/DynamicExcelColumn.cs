using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NameSplitter
{
    public class DynamicExcelColumn
    {
        private List<string> data;

        public string Header { get; set; }

        public DynamicExcelColumn()
            : this("",new List<string>())
        {
        }

        public DynamicExcelColumn(string header)
            : this(header, new List<string>())
        {
        }

        public DynamicExcelColumn(string header, List<string> data)
        {
            this.Header = header;
            this.data = data;
        }

        public void ClearData()
        {
            data.Clear();
        }

        public List<string> GetData()
        {
            return data;
        }


        public int RowNumberOf(string field)
        {
            if (data.Contains(field))
                return data.IndexOf(field) + 1;
            else
                return -1;
        }

        public void Push(List<string> fields)
        {
            data.AddRange(fields);
        }

        public void Push(string field)
        {
            data.Add(field); 
        }

        public string Pop()
        {
            string last = GetRowValue(data.Count);
            data.RemoveAt(data.Count - 1);
            return last;
        }

        public bool Insert(string field, int row)
        {
            if (row >= 1 && row <= data.Count)
            {
                data.Insert(row - 1, field);
                return true;
            }
            else
            {
                return false;
            }
        }

        public string GetRowValue(int rowNumber)
        {
            if (rowNumber > 1 && rowNumber < data.Count)
                return data[rowNumber - 1];
            else if (rowNumber == 1)
                return Header;
            else
                return null;
        }

        public ExcelColumn ToExcelColumn()
        {
            return new ExcelColumn(this.Header, this.data.ToArray());
        }

        public static ExcelColumn ToExcelColumn(DynamicExcelColumn column)
        {
            return new ExcelColumn(column.Header, column.GetData().ToArray());
        }
    }
}
