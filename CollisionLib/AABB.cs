using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CollisionLib
{
    public class AABB
    {
        public enum AABBSide
        {
            enTop = 0,
            enRight = 1,
            enBottom = 2,
            enLeft = 3
        };

        public LineSegment[] Sides { get; set; }
        public SFML.Window.Vector2f Position { get; set; }
        public SFML.Window.Vector2f Center { get; set; }
        public SFML.Window.Vector2f Extents { get; set; }

        public AABB(SFML.Window.Vector2f position, float width, float height)
        {
            Position = position;
            Center = new SFML.Window.Vector2f(position.X + (width / 2), position.Y + (height / 2));
            Extents = new SFML.Window.Vector2f(width, height);

            Sides = new LineSegment[4];
            Sides[(int)AABBSide.enTop] = new LineSegment(position, new SFML.Window.Vector2f(position.X + width, position.Y));
            Sides[(int)AABBSide.enRight] = new LineSegment(Sides[(int)AABBSide.enTop].End, new SFML.Window.Vector2f(position.X + width, position.Y + height));
            Sides[(int)AABBSide.enBottom] = new LineSegment(Sides[(int)AABBSide.enRight].End, new SFML.Window.Vector2f(position.X, position.Y + height));
            Sides[(int)AABBSide.enLeft] = new LineSegment(Sides[(int)AABBSide.enBottom].End, position);
        }

        public Boolean Overlaps(AABB other)
        {
            return (Math.Abs(Center.X - other.Center.X) * 2 <= (Extents.X + other.Extents.X)) && (Math.Abs(Center.Y - other.Center.Y) * 2 <= (Extents.Y + other.Extents.Y));
        }
    }
}
