using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VectorClass;

namespace CollisionLineTests
{
    class AABB
    {
        public enum AABBSide
        {
            enTop = 0,
            enRight = 1,
            enBottom = 2,
            enLeft = 3
        };

        public LineSegment[] Sides { get; set; }
        public Vector2D_Dbl Position { get; set; }
        public Vector2D_Dbl Center { get; set; }
        public Vector2D_Dbl Extents { get; set; }

        public AABB(Vector2D_Dbl position, Double width, Double height)
        {
            Position = position;
            Center = new Vector2D_Dbl(position.X + (width / 2), position.Y + (height / 2));
            Extents = new Vector2D_Dbl(width, height);

            Sides = new LineSegment[4];
            Sides[(int)AABBSide.enTop] = new LineSegment(position, new Vector2D_Dbl(position.X + width, position.Y));
            Sides[(int)AABBSide.enRight] = new LineSegment(Sides[(int)AABBSide.enTop].End, new Vector2D_Dbl(position.X + width, position.Y + height));
            Sides[(int)AABBSide.enBottom] = new LineSegment(Sides[(int)AABBSide.enRight].End, new Vector2D_Dbl(position.X, position.Y + height));
            Sides[(int)AABBSide.enLeft] = new LineSegment(Sides[(int)AABBSide.enBottom].End, position);
        }

        public Boolean Overlaps(AABB other)
        {
            return (Math.Abs(Center.X - other.Center.X) * 2 < (Extents.X + other.Extents.X)) && (Math.Abs(Center.Y - other.Center.Y) * 2 < (Center.Y + other.Center.Y));
        }
    }
}
