using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TheGreatEscape.GameCode;

namespace PlatformGame
{
    /// <summary>
    /// The simplest Graphic type, it consists of a Texture and a Vector 2 Position.
    /// </summary>
    internal class StaticGraphic
    {
        protected Vector2 m_position;
        protected Texture2D m_txr;
        public StaticGraphic(Vector2 Position, Texture2D txrImage)
        {
            m_position = Position;
            m_txr = txrImage;
        }
        public virtual void Draw(SpriteBatch sBatch)
        {
            sBatch.Draw(m_txr, m_position, Color.White);
        }
    }
    /// <summary>
    /// A static Graphi with velocity. It can move now.
    /// </summary>
    internal class MotionGraphic : StaticGraphic
    {
        protected Vector2 m_velocity;
        public  MotionGraphic(Vector2 Position, Texture2D txr) : base(Position, txr)
        {
           m_velocity = Vector2.Zero;
        }
    }
    /// <summary>
    /// A motion Graphic with a defined Bounding Sphere
    /// </summary>
    internal class Circle : MotionGraphic
    {
        protected BoundingSphere m_sphere;
        public Circle(Vector2 Position, Texture2D txr) : base(Position, txr)
        {
        }
        public void Update(GameTime gt, Vector2 m_position)
        {
            m_sphere = UtilityFunctions.CreateSphere(m_position, m_txr);
        }
        public BoundingSphere GetSphere()
        {
            return UtilityFunctions.CreateSphere(m_position, m_txr);
        }
        public virtual void Draw(SpriteBatch sBatch, Texture2D circleTxr)
        {
            sBatch.Draw(m_txr, m_position, Color.White);
        }
    }
    /// <summary>
    /// A motion Graphic with a defined Rectangle and BoundingBox
    /// </summary>
    internal class Square : MotionGraphic
    {
        protected Rectangle m_Rect;
        protected BoundingBox m_Box ;
        public Square(Vector2 Position, Texture2D txr): base(Position, txr)
        {
        }
        public virtual void Update(GameTime gt)
        {
            m_Rect = GetBounds();
            m_Box = GetBoundingBox();
        }
        /// <summary>
        /// Return a Bounding box the size of the brick and in the position of the brick
        /// </summary>
        /// <returns></returns>
        public Rectangle GetBounds()
        {
            return new Rectangle((int)m_position.X, (int)m_position.Y, m_txr.Width, m_txr.Height);
        }
        /// <summary>
        /// Return a Bounding box the size of the brick and in the position of the brick
        /// </summary>
        /// <returns></returns>
        public BoundingBox GetBoundingBox()
        {
            return UtilityFunctions.ConvertRectangleToBB(GetBounds());
        }
        public virtual void Draw(SpriteBatch sBatch, Texture2D box_min, Texture2D box_max)
        {
            sBatch.Draw(m_txr, m_position, Color.White);
        }
    }
}