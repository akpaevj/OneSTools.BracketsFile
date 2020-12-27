using System;
using System.Collections;
using System.Collections.Generic;

namespace OneSTools.BracketsFile
{
    public class BracketsNode : IEnumerable<BracketsNode>
    {
        public bool IsValueNode { get; private set; }
        public string Text { get; private set; }
        public List<BracketsNode> Nodes { get; private set; } = new List<BracketsNode>();
        public int Count => Nodes.Count;

        public BracketsNode this[int index]
        {
            get
            {
                return Nodes[index];
            }
        }

        public BracketsNode()
        {
            IsValueNode = false;
        }

        public BracketsNode(string text)
        {
            Text = text;
            IsValueNode = true;
        }

        public BracketsNode GetNode(params int[] address)
        {
            BracketsNode currentNode = this;

            for (int i = 0; i < address.Length; i++)
            {
                currentNode = currentNode[address[i]];
            }

            return currentNode;
        }

        public static implicit operator string(BracketsNode node)
        {
            return node.Text;
        }
        public static implicit operator short(BracketsNode node)
        {
            return short.Parse(node.Text);
        }
        public static implicit operator ushort(BracketsNode node)
        {
            return ushort.Parse(node.Text);
        }
        public static implicit operator int(BracketsNode node)
        {
            return int.Parse(node.Text);
        }
        public static implicit operator uint(BracketsNode node)
        {
            return uint.Parse(node.Text);
        }
        public static implicit operator long(BracketsNode node)
        {
            return long.Parse(node.Text);
        }
        public static implicit operator ulong(BracketsNode node)
        {
            return ulong.Parse(node.Text);
        }
        public static implicit operator Guid(BracketsNode node)
        {
            return Guid.Parse(node.Text);
        }
        public static implicit operator bool(BracketsNode node)
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

        public IEnumerator<BracketsNode> GetEnumerator()
        {
            return Nodes.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
