using Microsoft.Xna.Framework;
using System;

namespace Adventure
{
    public abstract class Session
    {
        public static Session Instance { get; protected set; }

        public event Action<int> PeerConnected;
        public event Action<int> PeerDisconnected;
        public event Action<int, Message> EventReceived;

        public int Id { get; protected set; }
        public bool IsServer { get; protected set; }
        public bool IsClient => !IsServer;
        public float Latency { get; protected set; }
        public int Tick { get; protected set; }

        public abstract void HostSession(int port);
        public abstract void JoinSession(string ipAddress, int port);
        public abstract void StopSession();
        public abstract void Update(GameTime gameTime);
        public abstract void ClientSendReliable(Message message);
        public abstract void ClientSendUnreliable(Message message);
        public abstract void ServerSendReliable(int peerId, Message message);
        public abstract void ServerSendUnreliable(int peerId, Message message);
        public abstract void ServerSendReliable(Message message);
        public abstract void ServerSendUnreliable(Message message);

        protected void InvokePeerConnected(int peerId) => PeerConnected.Invoke(peerId);
        protected void InvokePeerDisconnected(int peerId) => PeerDisconnected.Invoke(peerId);
        protected void InvokeEventReceived(int peerId, Message message) => EventReceived.Invoke(peerId, message);
    }
}
