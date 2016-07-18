using System;

namespace MultiAlignCore.Data
{
    /// <summary>
    /// Holds an index for a three dimensional collection
    /// </summary>
    public class Index3D
    {
        /// <summary>
        /// Initialize all indices to -1
        /// </summary>
        public Index3D()
        {
            X = -1;
            Y = -1;
            Z = -1;
        }

        /// <summary>
        /// Initialize each index to the supplied value
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public Index3D(int x, int y, int z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        /// <summary>
        /// Set each index to the supplied value
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public void Set(int x, int y, int z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        /// <summary>
        /// Set each index to the corresponding values of another Index3D
        /// </summary>
        /// <param name="other"></param>
        public void Set(Index3D other)
        {
            X = other.X;
            Y = other.Y;
            Z = other.Z;
        }

        /// <summary>
        /// The x-axis, or first, index
        /// </summary>
        public int X { get; set; }

        /// <summary>
        /// The y-axis, or second, index
        /// </summary>
        public int Y { get; set; }

        /// <summary>
        /// The z-axis, or third, index
        /// </summary>
        public int Z { get; set; }

        public bool IsValid()
        {
            return X >= 0 && Y >= 0 && Z >= 0;
        }

        public bool IsValid<T>(T[,,] array)
        {
            return IsValid(array.GetLength(0), array.GetLength(1), array.GetLength(2));
        }

        public bool IsValid(int xLength, int yLength, int zLength)
        {
            return IsValid() && X < xLength && Y < yLength  && Z < zLength;
        }

        public override string ToString()
        {
            return string.Format("[{0}, {1}, {2}]", X, Y, Z);
        }
    }
}
