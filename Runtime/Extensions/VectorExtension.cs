using UnityEngine;

namespace Dreamy.Core
{
    public static class VectorExtension
    {
        public static Vector2 WithX(this Vector2 v, float x) => new(x, v.y);
        public static Vector2 WithY(this Vector2 v, float y) => new(v.x, y);

        public static Vector3 WithX(this Vector3 v, float x) => new(x, v.y, v.z);
        public static Vector3 WithY(this Vector3 v, float y) => new(v.x, y, v.z);
        public static Vector3 WithZ(this Vector3 v, float z) => new(v.x, v.y, z);

        public static Vector3 ToVector3(this Vector2 v, float z = 0f) => new(v.x, v.y, z);
        public static Vector2 ToVector2XY(this Vector3 v) => new(v.x, v.y);
        public static Vector2 ToVector2XZ(this Vector3 v) => new(v.x, v.z);

        /// <summary>Returns a random float between x and y.</summary>
        public static float RandomRange(this Vector2 v) => Random.Range(v.x, v.y);

        /// <summary>Returns a random int between (int)x and (int)y, inclusive.</summary>
        public static int RandomRangeInt(this Vector2Int v) => Random.Range(v.x, v.y + 1);

        public static Vector3 Flat(this Vector3 v) => new(v.x, 0f, v.z);
    }
}
