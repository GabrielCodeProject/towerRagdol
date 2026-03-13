namespace RagdollRealms.Core
{
    public interface ICommand
    {
        void Execute();
        void Undo();
    }
}
