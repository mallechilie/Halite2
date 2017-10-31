using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Halite2.hlt;

namespace Halite2
{
	public class World
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

				Entity target = FindTarget(ship);
				if (target is Ship )
					Fight(ship, (Ship) target);
				else if (target is Planet)
					ColonizePlanet(ship, (Planet) target);
			}
			return moveList;
		}

		private void Fight(Ship ship, Ship target)
		{
			Dictionary<double, Ship> nearEnemyShips = gameMap.NearbyShipsByDistance(ship, e => e.GetOwner() != gameMap.GetMyPlayerId() && e.GetDockingStatus()!=Ship.DockingStatus.Undocked);
			if (nearEnemyShips == null || nearEnemyShips.Count == 0)
				nearEnemyShips = gameMap.NearbyShipsByDistance(ship, e => e.GetOwner() != gameMap.GetMyPlayerId());
			Ship closeEnemy = nearEnemyShips.OrderBy(e => e.Key).First().Value;
			closeEnemy = target;

			ThrustMove moveEnemy = closeEnemy.GetDockingStatus() != Ship.DockingStatus.Undocked ?
				                       Navigation.NavigateShipTowardsTargetCustom(gameMap, ship, closeEnemy, true, 1, 4) :
				                       Navigation.NavigateShipTowardsTargetCustom(gameMap, ship, closeEnemy, true, 1);
			moveList.Add(moveEnemy);
		}
		private Entity FindTarget(Ship ship)
		{
			Planet closestPlanet = FindPlanet(ship);
			Planet emptyPlanet = FindEmptyPlanet(ship);
			Ship dockedEnemy = FindDockedEnemy(ship);
			Ship closeEnemy = FindEnemy(ship);

			Dictionary<Entity, double> targets = new Dictionary<Entity, double>();

			if (emptyPlanet != null )
				targets.Add(emptyPlanet, ship.GetDistanceTo(emptyPlanet));
			if (closestPlanet != null&& !Equals(emptyPlanet, closestPlanet))
				targets.Add(closestPlanet, ship.GetDistanceTo(closestPlanet) * 1.5);
			if (dockedEnemy != null)
				targets.Add(dockedEnemy, ship.GetDistanceTo(dockedEnemy));
			if (closeEnemy != null && !Equals(closeEnemy, dockedEnemy))
					targets.Add(closeEnemy, ship.GetDistanceTo(closeEnemy) * 1.5);

			return targets.OrderBy(kvp => kvp.Value).First().Key;
		}
		private Planet FindPlanet(Ship ship)
		{
			Dictionary<double, Planet> colonizable = gameMap.NearbyPlanetsByDistance(ship, planet => (planet.GetOwner() == gameMap.GetMyPlayerId() && !planet.IsFull()) || !planet.IsOwned());

			if (colonizable == null || colonizable.Count == 0)
				return null;

			double closestPlanet = colonizable.Min(kvp => kvp.Key);
			return colonizable[closestPlanet];
		}
		private Planet FindEmptyPlanet(Ship ship)
		{
			Dictionary<double, Planet> colonizable = gameMap.NearbyPlanetsByDistance(ship, planet => !planet.IsOwned());

			if (colonizable == null || colonizable.Count == 0)
				return null;

			double closestPlanet = colonizable.Min(kvp => kvp.Key);
			return colonizable[closestPlanet];
		}
		private Ship FindDockedEnemy(Ship ship)
		{
			Dictionary<double, Ship> nearEnemyShips = gameMap.NearbyShipsByDistance(ship, e => e.GetOwner() != gameMap.GetMyPlayerId() && e.GetDockingStatus() != Ship.DockingStatus.Undocked);
			if (nearEnemyShips == null || nearEnemyShips.Count == 0)
				return null;
			return nearEnemyShips.OrderBy(e => e.Key).First().Value;
		}
		private Ship FindEnemy(Ship ship)
		{
			Dictionary<double, Ship> nearEnemyShips = gameMap.NearbyShipsByDistance(ship, e => e.GetOwner() != gameMap.GetMyPlayerId());
			if (nearEnemyShips == null || nearEnemyShips.Count == 0)
				return null;
			return nearEnemyShips.OrderBy(e => e.Key).First().Value;
		}
		private void ColonizePlanet(Ship ship, Planet target)
		{
			Planet[] planets = gameMap.NearbyPlanetsByDistance(ship, e => true).OrderBy(kvp => kvp.Key).Select(p => p.Value).ToArray();
			//foreach (Planet planet in Planets)
			for (int x = 0; x < planets.Length; x++)
			{
				Planet planet = planets[x];

				if (planet.IsOwned() && planet.GetOwner() != gameMap.GetMyPlayerId())
					continue;
				if (planet.IsFull())
					continue;

				if (ship.CanDock(planet))
				{
					moveList.Add(new DockMove(ship, planet));
					return;
				}
				if (Equals(planet, target))
					break;
			}
			ThrustMove newThrustMove = Navigation.NavigateShipToDock(gameMap, ship, target, Constants.MAX_SPEED);
			if (newThrustMove != null)
			{
				moveList.Add(newThrustMove);
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
