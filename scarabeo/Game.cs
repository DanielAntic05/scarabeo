using System;
using System.Runtime.CompilerServices;

namespace Scarabeo
{
	class Game
	{
		private int turn = 0;
		private static readonly string FILE = "dictionary.txt";
		private static readonly string[] letterGroups = { "aceiorst", "lmn", "p", "bdfguv", "hz", "q" };
 	    private static readonly int[] points = { 1, 2, 3, 4, 8, 10 };
		private	string? extractedLetters;
		private Scarabeo scarabeo;
		private CTrie dictionary;
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
			while (IsGameRunning)
			{
				extractedLetters = Distributor.GenerateRandomLetters();
				
				string result = FindHighestScoreWord();	




				turn = (turn + 1) % 2;
			}
		}


		private string FindHighestScoreWord()
		{
			int maxValue = 0;
			int i = 0, j = 0;
			string highestScoreWord = "";

			while (i < extractedLetters.Length)
			{
				string combinedLetters = GetCombinationOfLetters(i, j);

				if (!IsWordValid(in combinedLetters))
					continue;

				int tmpValue = CalculateWordValue(in combinedLetters);
				
				if (maxValue < tmpValue) // TODO control also the bonus in the board
					maxValue = tmpValue;

				if (j == extractedLetters.Length - 1)
				{
					i++;
					j = 0;
				}
				else
					j++;
			}

			return highestScoreWord;
		}


		private string GetCombinationOfLetters(int i, int j)
		{
			if (i == 0 && j == 0)
				return extractedLetters;

			char[] result = extractedLetters.ToCharArray();
			char tmpChar = result[i];

			result[i] = result[j];
			result[j] = tmpChar;

			return result.ToString();
		}


		private bool IsWordValid(in string combinedLetters)
		{
			return dictionary.Search(combinedLetters);
		}


		private int CalculateWordValue(in string combinedLetters)
		{
			int counter = 0;

			foreach (char character in combinedLetters)
				counter += FindLetterValue(character);

			return counter;
		}


		private int FindLetterValue(in char c)
		{
			for (int i = 0; i < letterGroups.Length; i++)
				for (int j = 0; j < letterGroups.Length; j++)
					if (letterGroups[i][j] == c)
						return points[i];

			return -1;
		}
	}
}