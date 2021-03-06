﻿using System;
using System.Text;
using System.IO;

namespace OneSTools.BracketsFile
{
    /// <summary>
    /// Represents methods for working with 1C "brackets" data that looks like a list of items
    /// </summary>
    public class BracketsListReader : IDisposable
    {
        private readonly StreamReader _stream;
        private bool disposedValue;

        /// <summary>
        /// Current position of the stream
        /// </summary>
        public long Position
        {
            get => _stream.GetPosition();
            set => _stream.SetPosition(value);
        }
        /// <summary>
        /// Stream's end flag
        /// </summary>
        public bool EndOfStream => _stream.EndOfStream;

        public BracketsListReader(Stream stream)
            => _stream = new StreamReader(stream);

        public BracketsListReader(Stream stream, int bufferSize)
            => _stream = new StreamReader(stream, Encoding.UTF8, false, bufferSize);

        public BracketsListReader(StreamReader stream)
            => _stream = stream;

        /// <summary>
        /// Reads and returns data of the next "brackets" item. If there is no data or the end of the item hasn't been found than it returns null
        /// </summary>
        /// <returns></returns>
        public string NextNodeAsString()
        {
            var itemBuilder = NextNodeAsStringBuilder();

            if (itemBuilder.Length == 0)
                return null;
            else
                return itemBuilder.ToString();
        }

        /// <summary>
        /// Reads and returns data of the next "brackets" item. If there is no data or the end of the item hasn't been found than it returns null
        /// </summary>
        /// <returns></returns>
        public BracketsNode NextNode()
        {
            var itemBuilder = NextNodeAsStringBuilder();

            if (itemBuilder.Length == 0)
                return null;
            else
                return BracketsParser.ParseBlock(itemBuilder);
        }

        /// <summary>
        /// Reads and returns a string builder of the next "brackets" item. If there is no data or the end of the item hasn't been found than it returns empty string builder
        /// </summary>
        /// <returns></returns>
        public StringBuilder NextNodeAsStringBuilder()
        {
            var itemData = new StringBuilder();

            var started = false;
            var index = 0;
            var quotes = 0;
            var brackets = 0;
            var endIndex = -1;
            var textValueBrackets = 0;

            while (!EndOfStream)
            {
                var currentChar = (char) _stream.Read();

                if (currentChar == -1)
                    break;

                if (!started && currentChar == '{')
                    started = true;

                if (started)
                    itemData.Append(currentChar);
                else
                    continue;

                if (textValueBrackets > 0)
                {
                    var textValueEndIndex = BracketsParser.GetTextValueEndIndex(itemData, itemData.Length - 1, ref textValueBrackets);

                    if (textValueEndIndex == -1)
                        continue;

                    index = textValueEndIndex;
                    textValueBrackets = 0;
                    continue;
                }

                endIndex = BracketsParser.GetNodeEndIndex(itemData, ref index, ref quotes, ref brackets, ref textValueBrackets);

                if (endIndex != -1)
                    break;
            }

            if (endIndex != -1) 
                return itemData;

            // if there is no end index than set stream's position to the beginning of the item and returns empty value
            var itemDataBytesLength = _stream.CurrentEncoding.GetBytes(itemData.ToString()).Length;
            _stream.SetPosition(_stream.GetPosition() - itemDataBytesLength);

            return itemData.Clear();

        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    
                }

                _stream?.Dispose();

                disposedValue = true;
            }
        }

        ~BracketsListReader()
        {
            Dispose(disposing: false);
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}