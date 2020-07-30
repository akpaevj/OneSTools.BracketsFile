using System;
using System.Collections;
using System.Collections.Generic;

namespace OneSTools.BracketsFile
{
    public class BracketsFileNode : IEnumerable<BracketsFileNode>
    {
        public bool IsValueNode { get; private set; }
        public string Text { get; private set; }
        public List<BracketsFileNode> Nodes { get; private set; } = new List<BracketsFileNode>();
        public int Count => Nodes.Count;

        public BracketsFileNode this[int index]
        {
            get
            {
                return Nodes[index];
            }
        }

        public BracketsFileNode()
        {
            IsValueNode = false;
        }
        public BracketsFileNode(string text)
        {
            Text = text;
            IsValueNode = true;
        }

        public BracketsFileNode GetNode(params int[] address)
        {
            BracketsFileNode currentNode = this;

            for (int i = 0; i < address.Length; i++)
            {
                currentNode = currentNode[address[i]];
            }

            return currentNode;
        }

        public static explicit operator string(BracketsFileNode node)
        {
            return node.Text;
        }
        public static explicit operator int(BracketsFileNode node)
        {
            return int.Parse(node.Text);
        }
        public static explicit operator Guid(BracketsFileNode node)
        {
            return Guid.Parse(node.Text);
        }
        public static explicit operator bool(BracketsFileNode node)
        {
            if (!node.IsValueNode)
                throw new ArgumentException("The node doesn't present a value");

            if ((string)node == "1")
                return true;
            else if ((string)node == "0")
                return false;
            else
                throw new ArgumentException($"\"{node.Text}\" value can not be casted to boolean");
        }

        public override string ToString()
        {
            if (IsValueNode)
                return Text;
            else
                return $"Count = {Nodes.Count}";
        }

        public IEnumerator<BracketsFileNode> GetEnumerator()
        {
            return Nodes.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
