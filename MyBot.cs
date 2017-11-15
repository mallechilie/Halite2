using System;
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
			//Trace.Listeners.Add(new TextWriterTraceListener(string.Format("Halite.{0}.text.log", AppDomain.CurrentDomain.FriendlyName)));
			//Trace.Listeners.Add(new EventLogTraceListener(string.Format("Halite.{0}.event.log", AppDomain.CurrentDomain.FriendlyName)));
			//Trace.Listeners.Add(new ConsoleTraceListener());
			//Trace.AutoFlush = true;
			Collision.CircleIntersectNewPoint(new Position(100, 0), new Position(100, 10), new Entity(0, 0, 97, 7, 100, 5), 0);
			try
			{
				world = new World(args);
				while (true)
					Networking.SendMoves(world.DoTurn());
			}
			catch (Exception ex)
			{
				DebugLog.AddLog("Message :" + ex.Message + Environment.NewLine + "StackTrace :" + ex.StackTrace + "" + Environment.NewLine + "Date :" + DateTime.Now);
				//Trace.TraceError("Message :" + ex.Message + Environment.NewLine + "StackTrace :" + ex.StackTrace + "" + Environment.NewLine + "Date :" + DateTime.Now);
			}
		}
	}
}
