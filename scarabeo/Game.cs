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
		private string highestScoreWord = "";
		private int maxValue = 0, bestRow = -1, bestCol = -1;

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
			// dictionary.Print();
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
				{
					//Console.Write("\nline = {line}\n");
					dictionary.Insert(line);
				}

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
				Console.WriteLine($"\nextractedLetters = {extractedLetters};\n");

				string result = FindHighestScoreWord();	
				Console.WriteLine($"\nresult = {result};\n");

				scarabeo.PrintBoard();

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
			highestScoreWord = "";
			maxValue = 0; 
			bestRow = -1; 
			bestCol = -1;


			Foo(extractedLetters);

			for (int i = 0; i < extractedLetters.Length; i++)
				for (int j = 0; j < extractedLetters.Length; j++)
					Foo(extractedLetters.Remove(i, j));


			if (!string.IsNullOrEmpty(highestScoreWord))
				InsertHighestScoreWordInBoard(bestRow, bestCol, highestScoreWord);

			return highestScoreWord;
		}


		private void Foo(string word)
		{
			if (string.IsNullOrEmpty(word))
				return;

			int i = 0, j = 0;

			while (i < word.Length)
			{
				string combinedLetters = GetCombinationOfLetters(i, j);

				if (j == word.Length - 1)
				{
					i++;
					j = 0;
				}
				else
					j++;

				// Console.Write($"\ncombinedLetters = {combinedLetters}\n");

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
			}
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

			return new string(result);
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