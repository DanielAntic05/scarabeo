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

		}


		public void InitializeGame()
		{
			scarabeo.InitializeScarabeo();
			InitializeDictionary();
			Run();
		}
	
		private void InitializeDictionary()
		{
			if (!File.Exists("files\\" + FILE))
				throw new FileNotFoundException($"\nThe file '{FILE}' could not be opened because it does not exist.\n");

			try
			{
				using StreamReader file = new("files\\" + FILE);
				string? line;

				while ((line = file.ReadLine()) != null)
					dictionary.Insert(line);

				file.Close();
			}
			catch
			{
				Console.Write($"\nError occured while reading the contents of the file '{FILE}'.\n");
			}
        }


		private void Run()
		{
			while (IsGameRunning)
			{
				extractedLetters = Distributor.GenerateRandomLetters();
				
				string result = FindHighestScoreWord();	

				Console.WriteLine($"\nresult = {result};\n");

				Console.Write("\nPress any key to change the turn.\n");
				Console.ReadKey();

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

				if (PutWordBasedOnConstraintCharacter(combinedLetters, tmpValue) == -1)
					continue;
				
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


		private int PutWordBasedOnConstraintCharacter(string combinedLetters, int wordValue)
		{
			const int BOARD_SIZE = Scarabeo.BOARD_SIZE;
			int leftAvaibleSpace = -1, rightAvaibleSpace = -1;
			int bestRow = -1, bestCol = -1;
			int maxValue = 0;

			for (int i = 0; i < BOARD_SIZE; i++)
			{
				int j;

				for (j = 0; j < BOARD_SIZE; j++)
					if (scarabeo[i, j] != null)
						break;

				int constraintCharacterIndex = j; 

				int tmpLeftAvaibleSpace = constraintCharacterIndex - 1;
				int tmpRightAvaibleSpace = (BOARD_SIZE - 1) - constraintCharacterIndex;
				
				if (!WordCanBePlacedInColumn(
					tmpLeftAvaibleSpace, tmpRightAvaibleSpace, 
					constraintCharacterIndex, combinedLetters.Length))
					
					continue;

				// puts the word in the right way in the col of the board ---> example:  {, , , i, , ,} <---- "ciao"
				// 																							  output: {, , c, i, a, o}

				// here i have to compare the points of the word 
				// in each column considering the "bonus".

				if (maxValue < wordValue)
				{
					bestRow = i; bestCol = j;
					maxValue = wordValue;
				}
			}

			return -1;
		}


		private void PutWordInBestColumn(int leftAvaibleSpace, int constraintCharacterIndex, int combinedLetters)
		{
			for (int l = 0, k = leftAvaibleSpace - (constraintCharacterIndex - 1); k < combinedLetters.Length; k++, l++)
				scarabeo[i, k] = combinedLetters[l];
		}


		// Controls if there is enough space before and 
		// after the cell where constraint character is placed.
		// Example:  {, l, , ,}   input: "hello" ---> there is not enough space at the left of 'i'

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