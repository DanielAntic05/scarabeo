using System;
using System.ComponentModel;
using System.Data;
using System.Runtime.CompilerServices;

namespace Scarabeo
{
	class Game
	{
		private const int N_PLAYERS = 2;
		private const int PLAYER_MAX_LETTERS = 8;
		private const int PENALITY = 5;
		private const int BOARD_SIZE = Scarabeo.BOARD_SIZE;
		private int turn = 1;
		private static readonly string FILE = "dictionary.txt";
		private static readonly string[] letterGroups = { "aceiorst", "lmn", "p", "bdfguv", "hz", "q" };
 	    private static readonly int[] points = { 1, 2, 3, 4, 8, 10 };
		private Scarabeo scarabeo;
		private CTrie dictionary;

		private string highestScoreWord = "";
		private int maxValue = 0, bestRow = -1, bestCol = -1;

		private List<char>[] playerExtractedLetters = new List<char>[N_PLAYERS];

		private int[] playerPoints = new int[N_PLAYERS];

		private int numberOfWordsFound = 0;
		public bool IsGameRunning { get; private set; } = true;


		public Game() 
		{
			scarabeo = new Scarabeo();
			dictionary = new CTrie();

			for (int i = 0; i < N_PLAYERS; i++)
				playerExtractedLetters[i] = new List<char>();
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
			while (numberOfWordsFound < BOARD_SIZE)
			{
				turn = (turn + 1) % N_PLAYERS;

				string extractedLetters = Distributor.GenerateRandomLetters(PLAYER_MAX_LETTERS - playerExtractedLetters[turn].Count());

				for (int i = 0; i < extractedLetters.Length; i++)
					playerExtractedLetters[turn].Add(extractedLetters[i]);

				//Console.WriteLine($"\nextractedLetters = {extractedLetters};\n");
				FindHighestScoreWord();

				if (string.IsNullOrEmpty(highestScoreWord))
				{
					ManageSituationWithNoResult();	
					continue;
				}

				Console.WriteLine($"highestScoreWord = {highestScoreWord}");
				++numberOfWordsFound;

				for (int i = 0; i < highestScoreWord.Length; i++)
					playerExtractedLetters[turn].RemoveAll(c => c == highestScoreWord[i]);

				playerPoints[turn] += maxValue;
			}

			scarabeo.PrintBoard();
			IsGameRunning = false;
		}

