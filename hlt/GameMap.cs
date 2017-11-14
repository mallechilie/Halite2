using System;
using System.Collections.Generic;
using System.Linq;

namespace Halite2.hlt {

    public class GameMap {
		public int[] Ships { get; private set; }
        private int width, height;
        private int playerId;
        private List<Player> players;
        private IList<Player> playersUnmodifiable;
        private Dictionary<int, Planet> planets;
        private List<Ship> allShips;
        private IList<Ship> allShipsUnmodifiable;

        // used only during parsing to reduce memory allocations
        private List<Ship> currentShips = new List<Ship>();

        public GameMap(int width, int height, int playerId) {
            this.width = width;
            this.height = height;
            this.playerId = playerId;
            players = new List<Player>(Constants.MAX_PLAYERS);
            playersUnmodifiable = players.AsReadOnly();
            planets = new Dictionary<int, Planet>();
            allShips = new List<Ship>();
            allShipsUnmodifiable = allShips.AsReadOnly();
        }

        public int GetHeight() {
            return height;
        }

        public int GetWidth() {
            return width;
        }

        public int GetMyPlayerId() {
            return playerId;
        }

        public IList<Player> GetAllPlayers() {
            return playersUnmodifiable;
        }

        public Player GetMyPlayer() => playersUnmodifiable[GetMyPlayerId()];

        public Ship GetShip(int playerId, int entityId) {
            return players[playerId].GetShip(entityId);
        }

        public Planet GetPlanet(int entityId) {
            return planets[entityId];
        }

        public Dictionary<int, Planet> GetAllPlanets() {
            return planets;
        }

        public IList<Ship> GetAllShips() {
            return allShipsUnmodifiable;
        }

        public List<Entity> ObjectsBetween(Position start, Position target) {
            List<Entity> entitiesFound = new List<Entity>();

            AddEntitiesBetween(entitiesFound, start, target, planets.Values.ToList<Entity>());
            AddEntitiesBetween(entitiesFound, start, target, allShips.ToList<Entity>());

            return entitiesFound;
        }

        private static void AddEntitiesBetween(List<Entity> entitiesFound,
                                               Position start, Position target,
                                               ICollection<Entity> entitiesToCheck) {

            foreach (Entity entity in entitiesToCheck) {
                if (entity.Equals(start) || entity.Equals(target)) {
                    continue;
                }
                if (Collision.SegmentCircleIntersect(start, target, entity, Constants.FORECAST_FUDGE_FACTOR)) {
                    entitiesFound.Add(entity);
                }
            }
        }

	    public Dictionary<double, Entity> NearbyEntitiesByDistance(Entity entity, Func<Entity, bool> filter = null, Func<Entity, Entity, double> distanceFunction = null)
		{
			Dictionary<double, Entity> entityByDistance = new Dictionary<double, Entity>();

			IEnumerable<Planet> filteredPlanets = filter == null ? planets.Values : planets.Values.Where(e => filter(e));
			foreach (Planet planet in filteredPlanets)
			{
				if (planet.Equals(entity))
				{
					continue;
				}
				if (distanceFunction == null)
					entityByDistance[entity.GetDistanceTo(planet)] = planet;
				else
					entityByDistance[distanceFunction(entity, planet)] = planet;

			}

			IEnumerable<Ship> filteredShips = filter == null ? GetAllShips() : GetAllShips().Where(e => filter(e));
			foreach (Ship ship in filteredShips)
			{
				if (ship.Equals(entity))
				{
					continue;
				}
				if (distanceFunction == null)
					entityByDistance[entity.GetDistanceTo(ship)] = ship;
				else
					entityByDistance[distanceFunction(entity, ship)] = ship;
			}

			return entityByDistance;
		}
		public Dictionary<double, Ship> NearbyShipsByDistance(Entity entity, Func<Ship, bool> filter = null, Func<Entity, Ship, double> distanceFunction = null)
		{
			Dictionary<double, Ship> entityByDistance = new Dictionary<double, Ship>();
			IEnumerable<Ship> filteredShips = filter == null ? GetAllShips() : GetAllShips().Where(e => filter(e));

			foreach (Ship ship in filteredShips)
			{
				if (ship.Equals(entity))
				{
					continue;
				}
				if (distanceFunction == null)
					entityByDistance[entity.GetDistanceTo(ship)] = ship;
				else
					entityByDistance[distanceFunction(entity, ship)] = ship;
			}

			return entityByDistance;
		}
		public Dictionary<double, Planet> NearbyPlanetsByDistance(Entity entity, Func<Planet, bool> filter = null, Func<Entity, Planet, double> distanceFunction = null)
		{
			Dictionary<double, Planet> entityByDistance = new Dictionary<double, Planet>();
			IEnumerable<Planet> filteredPlanets = filter == null ? planets.Values : planets.Values.Where(e=> filter(e));

			foreach (Planet planet in filteredPlanets)
			{
				if (planet.Equals(entity))
				{
					continue;
				}
				if (distanceFunction == null)
					entityByDistance[entity.GetDistanceTo(planet)] = planet;
				else
					entityByDistance[distanceFunction(entity, planet)] = planet;
			}

			return entityByDistance;
		}

		public GameMap UpdateMap(Metadata mapMetadata) {
            int numberOfPlayers = MetadataParser.ParsePlayerNum(mapMetadata);

            players.Clear();
            planets.Clear();
            allShips.Clear();

            // update players info
            for (int i = 0; i < numberOfPlayers; ++i) {
                currentShips.Clear();
                Dictionary<int, Ship> currentPlayerShips = new Dictionary<int, Ship>();
                int playerId = MetadataParser.ParsePlayerId(mapMetadata);

                Player currentPlayer = new Player(playerId, currentPlayerShips);
                MetadataParser.PopulateShipList(currentShips, playerId, mapMetadata);
                allShips.AddRange(currentShips);

                foreach (Ship ship in currentShips) {
                    currentPlayerShips[ship.GetId()] = ship;
                }
                players.Add(currentPlayer);
            }

            int numberOfPlanets = int.Parse(mapMetadata.Pop());

            for (int i = 0; i < numberOfPlanets; ++i) {
                List<int> dockedShips = new List<int>();
                Planet planet = MetadataParser.NewPlanetFromMetadata(dockedShips, mapMetadata);
                planets[planet.GetId()] = planet;
            }

            if (!mapMetadata.IsEmpty()) {
                throw new InvalidOperationException("Failed to parse data from Halite game engine. Please contact maintainers.");
            }

            return this;
        }
    }

}
