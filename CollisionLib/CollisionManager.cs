using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFML.Window;

namespace CollisionLib
{
    public class CollisionManager
    {
        public CollisionManager()
        {
        }

        private sealed class ProjectionsSideCollisionInfo
        {
            public AABBProjection.AABBProjectionSegment ProjectionSegment { get; set; }
            public AABB.AABBSide Side { get; set; }
            public float Length { get; set; }
            public Vector2f CollisionPoint { get; set; }
        }

        static public CollisionResults TestCollisions(CollisionObject object1, CollisionObject object2)
        {
            CollisionResults results = new CollisionResults() { Object1 = object1, Object2 = object2 };

            //Are they already colliding?
            if (object1.BoundingBox.Overlaps(object2.BoundingBox))
            {
                results.Type = CollisionType.enAbsolute;
                return results;
            }

            //If both items are stationary, then there's no reason for projections. Return
            if ((object1.Velocity.X == 0.0f && object1.Velocity.Y == 0.0f) && (object2.Velocity.X == 0.0f && object2.Velocity.Y == 0.0f))
            {
                return results;
            }

            AABBProjection object1Projection = new AABBProjection(object1.BoundingBox, object1.Velocity);
            AABBProjection object2Projection = new AABBProjection(object2.BoundingBox, object2.Velocity);

            results.Object1Projection = object1Projection;
            results.Object2Projection = object2Projection;

            List<AABBProjection.AABBProjectionCollisionResult> projectionResults = null;

            bool collisions = (object1Projection.CollidesWith(object2Projection, out projectionResults) &&
                                ((object1.Velocity.X != 0.0f || object1.Velocity.Y != 0.0f) && (object2.Velocity.X != 0.0f || object2.Velocity.Y != 0.0f)));
            if (collisions)
            {
                List<AABBProjection.AABBProjectionCollisionResult> sorted = projectionResults.OrderBy(x => x.Length).ToList();

                AABBProjection.AABBProjectionCollisionResult shortestResult = sorted[0];

                AABBProjection.AABBProjectionSegmentEnum[] adjacentSegments = AABBProjection.GetAdjacentSegments(shortestResult.LocalSide);

                List<AABBProjection.AABBProjectionCollisionResult> adjacentSegmentResults = sorted.Where(x => x.OtherSide == shortestResult.OtherSide && adjacentSegments.Contains(x.LocalSide)).OrderBy(x => x.Length).ToList();

                if (adjacentSegmentResults.Count > 0)
                {
                    float shortestLength = adjacentSegmentResults[0].Length;
                    //There could be multiples if we hit a corner
                    adjacentSegmentResults = adjacentSegmentResults.Where(x => x.Length == shortestLength).ToList();

                    //Verify collision by projecting the AABB's to this point-in-time in their movement.
                    float totalMovementDistance = Helpers.DistanceBetweenTwoPoints(object1.BoundingBox.Position, object1Projection.End.Position);
                    //This is when the collision *MIGHT* happen for THIS collision object
                    float pointInTime = shortestLength / totalMovementDistance;

                    SFML.Window.Vector2f object1Position = object1.BoundingBox.Position + (object1.Velocity * pointInTime);
                    AABB localCollisionResultProjection = new AABB(object1Position, object1.BoundingBox.Extents, SFML.Graphics.Color.Green);

                    //Now, see where the OTHER collision object will be at this point in time.
                    SFML.Window.Vector2f otherPosition = object2.BoundingBox.Position + (object2.Velocity * pointInTime);
                    AABB otherCollisionResultProjection = new AABB(otherPosition, object2.BoundingBox.Extents, SFML.Graphics.Color.Blue);
                    results.Object1CollisionAABB = localCollisionResultProjection;
                    results.Object2CollisionAABB = otherCollisionResultProjection;
                    results.CollisionTime = pointInTime;
                    //Do they overlap at the projection result?
                    if (!localCollisionResultProjection.Overlaps(otherCollisionResultProjection))
                        return results;

                    foreach (AABBProjection.AABBProjectionCollisionResult collisionResult in adjacentSegmentResults)
                    {
                        AABB.AABBSide collisionSide = AABBProjection.GetSideFromProjectionSegments(shortestResult.LocalSide, collisionResult.LocalSide);
                        AABB.AABBSide otherCollisionSide = AABB.GetOppositeSide(collisionSide);

                        results.Object1CollisionAABB.Sides[(int)collisionSide].SetColor(SFML.Graphics.Color.Red);
                        results.Object2CollisionAABB.Sides[(int)otherCollisionSide].SetColor(SFML.Graphics.Color.Red);
                        results.Sides[collisionSide] = otherCollisionSide;
                    }

                    results.Type = CollisionType.enPrediction;
                }
            }
            else
            {
                List<ProjectionsSideCollisionInfo> collisionInfo = null;
                AABBProjection checkedObjectProjection = null;
                if (object1.Velocity.X != 0.0f || object1.Velocity.Y != 0.0f)
                {
                    //Test projection1 against projection2's End's segments
                    TestProjectionAgainstAABB(object1Projection, object2Projection.End, out collisionInfo);
                    checkedObjectProjection = object1Projection;
                }
                else if (object2.Velocity.X != 0.0f || object2.Velocity.Y != 0.0f)
                {
                    //Test projection2 against projection1's End segments
                    TestProjectionAgainstAABB(object2Projection, object1Projection.End, out collisionInfo);
                    checkedObjectProjection = object2Projection;
                }

                if(collisionInfo != null)
                {
                    if (collisionInfo.Count > 0)
                    {
                        collisionInfo = collisionInfo.OrderBy(x => x.Length).ToList();
                        ProjectionsSideCollisionInfo closestInfo = collisionInfo[0];
                        float shortestLength = closestInfo.Length;
                        //Verify collision by projecting the AABB's to this point-in-time in their movement.
                        float totalMovementDistance = Helpers.DistanceBetweenTwoPoints(checkedObjectProjection.Start.Position, checkedObjectProjection.End.Position);
                        //This is when the collision *MIGHT* happen for THIS collision object
                        float pointInTime = shortestLength / totalMovementDistance;

                        SFML.Window.Vector2f object1Position = object1.BoundingBox.Position + (object1.Velocity * pointInTime);
                        AABB localCollisionResultProjection = new AABB(object1Position, object1.BoundingBox.Extents, SFML.Graphics.Color.Green);

                        //Now, see where the OTHER collision object will be at this point in time.
                        SFML.Window.Vector2f otherPosition = object2.BoundingBox.Position + (object2.Velocity * pointInTime);
                        AABB otherCollisionResultProjection = new AABB(otherPosition, object2.BoundingBox.Extents, SFML.Graphics.Color.Blue);
                        //Do they overlap at the projection result?
                        //if (!localCollisionResultProjection.Overlaps(otherCollisionResultProjection))
                            //return results;

                        results.Object1CollisionAABB = localCollisionResultProjection;
                        results.Object2CollisionAABB = otherCollisionResultProjection;

                        AABB.AABBSide object1Side = AABB.GetOppositeSide(closestInfo.Side);
                        results.Object1CollisionAABB.Sides[(int)object1Side].SetColor(SFML.Graphics.Color.Red);
                        results.Object2CollisionAABB.Sides[(int)closestInfo.Side].SetColor(SFML.Graphics.Color.Red);

                        results.Type = CollisionType.enPrediction;
                        results.Sides[object1Side] = closestInfo.Side;
                        results.CollisionTime = pointInTime;
                    }
                }
            }

            if (results.CollisionTime == (1.0f / 0.0f))
            {
                int i = 0;
            }

            return results;
        }

        static private void TestProjectionAgainstAABB(AABBProjection projection, AABB aabb, out List<ProjectionsSideCollisionInfo> collisionInfo )
        {
            collisionInfo = new List<ProjectionsSideCollisionInfo>();
            foreach (AABBProjection.AABBProjectionSegment projectionSegment in projection.PathSegments.Values)
            {
                for (int i = 0; i < 4; i++)
                {
                    AABB.AABBSide currentSide = (AABB.AABBSide)i;
                    LineSegment aabbSegment = aabb.Sides[i];
                    Vector2f[] collisionPoints = null;
                    if (projectionSegment.Path.CollidesWith(aabbSegment, out collisionPoints))
                    {
                        foreach (Vector2f collisionPoint in collisionPoints)
                        {
                            ProjectionsSideCollisionInfo info = new ProjectionsSideCollisionInfo();
                            info.ProjectionSegment = projectionSegment;
                            info.CollisionPoint = collisionPoint;
                            info.Side = currentSide;
                            info.Length = Helpers.DistanceBetweenTwoPoints(projectionSegment.Path.Start, collisionPoint);

                            collisionInfo.Add(info);
                        }
                    }
                }
            }
        }
    }
}