		private void ManageSituationWithNoResult()
		{
			playerExtractedLetters[turn].Clear();
/*
			Console.Write("\nPress 's' to substitute the letters with 8 new letters.\tWARNING: You will lose 5 points.");
			Console.Write("\nPress any other key to pass the turn.\tYou will gain 0 points");
			ConsoleKeyInfo keyInfo = Console.ReadKey();
			char inputCharacter = keyInfo.KeyChar;

			switch(inputCharacter)
			{
				case 's':
					playerPoints[turn] -= PENALITY;
					turn++;  // to keep the turn of the current_player
					break;

				default:
					break;
			}
*/
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
		private void FindHighestScoreWord()
		{
			highestScoreWord = "";
			maxValue = 0; 
			bestRow = -1; 
			bestCol = -1;

			int stringLength = playerExtractedLetters[turn].Count();
			string extractedLetters = new(playerExtractedLetters[turn].ToArray());

			for (int i = 0; i < stringLength; i++)
				for (int j = i; j < stringLength; j++)
					ManagePermutations(extractedLetters.Substring(i, stringLength - j));


			if (!string.IsNullOrEmpty(highestScoreWord))
				InsertHighestScoreWordInBoard(bestRow, bestCol, highestScoreWord);
		}


		private void ManagePermutations(string word)
		{
			if (string.IsNullOrEmpty(word))
				return;

			List<string> permutations = new List<string>();

			Permute(playerExtractedLetters[turn].ToArray(), 0, word.Length, ref permutations);

			for (int i = 0; i < permutations.Count(); i++)
			{
				if (!IsWordValid(permutations[i]))
					continue;
					
				int tmpValue = CalculateWordValue(permutations[i]);
				int tmpBestRow = -1, tmpBestCol = -1;

				if (PutWordBasedOnConstraintCharacter(permutations[i], tmpValue, ref tmpBestRow, ref tmpBestCol) == -1)
					continue;
				
				if (maxValue < tmpValue)
				{
					maxValue = tmpValue;
					highestScoreWord = permutations[i];
					bestRow = tmpBestRow;  bestCol = tmpBestCol;
				}
			}
		}


		private void InsertHighestScoreWordInBoard(int bestRow, int bestCol, string highestScoreWord)
		{
			for (int j = 0; j < highestScoreWord.Length; j++)
				scarabeo[bestRow, bestCol++] = highestScoreWord[j];
		}


		private void Permute(char[] letters, int start, int end, ref List<string> permutations)
		{
			if (start == end - 1)
			{
				permutations.Add(new string(letters));
				return;
			}
			
			for (int i = start; i < end; i++)
			{
				Swap(ref letters[start], ref letters[i]);

				Permute(letters, start + 1, end, ref permutations);

				Swap(ref letters[start], ref letters[i]);
			}
		}


		private void Swap(ref char x, ref char y)
		{
			char tmpValue = x;
			x = y;
			y = tmpValue;
		}


		private bool IsWordValid(string combinedLetters)
		{
			return dictionary.Search(combinedLetters);
		}


		// have to save the i, j to be able to place the word
		// in best column
		private int PutWordBasedOnConstraintCharacter(string combinedLetters, int wordValue, ref int bestRow, ref int bestCol)
		{
			int maxValue = -1;

			for (int i = 0, j; i < BOARD_SIZE; i++)
			{
				if (ColumnContainsWord(i))
					continue;

				for (j = 0; j < BOARD_SIZE; j++)
					if (scarabeo[i, j] != '\0')
						break;

				int constraintCharacterIndex = j; 
				char constraintCharacter = scarabeo[i, constraintCharacterIndex];

				if (!combinedLetters.Contains(constraintCharacter))
					continue;

				// foreach duplicate calculate if it can be placed in the board

				/*
					...
				*/

				int start = combinedLetters.IndexOf(constraintCharacter);

				do
				{
					int leftAvaibleSpaceInBoard = constraintCharacterIndex - 1;
					int rightAvaibleSpaceInBoard = (BOARD_SIZE - 1) - constraintCharacterIndex;

					int numberOfLeftCharacters = start - 1;
					int numberOfRightCharacters = (combinedLetters.Length - 1) - start;

					
					// check if constraint character is contained in combinedLetter

					// if true:
					//     check left right, after finding the index of combinerLetters[?] == scarabeo[i, constraintCharacterIndex].

					// else: continue

					
					if (!WordCanBePlacedInColumn(
						leftAvaibleSpaceInBoard, rightAvaibleSpaceInBoard,
						numberOfLeftCharacters, numberOfRightCharacters))
						
						continue;

					// puts the word in the right way in the col of the board ---> example:  {, , , i, , ,} <---- "ciao"
					// 																							  output: {, , c, i, a, o}

					// here i have to compare the points of the word 
					// in each column considering the "bonus".

					if (maxValue < wordValue)
					{
						bestRow = i; bestCol = leftAvaibleSpaceInBoard - numberOfLeftCharacters;
						maxValue = wordValue;
					}

				} while ((start = GetConstraintCharacterIndexInWord(combinedLetters, constraintCharacter, start)) == -1);
			}

			return maxValue;
		}


		private int GetConstraintCharacterIndexInWord(string combinedLetters, char constraintCharacter, int j)
		{
			for (int k = j; k < combinedLetters.Length; k++)
				if (combinedLetters[k] == constraintCharacter)
					return k;

			return -1;
		}


		// Controls if there is enough space before and 
		// after the cell where constraint character is placed.
		// Example:  {, l, , ,}   input: "hello" ---> there is not enough space at the left of 'i'

		private bool WordCanBePlacedInColumn(int leftAvaibleSpaceInBoard, int rightAvaibleSpaceInBoard, 
											 int numberOfLeftCharacters, int numberOfRightCharacters)
		{
			return leftAvaibleSpaceInBoard >= numberOfLeftCharacters &&
				   rightAvaibleSpaceInBoard >= numberOfRightCharacters;
		}

		
		private bool ColumnContainsWord(int y)
		{
			int counter = 0;

			for (int x = 0; x < BOARD_SIZE; x++)
				if (scarabeo[y, x] != '\0')
					counter++;
			
			return counter > 1 ? true : false;
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
				for (int j = 0; j < letterGroups[i].Length; j++)
					if (letterGroups[i][j] == c)
						return points[i];

			return -1;
		}
	}
}