using System;
using System.Threading;

namespace Scarabeo
{
	class Program
	{
		static void Main()
		{
			Thread mainThread = new Thread(() => new Game());
			mainThread.Start();
		}
	}

}