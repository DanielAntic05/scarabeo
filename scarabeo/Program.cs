using System;
using System.Threading;

namespace Scarabeo
{
	class Program
	{
		private static Game game;


		static void Main()
		{
			Thread mainThread = new Thread(() => game = new Game());
			mainThread.Start();

			while (game.IsGameRunning);

			mainThread.Join();
		}
	}

}