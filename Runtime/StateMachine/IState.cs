namespace Dreamy.Core
{
    public interface IState
    {
        void OnInitialize();
        void OnEnter(IStateData data = null);
        void OnExecute();
        void OnExit();
    }
}
