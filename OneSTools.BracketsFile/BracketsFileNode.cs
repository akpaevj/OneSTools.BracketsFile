using System.Collections.Generic;

namespace OneSTools.BracketsFile
{
    public class BracketsFileNode
    {
        public bool IsValueNode { get; private set; }
        public string Text { get; private set; }
        public List<BracketsFileNode> Nodes { get; private set; } = new List<BracketsFileNode>();

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

        public override string ToString()
        {
            if (IsValueNode)
                return Text;

            return base.ToString();
        }
    }
}
