using System;
using System.Runtime.CompilerServices;

namespace Scarabeo
{
	class Game
	{
		private Scarabeo scarabeo;
		private CTrie dictionary;
		private static readonly string FILE = "dictionary.txt";
		public bool IsGameRunning { get; private set; } = true;



		public Game() 
		{
			scarabeo = new Scarabeo();
			dictionary = new CTrie();

			scarabeo.InitializeScarabeo();
			InitializeDictionary();
			Run();
		}


	
		private void InitializeDictionary()
		{
			if (!File.Exists(FILE))
				throw new FileLoadException($"\nError occured while opening {FILE}\n");

            using StreamReader file = new StreamReader(FILE);
            string line;

            while ((line = file.ReadLine()) != null)
                dictionary.Insert(line);

            file.Close();
        }


		private void Run()
		{
			while (true)
			{

			}
		}
	
	}
}