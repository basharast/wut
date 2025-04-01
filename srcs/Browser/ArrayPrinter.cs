using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary
{
    public class ArrayPrinter
    {
        const string TOP_LEFT_JOINT = "┌";
        const string TOP_RIGHT_JOINT = "┐";
        const string BOTTOM_LEFT_JOINT = "└";
        const string BOTTOM_RIGHT_JOINT = "┘";
        const string TOP_JOINT = "┬";
        const string BOTTOM_JOINT = "┴";
        const string LEFT_JOINT = "├";
        const string JOINT = "┼";
        const string RIGHT_JOINT = "┤";
        const char HORIZONTAL_LINE = '─';
        const char PADDING = ' ';
        const string VERTICAL_LINE = "│";

        private static int[] GetMaxCellWidths(List<string[]> table)
        {
            int maximumCells = 0;
            foreach (Array row in table)
            {
                if (row.Length > maximumCells)
                    maximumCells = row.Length;
            }

            int[] maximumCellWidths = new int[maximumCells];
            for (int i = 0; i < maximumCellWidths.Length; i++)
                maximumCellWidths[i] = 0;

            foreach (Array row in table)
            {
                for (int i = 0; i < row.Length; i++)
                {
                    if (row.GetValue(i).ToString().Length > maximumCellWidths[i])
                        maximumCellWidths[i] = row.GetValue(i).ToString().Length;
                }
            }

            return maximumCellWidths;
        }

        public static string GetDataInTableFormat(List<string[]> table)
        {
            StringBuilder formattedTable = new StringBuilder();
            Array nextRow = table.FirstOrDefault();
            Array previousRow = table.FirstOrDefault();

            if (table == null || nextRow == null)
                return String.Empty;

            // FIRST LINE:
            int[] maximumCellWidths = GetMaxCellWidths(table);
            for (int i = 0; i < nextRow.Length; i++)
            {
                if (i == 0 && i == nextRow.Length - 1)
                    formattedTable.Append(String.Format("{0}{1}{2}", TOP_LEFT_JOINT, String.Empty.PadLeft(maximumCellWidths[i], HORIZONTAL_LINE), TOP_RIGHT_JOINT));
                else if (i == 0)
                    formattedTable.Append(String.Format("{0}{1}", TOP_LEFT_JOINT, String.Empty.PadLeft(maximumCellWidths[i], HORIZONTAL_LINE)));
                else if (i == nextRow.Length - 1)
                    formattedTable.AppendLine(String.Format("{0}{1}{2}", TOP_JOINT, String.Empty.PadLeft(maximumCellWidths[i], HORIZONTAL_LINE), TOP_RIGHT_JOINT));
                else
                    formattedTable.Append(String.Format("{0}{1}", TOP_JOINT, String.Empty.PadLeft(maximumCellWidths[i], HORIZONTAL_LINE)));
            }

            int rowIndex = 0;
            int lastRowIndex = table.Count - 1;
            foreach (Array thisRow in table)
            {
                // LINE WITH VALUES:
                int cellIndex = 0;
                int lastCellIndex = thisRow.Length - 1;
                foreach (object thisCell in thisRow)
                {
                    string thisValue = thisCell.ToString().PadLeft(maximumCellWidths[cellIndex], PADDING);

                    if (cellIndex == 0 && cellIndex == lastCellIndex)
                        formattedTable.AppendLine(String.Format("{0}{1}{2}", VERTICAL_LINE, thisValue, VERTICAL_LINE));
                    else if (cellIndex == 0)
                        formattedTable.Append(String.Format("{0}{1}", VERTICAL_LINE, thisValue));
                    else if (cellIndex == lastCellIndex)
                        formattedTable.AppendLine(String.Format("{0}{1}{2}", VERTICAL_LINE, thisValue, VERTICAL_LINE));
                    else
                        formattedTable.Append(String.Format("{0}{1}", VERTICAL_LINE, thisValue));

                    cellIndex++;
                }

                previousRow = thisRow;

                // SEPARATING LINE:
                if (rowIndex != lastRowIndex)
                {
                    nextRow = table[rowIndex + 1];

                    int maximumCells = Math.Max(previousRow.Length, nextRow.Length);
                    for (int i = 0; i < maximumCells; i++)
                    {
                        if (i == 0 && i == maximumCells - 1)
                        {
                            formattedTable.Append(String.Format("{0}{1}{2}", LEFT_JOINT, String.Empty.PadLeft(maximumCellWidths[i], HORIZONTAL_LINE), RIGHT_JOINT));
                        }
                        else if (i == 0)
                        {
                            formattedTable.Append(String.Format("{0}{1}", LEFT_JOINT, String.Empty.PadLeft(maximumCellWidths[i], HORIZONTAL_LINE)));
                        }
                        else if (i == maximumCells - 1)
                        {
                            if (i > previousRow.Length)
                                formattedTable.AppendLine(String.Format("{0}{1}{2}", TOP_JOINT, String.Empty.PadLeft(maximumCellWidths[i], HORIZONTAL_LINE), TOP_RIGHT_JOINT));
                            else if (i > nextRow.Length)
                                formattedTable.AppendLine(String.Format("{0}{1}{2}", BOTTOM_JOINT, String.Empty.PadLeft(maximumCellWidths[i], HORIZONTAL_LINE), BOTTOM_RIGHT_JOINT));
                            else if (i > previousRow.Length - 1)
                                formattedTable.AppendLine(String.Format("{0}{1}{2}", JOINT, String.Empty.PadLeft(maximumCellWidths[i], HORIZONTAL_LINE), TOP_RIGHT_JOINT));
                            else if (i > nextRow.Length - 1)
                                formattedTable.AppendLine(String.Format("{0}{1}{2}", JOINT, String.Empty.PadLeft(maximumCellWidths[i], HORIZONTAL_LINE), BOTTOM_RIGHT_JOINT));
                            else
                                formattedTable.AppendLine(String.Format("{0}{1}{2}", JOINT, String.Empty.PadLeft(maximumCellWidths[i], HORIZONTAL_LINE), RIGHT_JOINT));
                        }
                        else
                        {
                            if (i > previousRow.Length)
                                formattedTable.Append(String.Format("{0}{1}", TOP_JOINT, String.Empty.PadLeft(maximumCellWidths[i], HORIZONTAL_LINE)));
                            else if (i > nextRow.Length)
                                formattedTable.Append(String.Format("{0}{1}", BOTTOM_JOINT, String.Empty.PadLeft(maximumCellWidths[i], HORIZONTAL_LINE)));
                            else
                                formattedTable.Append(String.Format("{0}{1}", JOINT, String.Empty.PadLeft(maximumCellWidths[i], HORIZONTAL_LINE)));
                        }
                    }
                }

                rowIndex++;
            }

            // LAST LINE:
            for (int i = 0; i < previousRow.Length; i++)
            {
                if (i == 0)
                    formattedTable.Append(String.Format("{0}{1}", BOTTOM_LEFT_JOINT, String.Empty.PadLeft(maximumCellWidths[i], HORIZONTAL_LINE)));
                else if (i == previousRow.Length - 1)
                    formattedTable.AppendLine(String.Format("{0}{1}{2}", BOTTOM_JOINT, String.Empty.PadLeft(maximumCellWidths[i], HORIZONTAL_LINE), BOTTOM_RIGHT_JOINT));
                else
                    formattedTable.Append(String.Format("{0}{1}", BOTTOM_JOINT, String.Empty.PadLeft(maximumCellWidths[i], HORIZONTAL_LINE)));
            }

            return formattedTable.ToString();
        }
    }
    public interface ITextRow
    {
        String Output();
        void Output(StringBuilder sb);
        Object Tag { get; set; }
    }

    public class TableBuilder : IEnumerable<ITextRow>
    {
        protected class TextRow : List<String>, ITextRow
        {
            protected TableBuilder owner = null;
            public TextRow(TableBuilder Owner)
            {
                owner = Owner;
                if (owner == null) throw new ArgumentException("Owner");
            }
            public String Output()
            {
                StringBuilder sb = new StringBuilder();
                Output(sb);
                return sb.ToString();
            }
            public void Output(StringBuilder sb)
            {
                sb.AppendFormat(owner.FormatString, this.ToArray());
            }
            public Object Tag { get; set; }
        }

        public String Separator { get; set; }

        protected List<ITextRow> rows = new List<ITextRow>();
        protected List<int> colLength = new List<int>();

        public TableBuilder()
        {
            Separator = "  ";
        }

        public TableBuilder(String separator)
            : this()
        {
            Separator = separator;
        }

        public ITextRow AddRow(params object[] cols)
        {
            TextRow row = new TextRow(this);
            foreach (object o in cols)
            {
                String str = o.ToString().Trim();
                row.Add(str);
                if (colLength.Count >= row.Count)
                {
                    int curLength = colLength[row.Count - 1];
                    if (str.Length > curLength) colLength[row.Count - 1] = str.Length;
                }
                else
                {
                    colLength.Add(str.Length);
                }
            }
            rows.Add(row);
            return row;
        }

        protected String _fmtString = null;
        public String FormatString
        {
            get
            {
                if (_fmtString == null)
                {
                    String format = "";
                    int i = 0;
                    foreach (int len in colLength)
                    {
                        format += String.Format("{{{0},-{1}}}{2}", i++, len, Separator);
                    }
                    format += "\r\n";
                    _fmtString = format;
                }
                return _fmtString;
            }
        }

        public String Output()
        {
            StringBuilder sb = new StringBuilder();
            foreach (TextRow row in rows)
            {
                row.Output(sb);
            }
            return sb.ToString();
        }

        #region IEnumerable Members

        public IEnumerator<ITextRow> GetEnumerator()
        {
            return rows.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return rows.GetEnumerator();
        }

        #endregion
    }

}
