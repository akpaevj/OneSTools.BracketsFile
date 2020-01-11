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
        /// <summary>
        /// Parses the text and retuns an instance of the "BracketsFileNode" class
        /// </summary>
        /// <param name="text">The source text of the 1C "brackets file"</param>
        /// <returns></returns>
        public static BracketsFileNode Parse(string text)
        {
            var t = Regex.Replace(text, @"(\n|\r)", "");

            var root = new BracketsFileNode();

            Parse(root, ref t, text.IndexOf('{') + 1, t.Length - 2);

            return root;
        }

        private static void Parse(BracketsFileNode parentNode, ref string text, int currentIndex, int lastIndex)
        {
            var propNodeStartPosition = -1;

            for (int i = currentIndex; i <= lastIndex; i++)
            {
                char currentChar = text[i];

                if (currentChar == '{')
                {
                    propNodeStartPosition = -1;

                    var nodeEndIndex = GetNodeEndIndex(ref text, i);

                    var node = new BracketsFileNode();
                    parentNode.Nodes.Add(node);

                    Parse(node, ref text, i + 1, nodeEndIndex);

                    i = nodeEndIndex;
                }
                else if (currentChar == ',')
                {
                    if (propNodeStartPosition != -1)
                    {
                        var t = text.Substring(propNodeStartPosition, i - propNodeStartPosition).Trim('"');

                        var node = new BracketsFileNode(t);
                        parentNode.Nodes.Add(node);

                        propNodeStartPosition = -1;
                    }
                }
                else
                {
                    if (propNodeStartPosition == -1)
                        propNodeStartPosition = i;
                }
            }

            if (propNodeStartPosition != -1)
            {
                var offset = text[lastIndex - propNodeStartPosition] == '}' ? 1 : 0;
                var count = lastIndex - propNodeStartPosition - offset;
                if (count > 0)
                {
                    var t = text.Substring(propNodeStartPosition, lastIndex - propNodeStartPosition - offset).Trim('"');
                    var node = new BracketsFileNode(t);
                    parentNode.Nodes.Add(node);
                }
            }
        }

        /// <summary>
        /// Returns the end index of the current block
        /// </summary>
        /// <param name="parentNode"></param>
        /// <param name="startIndex"></param>
        /// <returns></returns>
        private static int GetNodeEndIndex(ref string text, int startIndex)
        { 
            int endIndex = text.Length - startIndex - 1;
            int quotesAmount = 0;
            int bracketAmount = 0;

            for (int x = startIndex; x < text.Length; x++)
            {
                var currentChar = text[x];

                if (currentChar == '{')
                    bracketAmount += 1;
                else if (currentChar == '}')
                    bracketAmount -= 1;
                else if (currentChar == '"')
                    quotesAmount += 1;

                endIndex = x;

                if (bracketAmount == 0 && (quotesAmount % 2) == 0)
                    break;
            }

            return endIndex;
        }
    }
}
