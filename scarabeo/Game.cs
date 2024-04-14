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

				scarabeo.PrintBoard();
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
			int bestRow = -1, bestCol = -1;

			while (i < extractedLetters.Length)
			{
				string combinedLetters = GetCombinationOfLetters(i, j);

				if (!IsWordValid(combinedLetters))
					continue;

				int tmpValue = CalculateWordValue(combinedLetters);
				int tmpBestRow = -1, tmpBestCol = -1;

				if (PutWordBasedOnConstraintCharacter(combinedLetters, tmpValue, ref tmpBestRow, ref tmpBestCol) == -1)
					continue;
				
				if (maxValue < tmpValue)
				{
					maxValue = tmpValue;
					highestScoreWord = combinedLetters;
					bestRow = tmpBestRow;  bestCol = tmpBestCol;
				}

				if (j == extractedLetters.Length - 1)
				{
					i++;
					j = 0;
				}
				else
					j++;
			}

			if (!string.IsNullOrEmpty(highestScoreWord))
				InsertHighestScoreWordInBoard(bestRow, bestCol, highestScoreWord);

			return highestScoreWord;
		}


		private void InsertHighestScoreWordInBoard(int bestRow, int bestCol, string highestScoreWord)
		{
			for (int j = 0; j < highestScoreWord.Length; j++)
				scarabeo[bestRow, bestCol++] = highestScoreWord[j];
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


		// have to save the i, j to be able to place the word
		// in best column
		private int PutWordBasedOnConstraintCharacter(string combinedLetters, int wordValue, ref int bestRow, ref int bestCol)
		{
			const int BOARD_SIZE = Scarabeo.BOARD_SIZE;
			int maxValue = -1;

			for (int i = 0; i < BOARD_SIZE; i++)
			{
				int j;

				for (j = 0; j < BOARD_SIZE; j++)
					if (scarabeo[i, j] != null)
						break;

				int constraintCharacterIndex = j; 

				int leftAvaibleSpace = constraintCharacterIndex - 1;
				int rightAvaibleSpace = (BOARD_SIZE - 1) - constraintCharacterIndex;
				
				if (!WordCanBePlacedInColumn(
					leftAvaibleSpace, rightAvaibleSpace, 
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

			return maxValue;
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