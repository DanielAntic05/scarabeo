using System;
using System.Threading;

namespace Scarabeo
{
	class Program
	{
		private static Game game = new();


		static void Main()
		{
			Thread mainThread = new Thread(() => game.InitializeGame());
			mainThread.Start();

			while (game.IsGameRunning);

			mainThread.Join();
		}
	}

}