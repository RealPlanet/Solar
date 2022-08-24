namespace SolarXCod.SeLibWrapper
{
    public static class SeExtensions
    {
        public static System.Numerics.Vector3 ToNumericsVec(this SELib.Utilities.Vector3 vec) => new System.Numerics.Vector3((float)vec.X, (float)vec.Y, (float)vec.Z);
    }
}
