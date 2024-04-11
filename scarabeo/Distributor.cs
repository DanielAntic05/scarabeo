using System;
using System.Reflection.Metadata;

namespace Scarabeo
{
    class Distributor
    {
        private const int N_CHARACTERS = 8;

        private static readonly string[] letters =
        {
            "aeio",
            "crst",
            "lmn",
            "bdfgpuv",
            "hqz"
        };

        private static readonly int[] lettersProbability = {12, 7, 6, 4, 2};


        public static string GenerateRandomLetters()
        {
            string result = "";

            Random rand = new Random();

            for (int i = 0; i < N_CHARACTERS; i++)
            {
                int totalFrequency = 0;

                for (int j = 0; j < letters.Length; j++)
                    for (int k = 0; k < letters[j].Length; k++)
                        totalFrequency += lettersProbability[j];

                int randomNumber = rand.Next(1, totalFrequency + 1);
                int cumulativeFrequency = 0;

                for (int j = 0; j < letters.Length)
                {
                    for (int k = 0; k < letters[j].Length; k++)
                        cumulativeFrequency += lettersProbability[j];

                    if (randomNumber <= cumulativeFrequency)
                    {
                        result += kvp.Key;
                        break;
                    }
                }
            }            

            return result;
        }
    }
}