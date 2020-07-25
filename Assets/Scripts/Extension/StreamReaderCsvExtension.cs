using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Extension
{
    /// <summary>
    /// Extends the stream reader with methods for handling CSV files.
    /// </summary>
    static class StreamReaderCsvExtension
    {
        /// <summary>
        /// Reads a line from the CSV line and splits them into cells. Allows new lines in cells.
        /// </summary>
        /// <param name="reader">The reader reading the CSV.</param>
        /// <param name="separator">The separator to use in the CSV file.</param>
        /// <returns>The list of cells in the current row, or empty list if EoF.</returns>
        public static List<string> ReadCsvLine(this StreamReader reader, char separator = ',')
        {
            // Ok, this is really hacky and won't work in all cases, but this is a quick way to get a line from csv line that might containg newlines in a cell between quotes.
            StringBuilder cellBuilder = new StringBuilder();
            List<string> cells = new List<string>();
            var buffer = new char[1];
            var isInQuotedCell = false;
            char? previousChar = null;
            while (!reader.EndOfStream)
            {
                reader.Read(buffer, 0, 1);
                var currentChar = buffer[0];
                if (previousChar == '"' && currentChar != '"')
                {
                    // Unescaped quote
                    isInQuotedCell = !isInQuotedCell;
                }
                if (currentChar == '\n' && !isInQuotedCell)
                {
                    break;
                }
                else if (currentChar == separator && !isInQuotedCell)
                {
                    cells.Add(cellBuilder.ToString());
                    cellBuilder.Clear();
                    previousChar = null;
                    continue;
                }
                else if (currentChar != '\r')
                {
                    cellBuilder.Append(currentChar);
                }
                previousChar = currentChar;
            }
            cells.Add(cellBuilder.ToString());
            return cells;
        }
    }
}
