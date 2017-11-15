using System;
using System.Linq;

namespace Halite2.hlt
{
	public static class Navigation
	{
		public static ThrustMove NavigateShipToDock(
			GameMap gameMap,
			Ship ship,
			Entity dockTarget,
			int maxThrust)
		{
			int maxCorrections = Constants.MAX_NAVIGATION_CORRECTIONS;
			bool avoidObstacles = true;
			double angularStepRad = Math.PI / 180.0;
			Position targetPos = ship.GetClosestPoint(dockTarget);

			return NavigateShipTowardsTarget(gameMap, ship, targetPos, maxThrust, avoidObstacles, maxCorrections, angularStepRad);
		}

		public static ThrustMove NavigateShipTowardsTarget(
			GameMap gameMap,
			Ship ship,
			Position targetPos,
			int maxThrust,
			bool avoidObstacles,
			int maxCorrections,
			double angularStepRad)
		{
			if (maxCorrections <= 0)
			{
				return null;
			}

			double distance = ship.GetDistanceTo(targetPos);
			double angleRad = ship.OrientTowardsInRad(targetPos);

			if (avoidObstacles && gameMap.ObjectsBetween(ship, targetPos).Any())
			{
				double newTargetDx = Math.Cos(angleRad + angularStepRad) * distance;
				double newTargetDy = Math.Sin(angleRad + angularStepRad) * distance;
				Position newTarget = new Position(ship.GetXPos() + newTargetDx, ship.GetYPos() + newTargetDy);

				return NavigateShipTowardsTarget(gameMap, ship, newTarget, maxThrust, true, (maxCorrections - 1), angularStepRad);
			}

			int thrust;
			if (distance < maxThrust)
			{
				// Do not round up, since overshooting might cause collision.
				thrust = (int) distance;
			}
			else
			{
				thrust = maxThrust;
			}

			int angleDeg = Util.AngleRadToDegClipped(angleRad);

			return new ThrustMove(ship, angleDeg, thrust);
		}

		public static ThrustMove NavigateShipTowardsTargetCustom(GameMap gameMap, Ship ship, Position target,
		                                                         bool avoidObstacles, double safeZone, double safeZoneToTarget = 0,  Entity[] closeEntities = null)
		{
			if (closeEntities == null)
			{

				closeEntities =
					gameMap.NearbyPlanetsByDistance(ship, e => true).OrderBy(kvp => kvp.Key).Select(kvp => kvp.Value).ToArray();
			}
			if (avoidObstacles)
				for (int x = 0; x < closeEntities.Length; x++)
				{
					Position newPosition = Collision.CircleIntersectNewPoint(ship, target, closeEntities[x], safeZone);
					if (Equals(newPosition, target))
						continue;
					return NavigateShipTowardsTargetCustom(gameMap, ship, newPosition, true, safeZone, 0,
						closeEntities.Take(x + 1).ToArray());
				}

			return GoToTarget(ship, ship.GetClosestPoint(target), safeZoneToTarget);
		} 

		public static ThrustMove GoToTarget(Ship ship, Position newPosition, double safeZone = 0, int minThrust = 0, int maxThrust = 7)
		{
			double distance = ship.GetDistanceTo(newPosition);
			int angle = ship.OrientTowardsInDeg(newPosition);

			int thrust = maxThrust;
			if (distance < maxThrust)
				thrust = (int) (distance - safeZone);
			if (thrust < minThrust)
				thrust = minThrust;

			return new ThrustMove(ship, angle, thrust);
		}
	}
}
