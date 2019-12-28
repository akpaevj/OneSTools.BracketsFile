using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace OneSTools.BracketsFile
{
    /// <summary>
    /// Represents methods for the parsing of the 1C "brackets file"
    /// </summary>
    public class BracketsFileParser
    {
        /// <summary>
        /// Parsed data
        /// </summary>
        public List<object> StructuredData { get; private set; }
        /// <summary>
        /// Returns a value of the property by the passed address
        /// </summary>
        /// <param name="node">The node for the searching</param>
        /// <param name="address">Node address</param>
        /// <returns></returns>
        public static object GetNodeProperty(List<object> node, params int[] address)
        {
            object currentNode = node;

            foreach (var addr in address)
            {
                currentNode = (currentNode as List<object>)[addr];
            }

            return currentNode;
        }
        /// <summary>
        /// Returns a value of the property by the passed address
        /// </summary>
        /// <param name="address">Node address</param>
        /// <returns></returns>
        public object GetNodeProperty(params int[] address)
        {
            return GetNodeProperty(StructuredData, address);
        }
        /// <summary>
        /// Returns a typed value of the property by the passed address
        /// </summary>
        /// <param name="node">The node for the searching</param>
        /// <param name="address">Node address</param>
        /// <returns></returns>
        public static T GetNodeProperty<T>(List<object> node, params int[] address)
        {
            return (T)Convert.ChangeType(GetNodeProperty(node, address), typeof(T));
        }
        /// <summary>
        /// Returns a typed value of the property by the passed address
        /// </summary>
        /// <param name="address">Node address</param>
        /// <returns></returns>
        public T GetNodeProperty<T>(params int[] address)
        {
            return (T)Convert.ChangeType(GetNodeProperty(address), typeof(T));
        }
        /// <summary>
        /// Searches the value and returns an its address
        /// </summary>
        /// <param name="value">Value for the searchng</param>
        /// <returns></returns>
        public int[] WhereIs(string value)
        {
            var address = new List<int>();

            WhereIs(value, StructuredData, address);

            if (address.Count > 0)
            {
                return address.ToArray();
            }
            else
            {
                return null;
            }
        }
        private bool WhereIs(string value, List<object> node, List<int> address)
        {
            for (int x = 0; x < node.Count; x++)
            {
                var currentNode = node[x];

                if (currentNode is List<object> n)
                {
                    address.Add(x);

                    var res = WhereIs(value, n, address);

                    if (res)
                    {
                        return true;
                    }
                    else
                    {
                        address.RemoveAt(address.Count - 1);
                    }
                }
                else if (currentNode.ToString() == value)
                {
                    address.Add(x);

                    return true;
                }
            }

            return false;
        }
        /// <summary>
        /// Parses the text and retuns an instance of the "BracketsFile" class
        /// </summary>
        /// <param name="text">The source text of the 1C "brackets file"</param>
        /// <returns></returns>
        public static BracketsFileParser Parse(string text)
        {
            var file = new BracketsFileParser();

            StringBuilder s = new StringBuilder(Regex.Replace(text, @"(\n|\r)", ""));
            file.StructuredData = ParseText(s, s.ToString().IndexOf('{'), s.ToString().Length - 1, true);

            return file;
        }
        private static List<object> ParseText(StringBuilder text, int startIndex, int lastIndex, bool root)
        {
            List<object> data = new List<object>();

            StringBuilder currentProperty = new StringBuilder();

            for (int x = startIndex; x <= lastIndex; x++)
            {
                var c = text[x];

                if (c == '{')
                {
                    var blockLastIndex = GetBlockEndIndex(text, x);

                    if (root && data.Count == 0)
                    {
                        data = ParseText(text, x + 1, blockLastIndex - 1, false);
                    }
                    else
                    {
                        data.Add(ParseText(text, x + 1, blockLastIndex - 1, false));
                    }

                    x = blockLastIndex + 1;
                }
                else if (c == ',')
                {
                    if (currentProperty.Length > 0)
                    {
                        data.Add(currentProperty.ToString().Trim('"'));

                        currentProperty.Clear();
                    }
                }
                else
                {
                    currentProperty.Append(c);
                }
            }

            if (currentProperty.Length > 0) data.Add(currentProperty.ToString().Trim('"'));

            return data;
        }
        /// <summary>
        /// Returns the end index of the current block
        /// </summary>
        /// <param name="text"></param>
        /// <param name="startIndex"></param>
        /// <returns></returns>
        private static int GetBlockEndIndex(StringBuilder text, int startIndex)
        {
            int endIndex = text.Length - startIndex - 1;
            int quotesAmount = 0;
            int bracketAmount = 0;

            for (int x = startIndex; x < text.Length; x++)
            {
                var currentChar = text[x];

                if (currentChar == '{')
                {
                    bracketAmount += 1;
                }
                else if (currentChar == '}')
                {
                    bracketAmount -= 1;
                }
                else if (currentChar == '"')
                {
                    quotesAmount += 1;
                }

                endIndex = x;

                if (bracketAmount == 0 && (quotesAmount % 2) == 0)
                {
                    break;
                }
            }

            return endIndex;
        }
    }
}
