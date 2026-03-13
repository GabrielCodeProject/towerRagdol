using System;

namespace RagdollRealms.Core
{
    public interface IServiceLocator
    {
        void Register<T>(T service) where T : class;
        void Unregister<T>() where T : class;
        T Get<T>() where T : class;
        bool TryGet<T>(out T service) where T : class;
        bool Has<T>() where T : class;
    }
}
