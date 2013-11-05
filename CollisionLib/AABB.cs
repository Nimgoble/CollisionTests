using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFML.Graphics;

namespace CollisionLib
{
    public class AABB : SFML.Graphics.Drawable
    {
        public enum AABBSide
        {
            enTop = 0,
            enRight = 1,
            enBottom = 2,
            enLeft = 3
        };

        public static AABBSide GetOppositeSide(AABBSide side)
        {
            switch (side)
            {
                case AABBSide.enTop:
                    return AABBSide.enBottom;
                case AABBSide.enBottom:
                    return AABBSide.enTop;
                case AABBSide.enLeft:
                    return AABBSide.enRight;
                case AABBSide.enRight:
                    return AABBSide.enLeft;
            }
            //Should never get here.
            throw new Exception("Side does not have an opposite!");
        }

        private SFML.Graphics.Color color;
        public SFML.Graphics.Color Color 
        {
            get { return color; }
            set
            {
                color = value;
                foreach(LineSegment segment in Sides)
                {
                    segment.SetColor(color);
                }
            }
        }

        public LineSegment[] Sides { get; set; }
        public SFML.Window.Vector2f Position { get; set; }
        public SFML.Window.Vector2f Center { get; set; }
        public SFML.Window.Vector2f Extents { get; set; }

        public AABB(SFML.Window.Vector2f position, float width, float height)
        {
            Position = position;
            Center = new SFML.Window.Vector2f(position.X + (width / 2), position.Y + (height / 2));
            Extents = new SFML.Window.Vector2f(width, height);

            this.color = Color.White;

            Sides = new LineSegment[4];
            Sides[(int)AABBSide.enTop] = new LineSegment(position, new SFML.Window.Vector2f(position.X + width, position.Y));
            Sides[(int)AABBSide.enRight] = new LineSegment(Sides[(int)AABBSide.enTop].End, new SFML.Window.Vector2f(position.X + width, position.Y + height));
            Sides[(int)AABBSide.enBottom] = new LineSegment(Sides[(int)AABBSide.enRight].End, new SFML.Window.Vector2f(position.X, position.Y + height));
            Sides[(int)AABBSide.enLeft] = new LineSegment(Sides[(int)AABBSide.enBottom].End, position);
        }

        public AABB(SFML.Window.Vector2f position, float width, float height, SFML.Graphics.Color color)
        {
            Position = position;
            Center = new SFML.Window.Vector2f(position.X + (width / 2), position.Y + (height / 2));
            Extents = new SFML.Window.Vector2f(width, height);

            this.color = color;

            Sides = new LineSegment[4];
            Sides[(int)AABBSide.enTop] = new LineSegment(position, new SFML.Window.Vector2f(position.X + width, position.Y), color);
            Sides[(int)AABBSide.enRight] = new LineSegment(Sides[(int)AABBSide.enTop].End, new SFML.Window.Vector2f(position.X + width, position.Y + height), color);
            Sides[(int)AABBSide.enBottom] = new LineSegment(Sides[(int)AABBSide.enRight].End, new SFML.Window.Vector2f(position.X, position.Y + height), color);
            Sides[(int)AABBSide.enLeft] = new LineSegment(Sides[(int)AABBSide.enBottom].End, position, color);
        }

        public Boolean Overlaps(AABB other)
        {
            return (Math.Abs(Center.X - other.Center.X) * 2 <= (Extents.X + other.Extents.X)) && (Math.Abs(Center.Y - other.Center.Y) * 2 <= (Extents.Y + other.Extents.Y));
        }

        public void Draw(RenderTarget target, RenderStates states)
        {
            foreach (LineSegment segment in Sides)
            {
                target.Draw(segment, states);
            }
        }
    }
}
