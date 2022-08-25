using FastBitmapLib;
using SolarXImage.Enum;
using System.Drawing;

namespace SolarX.Image
{
    public static class FastMapExtensions
    {
        public static Color GetPixelFromLayer(this FastBitmap bitmap, int x, int y, ImageLayer layer)
        {
            Color tmp = bitmap.GetPixel(x, y);
            switch (layer)
            {
                case ImageLayer.Alpha:
                    return Color.FromArgb(tmp.A, tmp.A, tmp.A);
                case ImageLayer.Red:
                    return Color.FromArgb(tmp.R, tmp.R, tmp.R);
                case ImageLayer.Green:
                    return Color.FromArgb(tmp.G, tmp.G, tmp.G);
                case ImageLayer.Blue:
                    return Color.FromArgb(tmp.B, tmp.B, tmp.B);
                case ImageLayer.RGB:
                    return Color.FromArgb(tmp.R, tmp.G, tmp.B);
                default:
                    return tmp;
            }
        }

        public static Bitmap GetLayer(this FastBitmap bitmap, ImageLayer layer)
        {

            Bitmap result = new Bitmap(bitmap.Width, bitmap.Height);
            using (FastBitmap fastResult = result.FastLock())
            {
                for (int i = 0; i < bitmap.Width; i++)
                    for (int j = 0; j < bitmap.Height; j++)
                    {
                        fastResult.SetPixel(i, j, bitmap.GetPixelFromLayer(i, j, layer));
                    }
            }
            return result;
        }

        public static bool IsFullyWhite(this FastBitmap bitmap)
        {
            bool IsWhite = true;
            int WhiteARGB = Color.White.ToArgb();
            for (int i = 0; i < bitmap.Width; i++)
                for (int j = 0; j < bitmap.Height; j++)
                {
                    Color pixel = bitmap.GetPixel(i, j);
                    IsWhite = IsWhite && pixel.ToArgb().Equals(WhiteARGB);
                }
            return IsWhite;
        }

        public static bool IsFullyBlack(this FastBitmap bitmap)
        {
            bool IsBlack = true;
            int BlackARGB = Color.Black.ToArgb();
            for (int i = 0; i < bitmap.Width; i++)
                for (int j = 0; j < bitmap.Height; j++)
                {
                    Color pixel = bitmap.GetPixel(i, j);
                    IsBlack = IsBlack && pixel.ToArgb().Equals(BlackARGB);
                }
            return IsBlack;
        }
    }
}
