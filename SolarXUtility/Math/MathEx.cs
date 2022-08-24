using System;
using System.Numerics;

namespace SolarXUtility
{
    public static class MathEx
    {
        public static readonly double INCH = 2.54;

        /// <summary>
        /// Rounds a double to the nearest integer and returns a float
        /// </summary>
        /// <param name="number"></param>
        /// <returns> Rounded double as float</returns>
        public static float RoundToNearestFloat(double number) => (float)Math.Round(number, MidpointRounding.AwayFromZero);

        public static Vector3 CalculateTriangleNormal(Vector3 a, Vector3 b, Vector3 c) => Vector3.Normalize((c - b) * (a - b));
    }
}
