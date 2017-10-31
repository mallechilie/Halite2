using System;

namespace Halite2.hlt
{
    public class Collision
    {
        /**
         * Test whether a given line segment intersects a circular area.
         *
         * @param start  The start of the segment.
         * @param end    The end of the segment.
         * @param circle The circle to test against.
         * @param fudge  An additional safety zone to leave when looking for collisions. (Probably set it to ship radius 0.5)
         * @return true if the segment intersects, false otherwise
         */
        public static bool SegmentCircleIntersect(Position start, Position end, Entity circle, double fudge)
        {
            // Parameterize the segment as start + t * (end - start),
            // and substitute into the equation of a circle
            // Solve for t
            double circleRadius = circle.GetRadius();
            double startX = start.GetXPos();
            double startY = start.GetYPos();
            double endX = end.GetXPos();
            double endY = end.GetYPos();
            double centerX = circle.GetXPos();
            double centerY = circle.GetYPos();
            double dx = endX - startX;
            double dy = endY - startY;

            double a = Square(dx) + Square(dy);

            double b = -2 * (Square(startX) - (startX * endX)
                                - (startX * centerX) + (endX * centerX)
                                + Square(startY) - (startY * endY)
                                - (startY * centerY) + (endY * centerY));

            if (a == 0.0)
            {
                // Start and end are the same point
                return start.GetDistanceTo(circle) <= circleRadius + fudge;
            }

            // Time along segment when closest to the circle (vertex of the quadratic)
            double t = Math.Min(-b / (2 * a), 1.0);
            if (t < 0)
            {
                return false;
            }

            double closestX = startX + dx * t;
            double closestY = startY + dy * t;
            double closestDistance = new Position(closestX, closestY).GetDistanceTo(circle);

            return closestDistance <= circleRadius + fudge;
        }

	    public static Position CircleIntersectNewPoint(Position start, Position end, Entity circle, double safeZone)
	    {
		    Vector s = new Vector(start);
		    Vector e = new Vector(end) ;
		    Vector c = new Vector(circle);
		    Vector se = e - s;
		    Vector sc = c - s;
		    Vector mid = (sc * se) / se.LengthSquared() * se + s;
		    if ((mid - c).LengthSquared() > (circle.GetRadius() + safeZone) * (circle.GetRadius() + safeZone ) ) 
			    return end;
		    if ((sc * se) / se.LengthSquared() < 0 ||
		        (sc * se) / se.LengthSquared() > 1)
			    return end;
		    Vector direction = (mid - c) / (mid - c).Length();
			Vector newPoint = direction * (circle.GetRadius() + safeZone + 1 - (mid - c).Length()) + mid;
		    return newPoint;
	    }

        public static double Square(double num)
        {
            return num * num;
        }
    }
}
