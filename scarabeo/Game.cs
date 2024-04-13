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
			if (!File.Exists("files\\" + FILE))
				throw new FileNotFoundException($"\nThe file '{FILE}' could not be opened because it does not exist.\n");

            using StreamReader file = new("files\\" + FILE);
            string line;

            while ((line = file.ReadLine()) != null)
                //dictionary.Insert(line);
				Console.WriteLine($"\n{line}\n");

            file.Close();
        }


		private void Run()
		{
			while (IsGameRunning)
			{
				extractedLetters = Distributor.GenerateRandomLetters();
				
				string result = FindHighestScoreWord();	

				Console.WriteLine($"\nresult = {result};\n");


				turn = (turn + 1) % 2;
			}
		}

		// TODO 
		/*
			1) combine the letters
			2) check if the word is valid
			3) control the points

			after these steps the word can be placed

			using bonus:
			5) 
		*/
		private string FindHighestScoreWord()
		{
			int maxValue = 0;
			int i = 0, j = 0;
			string highestScoreWord = "";

			while (i < extractedLetters.Length)
			{
				string combinedLetters = GetCombinationOfLetters(i, j);

				if (!IsWordValid(combinedLetters))
					continue;

				int tmpValue = CalculateWordValue(combinedLetters);
				
				if (maxValue < tmpValue)
				{
					maxValue = tmpValue;
					highestScoreWord = combinedLetters;
				}

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


		private bool IsWordValid(string combinedLetters)
		{
			return dictionary.Search(combinedLetters);
		}


		private int FindPositionOfConstraintCharacter(string combinedLetters)
		{
			const int BOARD_SIZE = Scarabeo.BOARD_SIZE;

			for (int i = 0; i < BOARD_SIZE; i++)
			{
				char constraintCharacter = '\0';
				int j;

				for (j = 0; j < BOARD_SIZE; j++)
					if (scarabeo[i, j] != null)
					{
						constraintCharacter = scarabeo[i, j];
						break;
					}

				int constraintCharacterIndex = combinedLetters.IndexOf(constraintCharacter);

				if (constraintCharacter == '\0' || constraintCharacter == -1)
					break;

				int leftAvaibleSpace = j - 1;
				int rightAvaibleSpace = (BOARD_SIZE - 1) - j;
				
				if (!WordCanBePlacedInColumn(
					leftAvaibleSpace, rightAvaibleSpace, 
					constraintCharacterIndex, combinedLetters.Length))
					
					break;

				// put the word in the right way in the col of the board ---> example:  {, , , i, , ,} <---- "ciao"

																						// output: {, , c, i, a, o}
				for (int l = 0, k = leftAvaibleSpace - (constraintCharacterIndex - 1); k < combinedLetters.Length; k++, l++)
					scarabeo[i, k] = combinedLetters[l];
			}


			return -1;
		}


		private bool WordCanBePlacedInColumn(int leftAvaibleSpace, int rightAvaibleSpace, int constraintCharacterIndex, int wordLength)
		{
			return leftAvaibleSpace >= constraintCharacterIndex - 1 &&
				   rightAvaibleSpace >= (wordLength - 1) - constraintCharacterIndex;
		}


		private int CalculateWordValue(string combinedLetters)
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