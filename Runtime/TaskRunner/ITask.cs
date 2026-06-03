using Cysharp.Threading.Tasks;

namespace Dreamy.Core
{
    public interface ITask
    {
        bool IsCompleted { get; }
        UniTask RunAsync();
        void Interrupt();
    }
}
