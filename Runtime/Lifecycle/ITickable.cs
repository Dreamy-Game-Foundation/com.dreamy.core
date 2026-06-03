namespace Dreamy.Core
{
    /// <summary>
    /// Implement this interface on any service that needs a per-frame Update tick
    /// without being a MonoBehaviour.
    /// Register with: AppTickService.Register(this)
    /// Unregister with: AppTickService.Unregister(this)
    /// </summary>
    public interface ITickable
    {
        void Tick(float deltaTime);
    }
}
