using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.IO.Compression;
using System.Threading.Tasks;

namespace OneSTools.BracketsFile
{
    /// <summary>
    /// Represents static methods for working with 1C "brackets" data
    /// </summary>
    public static class BracketsParser
    {
        /// <summary>
        /// Returns parsed node
        /// </summary>
        /// <param name="text"></param>
        /// <param name="startIndex"></param>
        /// <param name="endIndex"></param>
        /// <returns></returns>
        public static BracketsNode ParseBlock(string text, int startIndex = 0, int endIndex = -1)
        {
            var strBuilder = new StringBuilder(text);

            return ParseBlock(strBuilder, startIndex, endIndex);
        }
       
        /// <summary>
        /// Returns parsed node
        /// </summary>
        /// <param name="text"></param>
        /// <param name="startIndex"></param>
        /// <param name="endIndex"></param>
        /// <returns></returns>
        public static BracketsNode ParseBlock(StringBuilder text, int startIndex = 0, int endIndex = -1)
        {
            var node = new BracketsNode();

            if (endIndex == -1)
                endIndex = GetNodeEndIndex(text, startIndex);
            if (endIndex == -1)
                endIndex = text.Length - 1;

            if (text[startIndex] == '{' && text[endIndex] == '}')
            {
                startIndex += 1;
                endIndex -= 1;
            }

            for (var i = startIndex; i <= endIndex; i++)
            {
                var currentChar = text[i];

                switch (currentChar)
                {
                    // string value
                    case '"':
                    {
                        var valueEndIndex = GetTextValueEndIndex(text, i);
                        var value = text.ToString(i + 1, valueEndIndex - i - 1);
                        node.Nodes.Add(new BracketsNode(value));

                        i = valueEndIndex;
                        break;
                    }
                    // new block
                    case '{':
                    {
                        var valueEndIndex = GetNodeEndIndex(text, i);
                        var value = ParseBlock(text, i, valueEndIndex);
                        node.Nodes.Add(value);

                        i = valueEndIndex;
                        break;
                    }
                    default:
                    {
                        if (currentChar != '"' && currentChar != '}' && currentChar != ',' && !char.IsWhiteSpace(currentChar)) // another value
                        {
                            var valueEndIndex = GetValueEndIndex(text, i);
                            var value = text.ToString(i, valueEndIndex - i);
                            node.Nodes.Add(new BracketsNode(value));

                            i = valueEndIndex;
                        }

                        break;
                    }
                }
            }

            return node;
        }

        /// <summary>
        /// Returns the last index of the block. If the end hasn't been found than returns -1
        /// </summary>
        /// <param name="text"></param>
        /// <param name="startIndex"></param>
        /// <returns></returns>
        public static int GetNodeEndIndex(StringBuilder text, int startIndex)
        {
            var quotes = 0;
            var brackets = 0;
            var textValueStartIndex = -1;
            var textValueStarted = false;

            return GetNodeEndIndex(text, ref startIndex, ref quotes, ref brackets, ref textValueStartIndex, ref textValueStarted);
        }

        /// <summary>
        /// Returns the last index of the block and save counted key symbols in refs. If the end hasn't been found than returns -1
        /// </summary>
        /// <param name="text"></param>
        /// <param name="index"></param>
        /// <param name="quotes"></param>
        /// <param name="brackets"></param>
        /// <param name="textValueStartIndex"></param>
        /// <param name="textValueStarted"></param>
        /// <returns></returns>
        internal static int GetNodeEndIndex(StringBuilder text, ref int index, ref int quotes, ref int brackets, ref int textValueStartIndex, ref bool textValueStarted)
        {
            while (index < text.Length)
            {
                var prevChar = index > 0 ? text[index - 1] : '\0';
                var currentChar = text[index];

                if (!textValueStarted && prevChar == ',' && currentChar == '"')
                {
                    textValueStartIndex = index;
                    textValueStarted = true;

                    var textValueEndIndex = GetTextValueEndIndex(text, index);

                    if (textValueEndIndex == -1)
                    {
                        index = textValueEndIndex;
                        return index;
                    }

                    textValueStarted = false;
                    index = textValueEndIndex;
                    index++;
                    continue;
                }

                switch (currentChar)
                {
                    case '"':
                        quotes++;
                        break;
                    case '{':
                        brackets++;
                        break;
                    case '}':
                        brackets--;
                        break;
                }

                if (brackets == 0 && (quotes == 0 || (quotes != 0 && (quotes % 2) == 0)))
                    return index;

                index++;
            }

            return -1;
        }

        /// <summary>
        /// Returns the last index of the any value (except string value and block). If the end hasn't been found than returns -1
        /// </summary>
        /// <param name="text"></param>
        /// <param name="startIndex"></param>
        /// <returns></returns>
        public static int GetValueEndIndex(StringBuilder text, int startIndex)
        {
            for (var i = startIndex; i < text.Length; i++)
            {
                var c = text[i];

                if (c == ',' || c == '}')
                    return i;
            }

            return -1;
        }

        /// <summary>
        /// Returns the last index of the text value. If the end hasn't been found than returns -1
        /// </summary>
        /// <param name="text"></param>
        /// <param name="startIndex"></param>
        /// <returns></returns>
        public static int GetTextValueEndIndex(StringBuilder text, int startIndex)
        {
            var brackets = 0;

            for (var i = startIndex; i < text.Length; i++)
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
