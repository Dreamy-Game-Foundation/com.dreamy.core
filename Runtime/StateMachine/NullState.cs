namespace Dreamy.Core
{
    /// <summary>Null Object pattern — used when a requested state is not found, avoiding null checks.</summary>
    internal sealed class NullState : IState
    {
        public void OnInitialize() { }
        public void OnEnter(IStateData data = null) { }
        public void OnExecute() { }
        public void OnExit() { }
    }
}
