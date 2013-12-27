using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFML.Graphics;
using SFML.Window;

namespace CollisionLib
{
    public class CollisionResults : Drawable
    {
        public CollisionResults()
        {
            Type = CollisionType.enNone;
            CollisionTime = 0.0f;
            Sides = new Dictionary<AABB.AABBSide, AABB.AABBSide>();

            Object1CollisionAABB = new AABB(new SFML.Window.Vector2f(), new SFML.Window.Vector2f());
            Object2CollisionAABB = new AABB(new SFML.Window.Vector2f(), new SFML.Window.Vector2f());

            //Doesn't matter what we feed this, it should be overridden
            Object1Projection = new AABBProjection(Object1CollisionAABB, new SFML.Window.Vector2f());
            Object2Projection = new AABBProjection(Object2CollisionAABB, new SFML.Window.Vector2f());

            Overlap = new Vector2f();
        }

        public CollisionObject Object1 { get; set; }
        public CollisionObject Object2 { get; set; }
        public CollisionType Type { get; set; }
        public Dictionary<AABB.AABBSide, AABB.AABBSide> Sides { get; set; }
        public float CollisionTime { get; set; }
        public Vector2f Overlap { get; set; }

        //Temporary drawing stuffs.
        public AABB Object1CollisionAABB { get; set; }
        public AABB Object2CollisionAABB { get; set; }

        public AABBProjection Object1Projection { get; set; }
        public AABBProjection Object2Projection { get; set; }

        public void Draw(RenderTarget target, RenderStates states)
        {
            target.Draw(Object1.BoundingBox, states);
            target.Draw(Object2.BoundingBox, states);

            target.Draw(Object1Projection, states);
            target.Draw(Object2Projection, states);

            target.Draw(Object1CollisionAABB, states);
            target.Draw(Object2CollisionAABB, states);
        }
    }

    public enum CollisionType
    {
        enNone = 0,
        enAbsolute,
        enPrediction
    };
}
