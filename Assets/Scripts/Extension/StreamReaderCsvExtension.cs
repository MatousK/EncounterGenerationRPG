using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Extension
{
    static class StreamReaderCsvExtension
    {
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
