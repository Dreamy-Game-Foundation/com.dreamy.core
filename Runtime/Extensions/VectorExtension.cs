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

        public static float RandomRange(this Vector2 v) => Random.Range(v.x, v.y);
        public static int RandomRangeInt(this Vector2Int v) => Random.Range(v.x, v.y + 1);

        public static Vector3 Flat(this Vector3 v) => new(v.x, 0f, v.z);
        public static Vector2 Abs(this Vector2 v) => new(Mathf.Abs(v.x), Mathf.Abs(v.y));
        public static Vector3 Abs(this Vector3 v) => new(Mathf.Abs(v.x), Mathf.Abs(v.y), Mathf.Abs(v.z));
        public static Vector3 AddX(this Vector3 v, float x) => new(v.x + x, v.y, v.z);
        public static Vector3 AddY(this Vector3 v, float y) => new(v.x, v.y + y, v.z);
        public static Vector3 AddZ(this Vector3 v, float z) => new(v.x, v.y, v.z + z);
        public static float DistanceXZ(this Vector3 from, Vector3 to) => Vector2.Distance(from.ToVector2XZ(), to.ToVector2XZ());
    }
}
