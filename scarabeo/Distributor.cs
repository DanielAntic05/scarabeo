using System;
using System.Reflection.Metadata;

namespace Scarabeo
{
    class Distributor
    {
        private const int N_CHARACTERS = 21;
        private const int TOTAL_FREQUENCY = 128;

        private static readonly string[] letterGroups =
        {
            "aeio",
            "crst",
            "lmn",
            "bdfgpuv",
            "hqz"
        };

        private static readonly int[] lettersProbability = {12, 7, 6, 4, 2};


        public static string GenerateRandomLetters(int numberOfLettersToGenerate)
        {
            string result = "";

            Random rand = new Random();

            for (int i = 0; i < numberOfLettersToGenerate; i++)
            {
                int randomNumber = rand.Next(1, TOTAL_FREQUENCY + 1);
                int cumulativeFrequency = 0;

                int j = 0, k = 0;

                while (j < N_CHARACTERS)
                {
                    cumulativeFrequency += lettersProbability[j];

                    if (randomNumber <= cumulativeFrequency)
                    {
                        result += letterGroups[j][k];
                        break;
                    }

                    if (k == letterGroups[j].Length - 1)
                    {
                        j++;
                        k = 0;
                    }
                    else
                        k++;
                }
            }            

            return result;
        }
    }
}