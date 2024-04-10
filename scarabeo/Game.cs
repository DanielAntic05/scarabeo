using System;

namespace Scarabeo
{
	class Game
	{
		private Scarabeo scarabeo;
		public bool IsGameRunning { get; private set; } = true;


		public Game() 
		{
			scarabeo = new Scarabeo();
			scarabeo.InitializeScarabeo();
		}


	}
}