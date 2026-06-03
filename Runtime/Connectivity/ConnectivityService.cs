using System;
using Cysharp.Threading.Tasks;
using UnityEngine.Networking;

namespace Dreamy.Core
{
    /// <summary>
    /// Checks real internet connectivity by pinging a URL.
    /// Fires ConnectivityChangedEvent via MyEventBus when state changes.
    /// Periodically re-checks in background.
    /// </summary>
    public sealed class ConnectivityService : IConnectivityService, IDisposable
    {
        private const string PING_URL = "https://connectivitycheck.gstatic.com/generate_204";
        private const int TIMEOUT_SECONDS = 5;
        private const float CHECK_INTERVAL_SECONDS = 10f;

        private bool _isConnected;
        private bool _isRunning;

        public bool IsConnected => _isConnected;

        public ConnectivityService()
        {
            _isRunning = true;
            MonitorLoopAsync().Forget();
        }

        public async UniTask<bool> CheckInternetAsync()
        {
            try
            {
                using var request = UnityWebRequest.Head(PING_URL);
                request.timeout = TIMEOUT_SECONDS;
                await request.SendWebRequest();

                bool connected = request.result == UnityWebRequest.Result.Success;
                UpdateConnectivity(connected);
                return connected;
            }
            catch
            {
                UpdateConnectivity(false);
                return false;
            }
        }

        private async UniTaskVoid MonitorLoopAsync()
        {
            while (_isRunning)
            {
                await CheckInternetAsync();
                await UniTask.Delay(
                    TimeSpan.FromSeconds(CHECK_INTERVAL_SECONDS),
                    ignoreTimeScale: true);
            }
        }

        private void UpdateConnectivity(bool isConnected)
        {
            if (_isConnected == isConnected) return;
            _isConnected = isConnected;
            MyEventBus<ConnectivityChangedEvent>.Raise(new ConnectivityChangedEvent { IsConnected = isConnected });
            DreamyLog.Log($"Connectivity changed: {(isConnected ? "Online" : "Offline")}");
        }

        public void Dispose() => _isRunning = false;
    }
}
