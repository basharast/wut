using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharedLibrary
{
    public class Table
    {
        private const string TopLeftJoint = "┌";
        private const string TopRightJoint = "┐";
        private const string BottomLeftJoint = "└";
        private const string BottomRightJoint = "┘";
        private const string TopJoint = "┬";
        private const string BottomJoint = "┴";
        private const string LeftJoint = "├";
        private const string MiddleJoint = "┼";
        private const string RightJoint = "┤";
        private const char HorizontalLine = '─';
        private const string VerticalLine = "│";

        private string[] _headers;
        private List<string[]> _rows = new List<string[]>();

        public int Padding { get; set; } = 1;
        public bool HeaderTextAlignRight { get; set; }
        public bool RowTextAlignRight { get; set; }

        public void SetHeaders(params string[] headers)
        {
            _headers = headers;
        }

        public void AddRow(params string[] row)
        {
            _rows.Add(row);
        }

        public void ClearRows()
        {
            _rows.Clear();
        }

        private int[] GetMaxCellWidths(List<string[]> table)
        {
            var maximumColumns = 0;
            foreach (var row in table)
            {
                if (row.Length > maximumColumns)
                    maximumColumns = row.Length;
            }

            var maximumCellWidths = new int[maximumColumns];
            for (int i = 0; i < maximumCellWidths.Count(); i++)
                maximumCellWidths[i] = 0;

            var paddingCount = 0;
            if (Padding > 0)
            {
                //Padding is left and right
                paddingCount = Padding * 2;
            }

            foreach (var row in table)
            {
                for (int i = 0; i < row.Length; i++)
                {
                    var maxWidth = row[i].Length + paddingCount;

                    if (maxWidth > maximumCellWidths[i])
                        maximumCellWidths[i] = maxWidth;
                }
            }

            return maximumCellWidths;
        }

        private StringBuilder CreateTopLine(int[] maximumCellWidths, int rowColumnCount, StringBuilder formattedTable)
        {
            for (int i = 0; i < rowColumnCount; i++)
            {
                if (i == 0 && i == rowColumnCount - 1)
                    formattedTable.AppendLine(string.Format("{0}{1}{2}", TopLeftJoint, string.Empty.PadLeft(maximumCellWidths[i], HorizontalLine), TopRightJoint));
                else if (i == 0)
                    formattedTable.Append(string.Format("{0}{1}", TopLeftJoint, string.Empty.PadLeft(maximumCellWidths[i], HorizontalLine)));
                else if (i == rowColumnCount - 1)
                    formattedTable.AppendLine(string.Format("{0}{1}{2}", TopJoint, string.Empty.PadLeft(maximumCellWidths[i], HorizontalLine), TopRightJoint));
                else
                    formattedTable.Append(string.Format("{0}{1}", TopJoint, string.Empty.PadLeft(maximumCellWidths[i], HorizontalLine)));
            }

            return formattedTable;
        }

        private StringBuilder CreateBottomLine(int[] maximumCellWidths, int rowColumnCount, StringBuilder formattedTable)
        {
            for (int i = 0; i < rowColumnCount; i++)
            {
                if (i == 0 && i == rowColumnCount - 1)
                    formattedTable.AppendLine(string.Format("{0}{1}{2}", BottomLeftJoint, string.Empty.PadLeft(maximumCellWidths[i], HorizontalLine), BottomRightJoint));
                else if (i == 0)
                    formattedTable.Append(string.Format("{0}{1}", BottomLeftJoint, string.Empty.PadLeft(maximumCellWidths[i], HorizontalLine)));
                else if (i == rowColumnCount - 1)
                    formattedTable.AppendLine(string.Format("{0}{1}{2}", BottomJoint, string.Empty.PadLeft(maximumCellWidths[i], HorizontalLine), BottomRightJoint));
                else
                    formattedTable.Append(string.Format("{0}{1}", BottomJoint, string.Empty.PadLeft(maximumCellWidths[i], HorizontalLine)));
            }

            return formattedTable;
        }

        private StringBuilder CreateValueLine(int[] maximumCellWidths, string[] row, bool alignRight, StringBuilder formattedTable)
        {
            int cellIndex = 0;
            int lastCellIndex = row.Length - 1;

            var paddingString = string.Empty;
            if (Padding > 0)
                paddingString = string.Concat(Enumerable.Repeat(' ', Padding));

            foreach (var column in row)
            {
                var restWidth = maximumCellWidths[cellIndex];
                if (Padding > 0)
                    restWidth -= Padding * 2;

                var cellValue = alignRight ? column.PadLeft(restWidth, ' ') : column.PadRight(restWidth, ' ');

                if (cellIndex == 0 && cellIndex == lastCellIndex)
                    formattedTable.AppendLine(string.Format("{0}{1}{2}{3}{4}", VerticalLine, paddingString, cellValue, paddingString, VerticalLine));
                else if (cellIndex == 0)
                    formattedTable.Append(string.Format("{0}{1}{2}{3}", VerticalLine, paddingString, cellValue, paddingString));
                else if (cellIndex == lastCellIndex)
                    formattedTable.AppendLine(string.Format("{0}{1}{2}{3}{4}", VerticalLine, paddingString, cellValue, paddingString, VerticalLine));
                else
                    formattedTable.Append(string.Format("{0}{1}{2}{3}", VerticalLine, paddingString, cellValue, paddingString));

                cellIndex++;
            }

            return formattedTable;
        }

        private StringBuilder CreateSeperatorLine(int[] maximumCellWidths, int previousRowColumnCount, int rowColumnCount, StringBuilder formattedTable)
        {
            var maximumCells = Math.Max(previousRowColumnCount, rowColumnCount);

            for (int i = 0; i < maximumCells; i++)
            {
                if (i == 0 && i == maximumCells - 1)
                {
                    formattedTable.AppendLine(string.Format("{0}{1}{2}", LeftJoint, string.Empty.PadLeft(maximumCellWidths[i], HorizontalLine), RightJoint));
                }
                else if (i == 0)
                {
                    formattedTable.Append(string.Format("{0}{1}", LeftJoint, string.Empty.PadLeft(maximumCellWidths[i], HorizontalLine)));
                }
                else if (i == maximumCells - 1)
                {
                    if (i > previousRowColumnCount)
                        formattedTable.AppendLine(string.Format("{0}{1}{2}", TopJoint, string.Empty.PadLeft(maximumCellWidths[i], HorizontalLine), TopRightJoint));
                    else if (i > rowColumnCount)
                        formattedTable.AppendLine(string.Format("{0}{1}{2}", BottomJoint, string.Empty.PadLeft(maximumCellWidths[i], HorizontalLine), BottomRightJoint));
                    else if (i > previousRowColumnCount - 1)
                        formattedTable.AppendLine(string.Format("{0}{1}{2}", MiddleJoint, string.Empty.PadLeft(maximumCellWidths[i], HorizontalLine), TopRightJoint));
                    else if (i > rowColumnCount - 1)
                        formattedTable.AppendLine(string.Format("{0}{1}{2}", MiddleJoint, string.Empty.PadLeft(maximumCellWidths[i], HorizontalLine), BottomRightJoint));
                    else
                        formattedTable.AppendLine(string.Format("{0}{1}{2}", MiddleJoint, string.Empty.PadLeft(maximumCellWidths[i], HorizontalLine), RightJoint));
                }
                else
                {
                    if (i > previousRowColumnCount)
                        formattedTable.Append(string.Format("{0}{1}", TopJoint, string.Empty.PadLeft(maximumCellWidths[i], HorizontalLine)));
                    else if (i > rowColumnCount)
                        formattedTable.Append(string.Format("{0}{1}", BottomJoint, string.Empty.PadLeft(maximumCellWidths[i], HorizontalLine)));
                    else
                        formattedTable.Append(string.Format("{0}{1}", MiddleJoint, string.Empty.PadLeft(maximumCellWidths[i], HorizontalLine)));
                }
            }

            return formattedTable;
        }

        public override string ToString()
        {
            var table = new List<string[]>();

            var firstRowIsHeader = false;
            if (_headers?.Any() == true)
            {
                table.Add(_headers);
                firstRowIsHeader = true;
            }

            if (_rows?.Any() == true)
                table.AddRange(_rows);

            if (!table.Any())
                return string.Empty;

            var formattedTable = new StringBuilder();

            var previousRow = table.FirstOrDefault();
            var nextRow = table.FirstOrDefault();

            int[] maximumCellWidths = GetMaxCellWidths(table);

            formattedTable = CreateTopLine(maximumCellWidths, nextRow.Count(), formattedTable);

            int rowIndex = 0;
            int lastRowIndex = table.Count - 1;

            for (int i = 0; i < table.Count; i++)
            {
                var row = table[i];

                var align = RowTextAlignRight;
                if (i == 0 && firstRowIsHeader)
                    align = HeaderTextAlignRight;

                formattedTable = CreateValueLine(maximumCellWidths, row, align, formattedTable);

                previousRow = row;

                if (rowIndex != lastRowIndex)
                {
                    nextRow = table[rowIndex + 1];
                    formattedTable = CreateSeperatorLine(maximumCellWidths, previousRow.Count(), nextRow.Count(), formattedTable);
                }

                rowIndex++;
            }

            formattedTable = CreateBottomLine(maximumCellWidths, previousRow.Count(), formattedTable);

            return formattedTable.ToString();
        }
    }
}