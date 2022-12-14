using System.Drawing;

namespace SolarX.Extensions
{
    public static class ColorExtension
    {
        /// <summary>Blends the specified colors together.</summary>
        /// <param name="color">Color to blend onto the background color.</param>
        /// <param name="backColor">Color to blend the other color onto.</param>
        /// <param name="dt">How much of <paramref name="color"/> to keep,
        /// “on top of” <paramref name="backColor"/>.</param>
        /// <returns>The blended colors.</returns>
        public static Color Blend(this Color color, Color backColor, double dt)
        {
            byte r = (byte)(color.R * dt + backColor.R * (1 - dt));
            byte g = (byte)(color.G * dt + backColor.G * (1 - dt));
            byte b = (byte)(color.B * dt + backColor.B * (1 - dt));
            return Color.FromArgb(r, g, b);
        }

        public static Color Mix(this Color color, Color otherColor) => Color.FromArgb(
                (color.R + otherColor.R) / 2,
                (color.G + otherColor.G) / 2,
                (color.B + otherColor.B) / 2);
    }
}
