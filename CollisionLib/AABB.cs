using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFML.Graphics;
using SFML.Window;

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
        private Vector2f position;
        public SFML.Window.Vector2f Position 
        {
            get
            {
                return position;
            }
            set
            {
                position = value;
                Configure();
            }
        }
        public SFML.Window.Vector2f Center { get; set; }
        public SFML.Window.Vector2f Extents { get; set; }

        public AABB(SFML.Window.Vector2f position, float width, float height)
        {
            Position = position;
            Extents = new SFML.Window.Vector2f(width, height);

            this.color = Color.White;

            Configure();
        }

        public AABB(SFML.Window.Vector2f position, float width, float height, SFML.Graphics.Color color)
        {
            Position = position;
            Extents = new SFML.Window.Vector2f(width, height);
            this.color = color;

            Configure();
        }

        public AABB(SFML.Window.Vector2f position, Vector2f extents)
        {
            Position = position;
            Extents = extents;

            this.color = Color.White;

            Configure();
        }

        public AABB(SFML.Window.Vector2f position, Vector2f extents, Color color)
        {
            Position = position;
            Extents = extents;

            this.color = color;

            Configure();
        }

        public void Move(Vector2f movement)
        {
            this.Position += movement;
            Configure();
        }

        private void Configure()
        {
            Center = new SFML.Window.Vector2f(position.X + (Extents.X / 2), position.Y + (Extents.Y / 2));

            Sides = new LineSegment[4];
            Sides[(int)AABBSide.enTop] = new LineSegment(Position, new SFML.Window.Vector2f(Position.X + Extents.X, Position.Y), color);
            Sides[(int)AABBSide.enRight] = new LineSegment(Sides[(int)AABBSide.enTop].End, new SFML.Window.Vector2f(Position.X + Extents.X, Position.Y + Extents.Y), color);
            Sides[(int)AABBSide.enBottom] = new LineSegment(Sides[(int)AABBSide.enRight].End, new SFML.Window.Vector2f(Position.X, Position.Y + Extents.Y), color);
            Sides[(int)AABBSide.enLeft] = new LineSegment(Sides[(int)AABBSide.enBottom].End, Position, color);
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
