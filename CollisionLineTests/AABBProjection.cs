using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VectorClass;
namespace CollisionLineTests
{
    class AABBProjection
    {
        public enum AABBProjectionSegment
        {
            enTopLeft = 0,
            enTopRight = 1,
            enBottomLeft = 2,
            enBottomRight = 3
        };

        public AABB Start { get; set; }
        public AABB End { get; set; }
        public LineSegment[] PathSegments { get; set; }


        public AABBProjection(AABB start, Vector2D_Dbl movement)
        {
            Start = start;
            End = new AABB(new Vector2D_Dbl(start.Position.X + movement.X, start.Position.Y + movement.Y), start.Extents.X, start.Extents.Y);

            PathSegments = new LineSegment[4];
            PathSegments[(int)AABBProjectionSegment.enTopLeft] = new LineSegment(Start.Position, End.Position);
            PathSegments[(int)AABBProjectionSegment.enTopRight] = new LineSegment(Start.Sides[(int)AABB.AABBSide.enRight].Start, End.Sides[(int)AABB.AABBSide.enRight].Start);
            PathSegments[(int)AABBProjectionSegment.enBottomLeft] = new LineSegment(Start.Sides[(int)AABB.AABBSide.enLeft].Start, End.Sides[(int)AABB.AABBSide.enLeft].Start);
            PathSegments[(int)AABBProjectionSegment.enBottomRight] = new LineSegment(Start.Sides[(int)AABB.AABBSide.enBottom].Start, End.Sides[(int)AABB.AABBSide.enBottom].Start);
        }
    }
}
