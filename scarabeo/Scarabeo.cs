using System;
using System.IO;

namespace Scarabeo
{
    class Scarabeo
    {
        private const int BOARD_SIZE = 8;
        private char[,] board;


        public Scarabeo()
        {
            board = new char[BOARD_SIZE, BOARD_SIZE];
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



        public void GenerateInputFile()
        {

        }


        public void GenerateCharacters()
        {

        }
    }
}