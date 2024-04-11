using System;
using System.IO;
using System.Runtime.CompilerServices;


namespace Scarabeo
{
    class Scarabeo
    {
        private const int BOARD_SIZE = 8;
        private char[,] board;
        private static readonly string projectPath = GetProjectPath();


        public Scarabeo()
        {
            board = new char[BOARD_SIZE, BOARD_SIZE];

        }


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
            InitializeMatrix();
            GenerateInputFile();
        }


        // generate some characters and bonus
        private void InitializeMatrix() 
        {

        }



        private void GenerateInputFile()
        {
            try
            {
                
            }
            catch (System.Exception)
            {
                
                throw;
            }
            finally
            {

            }
        }


        private void GenerateCharacters()
        {

        }
    }
}