using System.Diagnostics;
using System.Text;
using Halite2.hlt;

namespace Halite2
{
    public static class MyBot
    {
	    private static World world;

		public static void Main(string[] args)
		{
			//Trace.Listeners.Add(new TextWriterTraceListener(string.Format("Halite.{0}.text.log", System.AppDomain.CurrentDomain.FriendlyName)));
			//Trace.Listeners.Add(new EventLogTraceListener(string.Format("Halite.{0}.event.log", System.AppDomain.CurrentDomain.FriendlyName)));
			//Trace.Listeners.Add(new ConsoleTraceListener());
			//Trace.AutoFlush = true;
			//Trace.TraceError("msg");
			Collision.CircleIntersectNewPoint(new Position(100, 0), new Position(100, 10), new Entity(0, 0, 97, 7, 100, 5), 0);
			world = new World(args);
			while (true)
				Networking.SendMoves(world.DoTurn());
		}
	}
}
