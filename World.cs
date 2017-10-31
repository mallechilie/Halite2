using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Halite2.hlt;

namespace Halite2
{
	{
		GameMap gameMap;
		List<Move> moveList;


		public World(string[] args)
		{
			string name = args.Length > 0 ? args[0] : "Sharpie";

			Networking networking = new Networking();
			gameMap = networking.Initialize(name);
			moveList = new List<Move>();
		}


		public List<Move> DoTurn()
		{
			StartTurn();
			foreach (Ship ship in gameMap.GetMyPlayer().GetShips().Values)
			{
				if (ship.GetDockingStatus() == Ship.DockingStatus.Docked)
					StartUndocking(ship);

				if (ship.GetDockingStatus() == Ship.DockingStatus.Docking || ship.GetDockingStatus() == Ship.DockingStatus.Undocking)
					continue;

				if (Colonize(ship))
					ColonizePlanet(ship);
				else
					Fight(ship);
			}
			return moveList;
		}

		private void Fight(Ship ship)
		{
			Dictionary<double, Ship> nearEnemyShips = gameMap.NearbyShipsByDistance(ship, e => e.GetOwner() != gameMap.GetMyPlayerId());
			Ship closeEnemy = nearEnemyShips.OrderBy(e => e.Key).First().Value;
			ThrustMove moveEnemy = Navigation.NavigateShipTowardsTargetCustom(gameMap, ship, closeEnemy, true, 5);
			moveList.Add(moveEnemy);
		}
		private bool Colonize(Ship ship)
		{
			Dictionary<double, Planet> colonizable = gameMap.NearbyPlanetsByDistance(ship, 
				planet => (planet.GetOwner() == gameMap.GetMyPlayerId() && !planet.IsFull()) || !planet.IsOwned());
			if (colonizable == null || colonizable.Count == 0)
				return false;
			double closestPlanet = colonizable.Min(kvp => kvp.Key);
			double closestShip = gameMap.NearbyShipsByDistance(ship, s => s.GetOwner() != gameMap.GetMyPlayerId()).Min(kvp => kvp.Key);
			return closestPlanet < closestShip;
		}
		private void ColonizePlanet(Ship ship)
		{
			var planets = gameMap.NearbyPlanetsByDistance(ship, e => true).OrderBy(kvp => kvp.Key).Select(p => p.Value).ToArray();
			//foreach (Planet planet in Planets)
			for (int x = 0; x < planets.Length; x++)
			{
				Planet planet = planets[x];

				if (planet.IsOwned() && planet.GetOwner() != gameMap.GetMyPlayerId())
				{
					continue;
				}
				if (planet.IsFull())
				{
					continue;
				}

				if (ship.CanDock(planet))
				{
					moveList.Add(new DockMove(ship, planet));
					break;
				}

				ThrustMove newThrustMove = Navigation.NavigateShipToDock(gameMap, ship, planet, Constants.MAX_SPEED);
				if (newThrustMove != null)
				{
					moveList.Add(newThrustMove);
					break;
				}

			}
		}
		private void StartUndocking(Ship ship)
		{
			if (false)
				moveList.Add(new UndockMove(ship));
		}
		private void StartTurn()
		{
			moveList.Clear();
			gameMap.UpdateMap(Networking.ReadLineIntoMetadata());
		}
	}
}
