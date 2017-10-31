using System;

namespace Halite2.hlt
{
    public class ThrustMove : Move
    {

        private int angleDeg;
        private int thrust;

        public ThrustMove(Ship ship, int angleDeg, int thrust)
            : base(MoveType.Thrust, ship)
        {
            this.thrust = thrust;
            this.angleDeg = angleDeg;
        }
	    public ThrustMove(Ship ship, Position position, int thrust = 7)
			: base(MoveType.Thrust, ship)
	    {
		    position = ship.GetClosestPoint(position);
            this.thrust = Math.Min((int)ship.GetDistanceTo(position), thrust);
		    angleDeg = Util.AngleRadToDegClipped(ship.OrientTowardsInRad(position));
	    }

        public int GetAngle()
        {
            return angleDeg;
        }

        public int GetThrust()
        {
            return thrust;
        }
    }
}
