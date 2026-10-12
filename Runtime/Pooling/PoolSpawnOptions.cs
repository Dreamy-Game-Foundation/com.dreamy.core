using System;
using UnityEngine;

namespace Dreamy.Core.Pooling
{
    /// <summary>Optional transform overrides; omitted values use the prefab transform. Scale is always local.</summary>
    public struct PoolSpawnOptions
    {
        public Transform Parent;
        public Space Space;
        public Vector3? Position;
        public Quaternion? Rotation;
        public Vector3? LocalScale;
        /// <summary>Optional instance name; omitted names reset to the prefab name on every spawn.</summary>
        public string Name;
        /// <summary>Runs after activation and pool OnSpawn notifications, before Spawn returns.</summary>
        public Action<GameObject> Configure;

        public static PoolSpawnOptions World(Vector3 position, Quaternion rotation, Transform parent = null)
            => new PoolSpawnOptions { Space = Space.World, Position = position, Rotation = rotation, Parent = parent };

        public static PoolSpawnOptions Local(Transform parent, Vector3 position, Quaternion rotation, Vector3? scale = null)
            => new PoolSpawnOptions { Space = Space.Self, Parent = parent, Position = position, Rotation = rotation, LocalScale = scale };
    }
}
