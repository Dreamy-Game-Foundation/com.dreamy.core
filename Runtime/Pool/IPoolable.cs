namespace Dreamy.Core
{
    /// <summary>
    /// Implement on any Component that will be managed by <see cref="GameObjectPool{T}"/>.
    /// </summary>
    public interface IPoolable
    {
        /// <summary>Called each time the object is retrieved from the pool.</summary>
        void OnSpawn();

        /// <summary>Called just before the object is returned to the pool.</summary>
        void OnDespawn();
    }
}
