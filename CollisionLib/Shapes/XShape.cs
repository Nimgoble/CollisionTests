using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFML.Window;
using SFML.Graphics;

namespace CollisionLib.Shapes
{
    public class XShape : Drawable
    {
        private LineSegment[] crossLines;
        public XShape(Vector2f center, float lineLength)
        {
            crossLines = new LineSegment[]
            {
                new LineSegment( new SFML.Window.Vector2f(center.X - lineLength, center.Y - lineLength), 
                                    new SFML.Window.Vector2f(center.X + lineLength, center.Y + lineLength),
                                    Color.Red),
                new LineSegment( new SFML.Window.Vector2f(center.X + lineLength, center.Y - lineLength),
                                    new SFML.Window.Vector2f(center.X - lineLength, center.Y + lineLength),
                                    Color.Red)
            };
        }

        public XShape(Vector2f center, float lineLength, Color color)
        {
            crossLines = new LineSegment[]
            {
                new LineSegment( new SFML.Window.Vector2f(center.X - lineLength, center.Y - lineLength), 
                                    new SFML.Window.Vector2f(center.X + lineLength, center.Y + lineLength),
                                    color),
                new LineSegment( new SFML.Window.Vector2f(center.X + lineLength, center.Y - lineLength),
                                    new SFML.Window.Vector2f(center.X - lineLength, center.Y + lineLength),
                                    color)
            };
        }

        public void Draw(RenderTarget target, RenderStates states)
        {
            foreach (LineSegment segment in crossLines)
                target.Draw(segment, states);
        }
    }
}
