using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NameSplitter
{
    public class ExcelColumn
    {
        private string[] data;

        private string header;

        private int lastElement;

        public ExcelColumn(string header, int dataSize)
        {
            this.header = header;
            data = new string[dataSize];
            lastElement = 0;
        }

        public ExcelColumn(string header, string[] data)
        {
            this.header = header;
            this.data = data;
            lastElement = 0;
        }

        public int GetRowCount()
        {
            return data.Length + 1;
        }

        public string[] GetData()
        {
            return data;
        }

        public string GetHeader()
        {
            return header;
        }

        public string GetDataValue(int elementNumber)
        {
            if (elementNumber > 1 && elementNumber < data.Length)
                return data[elementNumber - 1];
            else if (elementNumber == 1)
                return header;
            else
                return null;
        }

        public void SetDataValue(int elementNumber, string value)
        {
            if (elementNumber >= 0 && elementNumber < data.Length)
                data[elementNumber - 1] = value;
            
        }

        public void Push(string value)
        {
            if (lastElement != data.Length - 1)
                data[lastElement++] = value;
        }

        public string Pop()
        {
            if (lastElement != 0)
                return data[lastElement--];
            else
                return null;
        }

        public DynamicExcelColumn ToDynamicExcelColumn()
        {
            return new DynamicExcelColumn(header: this.header, data: this.data.ToList());
        }

        public static DynamicExcelColumn ToDynamicExcelColumn(ExcelColumn column)
        {
            return new DynamicExcelColumn(header: column.header, data: column.GetData().ToList());
        }
    }
}
