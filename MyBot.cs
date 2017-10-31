using Halite2.hlt;

namespace Halite2
{
    public static class MyBot
    {
	    private static World world;

		public static void Main(string[] args)
		{
			Collision.CircleIntersectNewPoint(new Position(100, 0), new Position(100, 10), new Entity(0, 0, 97, 7, 100, 5), 0);
			world = new World(args);
			while (true)
				Networking.SendMoves(world.DoTurn());
		}
	}
}
