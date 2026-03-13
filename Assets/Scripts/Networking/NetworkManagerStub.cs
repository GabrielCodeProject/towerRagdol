using RagdollRealms.Core;
using RagdollRealms.Core.Events;
using UnityEngine;

namespace RagdollRealms.Networking
{
    /// <summary>
    /// Stub network manager for single-player development.
    /// Replace with Mirror/Fusion implementation when networking is integrated.
    /// </summary>
    public class NetworkManagerStub : MonoBehaviour, INetworkManager
    {
        public bool IsHost => true;
        public bool IsConnected => true;
        public int LocalPlayerId => 0;
        public int PlayerCount => 1;

        private IEventBus _eventBus;

        private void Awake()
        {
            ServiceLocator.Instance.Register<INetworkManager>(this);
            _eventBus = ServiceLocator.Instance.Get<IEventBus>();
        }

        public void HostSession(int maxPlayers = 4)
        {
            Debug.Log("[NetworkManagerStub] Hosting stub session (single-player).");
            _eventBus.Publish(new OnPlayerConnected(0));
        }

        public void JoinSession(string address, int port = 7777)
        {
            Debug.LogWarning("[NetworkManagerStub] Join not supported in stub. Use single-player.");
        }

        public void Disconnect()
        {
            _eventBus.Publish(new OnPlayerDisconnected(0));
            Debug.Log("[NetworkManagerStub] Disconnected from stub session.");
        }

        private void OnDestroy()
        {
            if (ServiceLocator.Instance != null)
                ServiceLocator.Instance.Unregister<INetworkManager>();
        }
    }
}
