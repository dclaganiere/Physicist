namespace Physicist.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using FarseerPhysics;
    using Microsoft.Xna.Framework;

    public static class Extensions
    {
        public static IEnumerable<Point> GetCorners(this Rectangle rect)
        {
            List<Point> corners = new List<Point>()
            {
                new Point(rect.X, rect.Y),
                new Point(rect.X + rect.Width, rect.Y),
                new Point(rect.X + rect.Width, rect.Y + rect.Height),
                new Point(rect.X, rect.Y + rect.Height)
            };
            
            return corners;
        }

        public static Vector2 ToSimUnits(this Vector2 vect)
        {
            return ConvertUnits.ToSimUnits(vect);
        }

        public static Vector2 ToDisplayUnits(this Vector2 vect)
        {
            return ConvertUnits.ToDisplayUnits(vect);
        }
    }
}
