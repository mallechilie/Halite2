using System;
using System.Collections.Generic;
using System.Linq;

namespace Halite2.hlt
{

    public class Entity : Position
    {
	    public List<Ship> DockedShips
	    {
		    get
		    {
			    dockedShips = dockedShips.Where(s => s.GetDockingStatus() != Ship.DockingStatus.Undocked).ToList();
			    return dockedShips;
		    }
	    }

	    private int owner;
        private int id;
        private int health;
        private double radius;
	    private List<Ship> dockedShips;

	    public Entity(int owner, int id, double xPos, double yPos, int health, double radius)
            : base(xPos, yPos)
        {
            this.owner = owner;
            this.id = id;
            this.health = health;
            this.radius = radius;
	        dockedShips = new List<Ship>();
        }

        public int GetOwner()
        {
            return owner;
        }

        public int GetId()
        {
            return id;
        }

        public int GetHealth()
        {
            return health;
        }

        public override double GetRadius()
        {
            return radius;
        }

        public override string ToString()
        {
            return "Entity[" +
                    base.ToString() +
                    ", owner=" + owner +
                    ", id=" + id +
                    ", health=" + health +
                    ", radius=" + radius +
                    "]";
        }

	}
}
