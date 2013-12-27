using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFML.Window;
using SFML.Graphics;
namespace CollisionLib
{
    public class CollisionObject : Drawable
    {
        public Boolean IsPlayer { get; set; }

        private SFML.Graphics.Color originalColor;
        public void RevertToOriginalColor()
        {
            BoundingBox.Color = originalColor;
        }

        public CollisionObject()
        {
            originalColor = SFML.Graphics.Color.White;
            Configure();
        }

        public CollisionObject(Vector2f position, Vector2f extents)
        {
            originalColor = SFML.Graphics.Color.White;
            BoundingBox = new AABB(position, extents, originalColor);
            Configure();
        }

        public CollisionObject(Vector2f position, Vector2f extents, SFML.Graphics.Color color)
        {
            originalColor = color;
            BoundingBox = new AABB(position, extents, color);
            Configure();
        }

        private void Configure()
        {
            velocity = new Vector2f();
            currentFrameCollisionResults = new List<CollisionResults>();
        }

        public AABB BoundingBox { get; set; }
        private Vector2f velocity;
        public Vector2f Velocity { get { return velocity; } set { velocity = value; } }

        public void Move()
        {
            BoundingBox.Position += velocity;
        }

        private List<CollisionResults> currentFrameCollisionResults;
        public void OnCollision(CollisionResults results)
        {
            currentFrameCollisionResults.Add(results);
        }

        public void ProcessCollisions()
        {
            if (IsPlayer && currentFrameCollisionResults.Count > 1)
            {
                int i = 0;
            }
            foreach (CollisionResults results in currentFrameCollisionResults)
            {
                HandleCollision(results);
            }
        }
        public void PostFrame()
        {
            currentFrameCollisionResults.Clear();
        }
        private void HandleCollision(CollisionResults results)
        {
            if (IsPlayer && results.Type == CollisionType.enAbsolute)
            {
                CollisionResults reaffirm = CollisionManager.TestCollisions(this, (this == results.Object1) ? results.Object2 : results.Object1);
            }
            if (IsPlayer && ((results.CollisionTime > 0.0f && results.CollisionTime < 1.0f) || (results.Sides.Count > 1)))
            {
                int i = 0;
            }
            BoundingBox.Color = SFML.Graphics.Color.Green;
            //For testing purposes.  This will stop an object BEFORE it collides.
            if (results.Object1 == this)
            {
                foreach (AABB.AABBSide collision in results.Sides.Keys)
                {
                    switch (collision)
                    {
                        case AABB.AABBSide.enTop:
                            {
                                if (velocity.Y < 0.0f)
                                    velocity.Y = 0.0f;
                            }
                            break;
                        case AABB.AABBSide.enRight:
                            {
                                if (velocity.X > 0.0f)
                                    velocity.X = 0.0f;
                            }
                            break;
                        case AABB.AABBSide.enBottom:
                            {
                                if (velocity.Y > 0.0f)
                                    velocity.Y = 0.0f;
                            }
                            break;
                        case AABB.AABBSide.enLeft:
                            {
                                if (velocity.X < 0.0f)
                                    velocity.X = 0.0f;
                            }
                            break;
                    };

                    BoundingBox.Sides[(int)collision].SetColor(SFML.Graphics.Color.Red);
                }
            }
            else
            {
                foreach (AABB.AABBSide collision in results.Sides.Values)
                {
                    switch (collision)
                    {
                        case AABB.AABBSide.enTop:
                            {
                                if (velocity.Y < 0.0f)
                                    velocity.Y = 0.0f;
                            }
                            break;
                        case AABB.AABBSide.enRight:
                            {
                                if (velocity.X > 0.0f)
                                    velocity.X = 0.0f;
                            }
                            break;
                        case AABB.AABBSide.enBottom:
                            {
                                if (velocity.Y > 0.0f)
                                    velocity.Y = 0.0f;
                            }
                            break;
                        case AABB.AABBSide.enLeft:
                            {
                                if (velocity.X < 0.0f)
                                    velocity.X = 0.0f;
                            }
                            break;
                    };

                    BoundingBox.Sides[(int)collision].SetColor(SFML.Graphics.Color.Red);
                }
            }
        }

        public void Draw(RenderTarget target, RenderStates states)
        {
            target.Draw(BoundingBox, states);
        }

        /*
        public List<CollisionResults> CollidesWith(CollisionObject other)
        {
            List<CollisionResults> results = new List<CollisionResults>();

            //Are they already colliding?
            if(BoundingBox.Overlaps(other.BoundingBox)) 
            {
                results.Add(new CollisionResults() { Type = CollisionType.enAbsolute });
                return results;
            }

            //If there's no reason for projections, return
            if ((Velocity.X == 0.0f && Velocity.Y == 0.0f) && (other.Velocity.X == 0.0f && other.Velocity.Y == 0.0f))
            {
                return results;
            }

            AABBProjection localProjection = new AABBProjection(BoundingBox, Velocity);
            AABBProjection otherProjection = new AABBProjection(other.BoundingBox, other.Velocity);

            List<AABBProjection.AABBProjectionCollisionResult> projectionResults = null;

            bool collisions = localProjection.CollidesWith(otherProjection, out projectionResults);
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
                    float totalMovementDistance = Helpers.DistanceBetweenTwoPoints(BoundingBox.Position, localProjection.End.Position);
                    //This is when the collision *MIGHT* happen for THIS collision object
                    float pointInTime = shortestLength / totalMovementDistance;

                    SFML.Window.Vector2f localPosition = BoundingBox.Position + (Velocity * pointInTime);
                    AABB localCollisionResultProjection = new AABB(localPosition, BoundingBox.Extents.X, BoundingBox.Extents.Y);

                    //Now, see where the OTHER collision object will be at this point in time.
                    SFML.Window.Vector2f otherPosition = other.BoundingBox.Position + (other.Velocity * pointInTime);
                    AABB otherCollisionResultProjection = new AABB(otherPosition, other.BoundingBox.Extents.X, other.BoundingBox.Extents.Y);
                    //Do they overlap at the projection result?
                    if (!localCollisionResultProjection.Overlaps(otherCollisionResultProjection))
                        return results;

                    CollisionResults collisionResults = new CollisionResults() { Type = CollisionType.enPrediction };
                    collisionResults.Object1 = this;
                    collisionResults.Object2 = other;
                    collisionResults.CollisionTime = pointInTime;
                    foreach (AABBProjection.AABBProjectionCollisionResult collisionResult in adjacentSegmentResults)
                    {
                        AABB.AABBSide collisionSide = AABBProjection.GetSideFromProjectionSegments(shortestResult.LocalSide, collisionResult.LocalSide);
                        AABB.AABBSide otherCollisionSide = AABB.GetOppositeSide(collisionSide);
                        collisionResults.Sides[collisionSide] = otherCollisionSide;
                    }

                    results.Add(collisionResults);
                }
            }

            return results;
        }
        */
    }
}
