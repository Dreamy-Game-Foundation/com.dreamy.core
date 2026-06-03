using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Dreamy.Core
{
    /// <summary>
    /// Abstract base for a single unit of work in a TaskRunner.
    /// Override Begin() for setup, Run() for the main logic, End() for cleanup.
    /// Sub-tasks (children of this GO) run in parallel during Run().
    /// </summary>
    public abstract class BaseTask : MonoBehaviour, ITask
    {
        public bool IsCompleted { get; private set; }
        public bool IsInterrupted { get; private set; }

        public event Action OnTaskComplete;

        async UniTask ITask.RunAsync()
        {
            IsCompleted = false;
            IsInterrupted = false;

            await Begin();
            if (!IsInterrupted) await Run();
            await End();

            IsCompleted = true;
            OnTaskComplete?.Invoke();
        }

        protected virtual UniTask Begin() => UniTask.CompletedTask;
        protected abstract UniTask Run();
        protected virtual UniTask End() => UniTask.CompletedTask;

        public void Interrupt() => IsInterrupted = true;
    }
}
