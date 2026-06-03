using Cysharp.Threading.Tasks;

namespace Dreamy.Core
{
    public interface IConnectivityService
    {
        bool IsConnected { get; }

        /// <summary>
        /// Performs a real HTTP HEAD request to verify internet access.
        /// More reliable than Application.internetReachability.
        /// </summary>
        UniTask<bool> CheckInternetAsync();
    }

    public struct ConnectivityChangedEvent : IEvent
    {
        public bool IsConnected;
    }
}
