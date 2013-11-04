using VectorClass;
using System;
using System.Data;
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;

namespace CollisionLineTests
{
    public class LineSegment
    {
        private static double MyEpsilon = 0.00001;

        public Vector2D_Dbl Start { get; set; }
        public Vector2D_Dbl End { get; set; }
        /// <summary>
        /// Initializes a new instance of the LineSegment class.
        /// </summary>
        public LineSegment()
        {
        }

        public LineSegment(Vector2D_Dbl start, Vector2D_Dbl end)
        {
            Start = start;
            End = end;
        }

        public bool CollidesWith(LineSegment other, out Vector2D_Dbl[] collisionPoints)
        {
            collisionPoints = Intersection(Start, End, other.Start, other.End);
            return collisionPoints.Length > 0;
        }

        #region Intersection stuff
        private static double[] OverlapIntervals(double ub1, double ub2)
        {
            double l = Math.Min(ub1, ub2);
            double r = Math.Max(ub1, ub2);
            double A = Math.Max(0, l);
            double B = Math.Min(1, r);
            if (A > B) // no intersection
                return new double[] { };
            else if (A == B)
                return new double[] { A };
            else // if (A < B)
                return new double[] { A, B };
        }

        // IMPORTANT: a1 and a2 cannot be the same, e.g. a1--a2 is a true segment, not a point
        // b1/b2 may be the same (b1--b2 is a point)
        private static Vector2D_Dbl[] OneD_Intersection(Vector2D_Dbl a1, Vector2D_Dbl a2, Vector2D_Dbl b1, Vector2D_Dbl b2)
        {
            //double ua1 = 0.0f; // by definition
            //double ua2 = 1.0f; // by definition
            double ub1, ub2;

            double denomx = a2.X - a1.X;
            double denomy = a2.Y - a1.Y;

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

            List<Vector2D_Dbl> ret = new List<Vector2D_Dbl>();
            double[] interval = OverlapIntervals(ub1, ub2);
            foreach (double f in interval)
            {
                double x = a2.X * f + a1.X * (1.0f - f);
                double y = a2.Y * f + a1.Y * (1.0f - f);
                Vector2D_Dbl p = new Vector2D_Dbl(x, y);
                ret.Add(p);
            }
            return ret.ToArray();
        }

        private static bool PointOnLine(Vector2D_Dbl p, Vector2D_Dbl a1, Vector2D_Dbl a2)
        {
            double dummyU = 0.0f;
            double d = DistFromSeg(p, a1, a2, MyEpsilon, ref dummyU);
            return d < MyEpsilon;
        }

        private static double DistFromSeg(Vector2D_Dbl p, Vector2D_Dbl q0, Vector2D_Dbl q1, double radius, ref double u)
        {
            // formula here:
            //http://mathworld.wolfram.com/Point-LineDistance2-Dimensional.html
            // where x0,y0 = p
            //       x1,y1 = q0
            //       x2,y2 = q1
            double dx21 = q1.X - q0.X;
            double dy21 = q1.Y - q0.Y;
            double dx10 = q0.X - p.X;
            double dy10 = q0.Y - p.Y;
            double segLength = Math.Sqrt(dx21 * dx21 + dy21 * dy21);
            if (segLength < MyEpsilon)
                throw new Exception("Expected line segment, not point.");
            double num = Math.Abs(dx21 * dy10 - dx10 * dy21);
            double d = num / segLength;
            return d;
        }

        // this is the general case. Really really general
        public static Vector2D_Dbl[] Intersection(Vector2D_Dbl a1, Vector2D_Dbl a2, Vector2D_Dbl b1, Vector2D_Dbl b2)
        {
            if (a1.Equals(a2) && b1.Equals(b2))
            {
                // both "segments" are points, return either point
                if (a1.Equals(b1))
                    return new Vector2D_Dbl[] { a1 };
                else // both "segments" are different points, return empty set
                    return new Vector2D_Dbl[] { };
            }
            else if (b1.Equals(b2)) // b is a point, a is a segment
            {
                if (PointOnLine(b1, a1, a2))
                    return new Vector2D_Dbl[] { b1 };
                else
                    return new Vector2D_Dbl[] { };
            }
            else if (a1.Equals(a2)) // a is a point, b is a segment
            {
                if (PointOnLine(a1, b1, b2))
                    return new Vector2D_Dbl[] { a1 };
                else
                    return new Vector2D_Dbl[] { };
            }

            // at this point we know both a and b are actual segments

            double ua_t = (b2.X - b1.X) * (a1.Y - b1.Y) - (b2.Y - b1.Y) * (a1.X - b1.X);
            double ub_t = (a2.X - a1.X) * (a1.Y - b1.Y) - (a2.Y - a1.Y) * (a1.X - b1.X);
            double u_b = (b2.Y - b1.Y) * (a2.X - a1.X) - (b2.X - b1.X) * (a2.Y - a1.Y);

            // Infinite lines intersect somewhere
            if (!(-MyEpsilon < u_b && u_b < MyEpsilon))   // e.g. u_b != 0.0
            {
                double ua = ua_t / u_b;
                double ub = ub_t / u_b;
                if (0.0f <= ua && ua <= 1.0f && 0.0f <= ub && ub <= 1.0f)
                {
                    // Intersection
                    return new Vector2D_Dbl[] {
                    new Vector2D_Dbl(a1.X + ua * (a2.X - a1.X),
                        a1.Y + ua * (a2.Y - a1.Y)) };
                }
                else
                {
                    // No Intersection
                    return new Vector2D_Dbl[] { };
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
                    return new Vector2D_Dbl[] { };
                }
            }
        }

        #endregion 
    }
}