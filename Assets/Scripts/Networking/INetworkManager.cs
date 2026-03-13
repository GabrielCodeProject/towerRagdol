namespace RagdollRealms.Networking
{
    public interface INetworkManager
    {
        void HostSession(int maxPlayers = 4);
        void JoinSession(string address, int port = 7777);
        void Disconnect();
        bool IsHost { get; }
        bool IsConnected { get; }
        int LocalPlayerId { get; }
        int PlayerCount { get; }
    }
}
