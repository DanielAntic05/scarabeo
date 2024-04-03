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

        public CTrieNode? Root { get; private set; }


        public void Insert(string key)
        {
            CTrieNode? tmpNode = Root;

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

                if (tmpNode.Children[index] == null)
                    return false;

                tmpNode = tmpNode.Children[index];
            }

            return (tmpNode.IsEndOfWord);
        }


        public void Delete(string key)
        {
            CTrieNode tmpNode = Root;

            for (int level = 0; level < key.Length; level++)
            {
                int index = key[level] - 'a';

                // if (tmpNode.Children[index] == )
            }
        }
    }

    
}