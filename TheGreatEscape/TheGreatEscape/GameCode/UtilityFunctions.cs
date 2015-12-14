using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TheGreatEscape.GameCode
{
    /// <summary>
    /// A repository of useful functions that remain spearate from the main project
    /// </summary>
    public static class UtilityFunctions
    {
        /// <summary>
        /// Quickly convers a Rectangle to a Bounding Box
        /// </summary>
        /// <param name="rectangle"></param>An objects rectangle
        /// <returns></returns>
        public static BoundingBox ConvertRectangleToBB(Rectangle rectangle)
        {
            Vector3 min = new Vector3(rectangle.X, rectangle.Y, 0);
            Vector3 max = new Vector3(rectangle.X + rectangle.Width, rectangle.Y + rectangle.Height, 0);
            return new BoundingBox(min, max);
        }
        /// <summary>
        ///  Quickly convers a Bounding Box to a Rectangle
        /// </summary>
        /// <param name="box"></param>An objects BB
        /// <returns></returns>
        public static Rectangle ConvertBBToRectangle(BoundingBox box)
        {
            Rectangle rect = new Rectangle();
            rect.X = (int) box.Min.X;
            rect.Y = (int) box.Min.Y;
            rect.Width = (int) (Math.Abs(box.Min.X - box.Max.X));
            rect.Height = (int) (Math.Abs(box.Min.Y - box.Max.Y));
            return rect;
        }
        /// <summary>
        /// Using an objects position and texture create a sphere collider around
        /// </summary>
        /// <param name="position"></param>
        /// <param name="txr"></param>
        /// <returns></returns>
        public static BoundingSphere CreateSphere(Vector2 position, Texture2D txr)
        {
            return new BoundingSphere(new Vector3(position, 0), txr.Width/2f);
        }
        /// <summary>
        /// Convert a Vector3 into a Vector2
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static Vector2 V3ToV2(Vector3 input)
        {
            return new Vector2(input.X, input.Y);
        }
    }
}
