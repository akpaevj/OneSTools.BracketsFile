using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.IO.Compression;
using System.IO;
using System.Threading.Tasks;

namespace OneSTools.BracketsFile
{
    /// <summary>
    /// Represents methods for the parsing of the 1C "brackets file"
    /// </summary>
    public static class BracketsFileParser
    {
        public static BracketsFileNode ParseBlock(string text, int startIndex = 0, int endIndex = -1)
        {
            var strBuilder = new StringBuilder(text);

            return ParseBlock(strBuilder, startIndex, endIndex);
        }
       
        public static BracketsFileNode ParseBlock(StringBuilder text, int startIndex = 0, int endIndex = -1)
        {
            var node = new BracketsFileNode();

            if (endIndex == -1)
                endIndex = GetNodeEndIndex(text, startIndex);
            if (endIndex == -1)
                endIndex = text.Length - 1;

            if (text[startIndex] == '{' && text[endIndex] == '}')
            {
                startIndex += 1;
                endIndex -= 1;
            }

            for (int i = startIndex; i <= endIndex; i++)
            {
                var currentChar = text[i];

                if (currentChar == '"') // string value
                {
                    var valueEndIndex = GetTextValueEndIndex(text, i);
                    var value = text.ToString(i + 1, valueEndIndex - i - 1);
                    node.Nodes.Add(new BracketsFileNode(value));

                    i = valueEndIndex;
                }
                else if (currentChar == '{') // new block
                {
                    var valueEndIndex = GetNodeEndIndex(text, i);
                    var value = ParseBlock(text, i, valueEndIndex);
                    node.Nodes.Add(value);

                    i = valueEndIndex;
                }
                else if (currentChar != '"' && currentChar != '}' && currentChar != ',' && !char.IsWhiteSpace(currentChar)) // another value
                {
                    var valueEndIndex = GetValueEndIndex(text, i);
                    var value = text.ToString(i, valueEndIndex - i);
                    node.Nodes.Add(new BracketsFileNode(value));

                    i = valueEndIndex;
                }
            }

            return node;
        }

        public static int GetNodeEndIndex(StringBuilder text, int startIndex)
        {
            int quotes = 0;
            int brackets = 0;

            for (int i = startIndex; i < text.Length; i++)
            {
                var prevChar = i > startIndex ? text[i - 1] : '\0';
                var currentChar = text[i];

                if (prevChar == ',' && currentChar == '"')
                {
                    var textValueEndIndex = GetTextValueEndIndex(text, i);

                    if (textValueEndIndex == -1)
                        return textValueEndIndex;

                    i = textValueEndIndex;
                    continue;
                }

                if (currentChar == '"')
                    quotes++;
                else if (currentChar == '{')
                    brackets++;
                else if (currentChar == '}')
                    brackets--;

                if (brackets == 0 && (quotes == 0 || (quotes != 0 && (quotes % 2) == 0)))
                    return i;
            }

            return -1;
        }

        public static int GetValueEndIndex(StringBuilder text, int startIndex)
        {
            for (int i = startIndex; i < text.Length; i++)
            {
                var c = text[i];

                if (c == ',' || c == '}')
                    return i;
            }

            return -1;
        }

        public static int GetTextValueEndIndex(StringBuilder text, int startIndex)
        {
            var brackets = 0;

            for (int i = startIndex; i < text.Length; i++)
            {
                var currentChar = text[i];
                var nextChar = text.Length > i + 1 ? text[i + 1] : '\0';

                if (currentChar == '"')
                    brackets++;

                if (currentChar == '"' && (nextChar == ',' || nextChar == '}') && (brackets == 0 || brackets % 2 == 0))
                    return i;
            }

            return -1;
        }
    }
}
