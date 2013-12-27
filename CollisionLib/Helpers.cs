using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CollisionLib
{
    public class Helpers
    {
        static public float DistanceBetweenTwoPoints(SFML.Window.Vector2f a, SFML.Window.Vector2f b)
        {
            float distanceX = b.X - a.X;
            float distanceY = b.Y - a.Y;

            return (float)Math.Sqrt((Double)(distanceX * distanceX) + (Double)(distanceY * distanceY));
        }
    }
}
