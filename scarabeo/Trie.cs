using System;

namespace Scarabeo
{
    class CTrie
    {
        private static readonly int ALPHABET_SIZE = 26;

        internal class CTrieNode
        {
            public CTrieNode[] Children { get; set; }
            public bool IsEndOfWord     { get; set; }

            public CTrieNode()
            {
                Children = new CTrieNode[ALPHABET_SIZE];
                IsEndOfWord = false;
            }
        } 


        private readonly CTrieNode Root;


        public CTrie() { Root = new CTrieNode(); }


        public void Insert(string key)
        {
            CTrieNode tmpNode = Root;

            for (int level = 0; level < key.Length; level++)
            {
                int index = key[level] - 'a';

                if (tmpNode.Children[index] == null)
                    tmpNode.Children[index] = new CTrieNode();

                tmpNode = tmpNode.Children[index];
            }

            tmpNode.IsEndOfWord = true;
        }


        public bool Search(string key)
        {
            CTrieNode tmpNode = Root;

            for (int level = 0; level < key.Length; level++)
            {
                int index = key[level] - 'a';

                // Console.Write($"\nindex = {key[level]}\n");

                if (tmpNode.Children[index] == null)
                    return false;

                tmpNode = tmpNode.Children[index];
            }

            return tmpNode.IsEndOfWord;
        }


        public void Delete(string key)
        {
            DeleteRecursive(Root, key, 0);
        }


        private bool DeleteRecursive(CTrieNode node, string key, int level)
        {
            if (node == null)
                return false;

            if (level == key.Length)
            {
                if (node.IsEndOfWord)
                {
                    node.IsEndOfWord = false;
                    return IsNodeEmpty(node);
                }

                return false;
            }

            int index = key[level] - 'a';

            if (DeleteRecursive(node.Children[index], key, level + 1))
            {
                node.Children[index] = null;
                return !node.IsEndOfWord && IsNodeEmpty(node);
            }

            return false;
        }


        private bool IsNodeEmpty(CTrieNode node)
        {
            foreach (var child in node.Children)
                if (child != null)
                    return false;

            return true;
        }



        public void Print()
        {
            PrintRecursive(Root, "");
        }


        private void PrintRecursive(CTrieNode node, string prefix)
        {
            if (node.IsEndOfWord)
                Console.WriteLine(prefix);

            for (int level = 0; level < node.Children.Length; level++)
            {
                if (node.Children[level] == null)
                    continue;

                char c = (char)('a' + level);
                PrintRecursive(node.Children[level], prefix + c);
            }
        }
    }
}