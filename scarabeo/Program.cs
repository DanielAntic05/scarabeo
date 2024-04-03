using System;

namespace Scarabeo
{
	class Program
	{
		static void Main()
		{
			CTrie trie = new();

			trie.Insert("yogurt");
			trie.Print();
			trie.Delete("yogurt");
			trie.Print();
		}
	}

}