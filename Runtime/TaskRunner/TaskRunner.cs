using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Dreamy.Core
{
    /// <summary>
    /// Runs immediate child BaseTask components sequentially.
    /// Each task can have its own children for internal parallel logic.
    /// </summary>
    /// <example>
    /// // In scene hierarchy:
    /// // TaskRunner
    /// //   ├── LoadDataTask      (runs first)
    /// //   ├── InitServicesTask  (runs second)
    /// //   └── ShowMenuTask      (runs third)
    /// //
    /// taskRunner.Run();
    /// taskRunner.OnComplete += () => Debug.Log("All tasks done");
    /// </example>
    public sealed class TaskRunner : MonoBehaviour
    {
        [SerializeField] private bool _runOnStart;

        public event Action OnComplete;
        public bool IsRunning { get; private set; }

        private CancellationTokenSource _cts;

        private void Start()
        {
            if (_runOnStart) Run();
        }

        public void Run() => ExecuteAsync().Forget();

        public async UniTask RunAsync()
        {
            if (IsRunning)
            {
                DreamyLog.Warn("TaskRunner is already running — call Stop() first");
                return;
            }
            await ExecuteAsync();
        }

        private async UniTask ExecuteAsync()
        {
            _cts?.Dispose();
            _cts = new CancellationTokenSource();
            IsRunning = true;

            var tasks = GetDirectChildTasks();
            foreach (var task in tasks)
            {
                if (_cts.IsCancellationRequested) break;
                await task.RunAsync();
            }

            IsRunning = false;
            OnComplete?.Invoke();
        }

        public void Stop()
        {
            _cts?.Cancel();
            IsRunning = false;
        }

        private List<ITask> GetDirectChildTasks()
        {
            var result = new List<ITask>();
            for (int i = 0; i < transform.childCount; i++)
            {
                var child = transform.GetChild(i);
                if (child.gameObject.activeSelf && child.TryGetComponent<BaseTask>(out var task))
                    result.Add(task);
            }
            return result;
        }

        private void OnDestroy() => _cts?.Cancel();
    }
}
