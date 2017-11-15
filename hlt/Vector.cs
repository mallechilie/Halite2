using System;

namespace Halite2.hlt
{
	class Vector : Position 
	{
		public Vector(double xPos, double yPos) : base(xPos, yPos)
		{
		}
		public Vector(Position position) : base(position.GetXPos(), position.GetYPos())
		{
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
			return new Vector(a.GetXPos() + b.GetXPos(), a.GetYPos() + b.GetYPos());
		}
		public static Vector operator -(Vector a, Vector b)
		{
			return new Vector(a.GetXPos() - b.GetXPos(), a.GetYPos() - b.GetYPos());
		}
		public static Vector operator -(Vector a)
		{
			return new Vector(-a.GetXPos(), -a.GetYPos());
		}
		public static double operator *(Vector a, Vector b)
		{
			return (a.GetXPos() * b.GetXPos() + a.GetYPos() * b.GetYPos());
		}
		public static Vector operator *(Vector a, double b)
		{
			return new Vector(a.GetXPos() * b, a.GetYPos() * b);
		}
		public static Vector operator *(double b, Vector a)
		{
			return new Vector(a.GetXPos() * b, a.GetYPos() * b);
		}
		public static Vector operator /(Vector a, double b)
		{
			return new Vector(a.GetXPos() / b, a.GetYPos() / b);
		}
	}
}
