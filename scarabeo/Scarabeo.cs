using System;
using System.IO;
using System.Runtime.CompilerServices;


namespace Scarabeo
{
    class Scarabeo
    {
        public const int BOARD_SIZE = 8;
        private const int FIRST_CHARACTER_ASCII_VALUE = 97;
        private const int LAST_CHARACTER_ASCII_VALUE = 123;
        private char[,] board;
        private static readonly string projectPath = GetProjectPath();


        public Scarabeo()  { board = new char[BOARD_SIZE, BOARD_SIZE]; }


        private static string GetProjectPath()
        {
            return Environment.CurrentDirectory;;
        }


        public char this[int y, int x]
        {
            get
            {
                if (IsSquareOutsideTheBoard(x, y))
                    throw new IndexOutOfRangeException();

                return board[y, x];
            }

            set
            {
                if (IsSquareOutsideTheBoard(x, y))
                    throw new IndexOutOfRangeException();

                board[y, x] = value;
            }
        }



        private bool IsSquareOutsideTheBoard(int x, int y)
        {
            if (x < 0 || x > BOARD_SIZE ||
                y < 0 || y > BOARD_SIZE)
                return true;

            return false;
        }


        public void InitializeScarabeo()
        {
            int j;
            Random rnd = new();

            for (int i = 0; i < BOARD_SIZE; i++)
            {
                j = rnd.Next(0, BOARD_SIZE); 
                board[i, j] = GenerateRandomCharacter(); 
            }
        }


        // generate some characters and bonus
        private char GenerateRandomCharacter()
        {
            Random rnd = new();

            return (char)rnd.Next(FIRST_CHARACTER_ASCII_VALUE, LAST_CHARACTER_ASCII_VALUE);
        }


        public void PrintBoard()
        {
            Console.Write("\n\n");

            for (int i = 0; i < BOARD_SIZE; i++)
            {
                Console.Write("\n");

                for (int j = 0; j < BOARD_SIZE; j++)
                    Console.Write($"{board[i, j]} ");
            }
        }
    }
}