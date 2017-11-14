using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Halite2.hlt
{
	public class Vector  
	{
		public double xPos { get; private set; }
		public double yPos { get; private set; }

		public Vector(double xPos, double yPos)
		{
			this.xPos = xPos;
			this.yPos = yPos;
		}
		public Vector(Position position)
		{
			xPos = position.GetXPos();
			yPos = position.GetYPos();
		}


		public double LengthSquared()
		{
			return this * this;
		}
		public double Length()
		{
			return Math.Sqrt(LengthSquared());
		}

		public static Vector operator +(Vector a, Vector b)
		{
			return new Vector(a.xPos + b.xPos, a.yPos + b.yPos);
		}
		public static Vector operator -(Vector a, Vector b)
		{
			return new Vector(a.xPos - b.xPos, a.yPos - b.yPos);
		}
		public static Vector operator -(Vector a)
		{
			return new Vector(-a.xPos, -a.yPos);
		}
		public static double operator *(Vector a, Vector b)
		{
			return a.xPos * b.xPos + a.yPos * b.yPos;
		}
		public static Vector operator *(Vector a, double b)
		{
			return new Vector(a.xPos * b, a.yPos * b);
		}
		public static Vector operator *(double b, Vector a)
		{
			return new Vector(a.xPos * b, a.yPos * b);
		}
		public static Vector operator /(Vector a, double b)
		{
			return new Vector(a.xPos / b, a.yPos / b);
		}
		public static implicit operator Position(Vector v)
		{
			return new Position(v.xPos,v.yPos);
		}
		public static explicit operator Vector(Position p)
		{
			return new Vector(p);
		}
	}
}
