using System;
using System.Data;
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using SFML.Graphics;

namespace CollisionLib
{
    public class LineSegment : SFML.Graphics.Drawable
    {
        private static float MyEpsilon = 0.00001f;
        private VertexArray linePoints;

        public SFML.Window.Vector2f Start { get; set; }
        public SFML.Window.Vector2f End { get; set; }
        /// <summary>
        /// Initializes a new instance of the LineSegment class.
        /// </summary>
        public LineSegment()
        {
        }

        public LineSegment(SFML.Window.Vector2f start, SFML.Window.Vector2f end)
        {
            Start = start;
            End = end;

            linePoints = new VertexArray(PrimitiveType.Lines);
            linePoints.Append(new Vertex(start));
            linePoints.Append(new Vertex(end));
        }

        public LineSegment(SFML.Window.Vector2f start, SFML.Window.Vector2f end, SFML.Graphics.Color color)
        {
            Start = start;
            End = end;

            linePoints = new VertexArray(PrimitiveType.Lines);
            linePoints.Append(new Vertex(start) { Color = color });
            linePoints.Append(new Vertex(end) { Color = color });
        }

        public void SetColor(SFML.Graphics.Color color)
        {
            linePoints.Clear();
            linePoints.Append(new Vertex(Start) { Color = color });
            linePoints.Append(new Vertex(End) { Color = color });
        }

        public bool CollidesWith(LineSegment other, out SFML.Window.Vector2f[] collisionPoints)
        {
            collisionPoints = Intersection(Start, End, other.Start, other.End);
            return collisionPoints.Length > 0;
        }

        public void Draw(RenderTarget target, RenderStates states)
        {
            target.Draw(linePoints, states);
        }

        #region Intersection stuff
        private static float[] OverlapIntervals(float ub1, float ub2)
        {
            float l = Math.Min(ub1, ub2);
            float r = Math.Max(ub1, ub2);
            float A = Math.Max(0, l);
            float B = Math.Min(1, r);
            if (A > B) // no intersection
                return new float[] { };
            else if (A == B)
                return new float[] { A };
            else // if (A < B)
                return new float[] { A, B };
        }

        // IMPORTANT: a1 and a2 cannot be the same, e.g. a1--a2 is a true segment, not a point
        // b1/b2 may be the same (b1--b2 is a point)
        private static SFML.Window.Vector2f[] OneD_Intersection(SFML.Window.Vector2f a1, SFML.Window.Vector2f a2, SFML.Window.Vector2f b1, SFML.Window.Vector2f b2)
        {
            //float ua1 = 0.0f; // by definition
            //float ua2 = 1.0f; // by definition
            float ub1, ub2;

            float denomx = a2.X - a1.X;
            float denomy = a2.Y - a1.Y;

            if (Math.Abs(denomx) > Math.Abs(denomy))
            {
                ub1 = (b1.X - a1.X) / denomx;
                ub2 = (b2.X - a1.X) / denomx;
            }
            else
            {
                ub1 = (b1.Y - a1.Y) / denomy;
                ub2 = (b2.Y - a1.Y) / denomy;
            }

            List<SFML.Window.Vector2f> ret = new List<SFML.Window.Vector2f>();
            float[] interval = OverlapIntervals(ub1, ub2);
            foreach (float f in interval)
            {
                float x = a2.X * f + a1.X * (1.0f - f);
                float y = a2.Y * f + a1.Y * (1.0f - f);
                SFML.Window.Vector2f p = new SFML.Window.Vector2f(x, y);
                ret.Add(p);
            }
            return ret.ToArray();
        }

        private static bool PointOnLine(SFML.Window.Vector2f p, SFML.Window.Vector2f a1, SFML.Window.Vector2f a2)
        {
            float dummyU = 0.0f;
            float d = DistFromSeg(p, a1, a2, MyEpsilon, ref dummyU);
            return d < MyEpsilon;
        }

        private static float DistFromSeg(SFML.Window.Vector2f p, SFML.Window.Vector2f q0, SFML.Window.Vector2f q1, float radius, ref float u)
        {
            // formula here:
            //http://mathworld.wolfram.com/Point-LineDistance2-Dimensional.html
            // where x0,y0 = p
            //       x1,y1 = q0
            //       x2,y2 = q1
            float dx21 = q1.X - q0.X;
            float dy21 = q1.Y - q0.Y;
            float dx10 = q0.X - p.X;
            float dy10 = q0.Y - p.Y;
            float segLength = (float)Math.Sqrt((double)dx21 * dx21 + dy21 * dy21);
            if (segLength < MyEpsilon)
                throw new Exception("Expected line segment, not point.");
            float num = Math.Abs(dx21 * dy10 - dx10 * dy21);
            float d = num / segLength;
            return d;
        }

        // this is the general case. Really really general
        public static SFML.Window.Vector2f[] Intersection(SFML.Window.Vector2f a1, SFML.Window.Vector2f a2, SFML.Window.Vector2f b1, SFML.Window.Vector2f b2)
        {
            if (a1.Equals(a2) && b1.Equals(b2))
            {
                // both "segments" are points, return either point
                if (a1.Equals(b1))
                    return new SFML.Window.Vector2f[] { a1 };
                else // both "segments" are different points, return empty set
                    return new SFML.Window.Vector2f[] { };
            }
            else if (b1.Equals(b2)) // b is a point, a is a segment
            {
                if (PointOnLine(b1, a1, a2))
                    return new SFML.Window.Vector2f[] { b1 };
                else
                    return new SFML.Window.Vector2f[] { };
            }
            else if (a1.Equals(a2)) // a is a point, b is a segment
            {
                if (PointOnLine(a1, b1, b2))
                    return new SFML.Window.Vector2f[] { a1 };
                else
                    return new SFML.Window.Vector2f[] { };
            }

            // at this point we know both a and b are actual segments

            float ua_t = (b2.X - b1.X) * (a1.Y - b1.Y) - (b2.Y - b1.Y) * (a1.X - b1.X);
            float ub_t = (a2.X - a1.X) * (a1.Y - b1.Y) - (a2.Y - a1.Y) * (a1.X - b1.X);
            float u_b = (b2.Y - b1.Y) * (a2.X - a1.X) - (b2.X - b1.X) * (a2.Y - a1.Y);

            // Infinite lines intersect somewhere
            if (!(-MyEpsilon < u_b && u_b < MyEpsilon))   // e.g. u_b != 0.0
            {
                float ua = ua_t / u_b;
                float ub = ub_t / u_b;
                if (0.0f <= ua && ua <= 1.0f && 0.0f <= ub && ub <= 1.0f)
                {
                    // Intersection
                    return new SFML.Window.Vector2f[] {
                    new SFML.Window.Vector2f(a1.X + ua * (a2.X - a1.X),
                        a1.Y + ua * (a2.Y - a1.Y)) };
                }
                else
                {
                    // No Intersection
                    return new SFML.Window.Vector2f[] { };
                }
            }
            else // lines (not just segments) are parallel or the same line
            {
                // Coincident
                // find the common overlapping section of the lines
                // first find the distance (squared) from one point (a1) to each point
                if ((-MyEpsilon < ua_t && ua_t < MyEpsilon)
                   || (-MyEpsilon < ub_t && ub_t < MyEpsilon))
                {
                    if (a1.Equals(a2)) // danger!
                        return OneD_Intersection(b1, b2, a1, a2);
                    else // safe
                        return OneD_Intersection(a1, a2, b1, b2);
                }
                else
                {
                    // Parallel
                    return new SFML.Window.Vector2f[] { };
                }
            }
        }

        #endregion 
    }
}