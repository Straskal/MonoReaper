using LiteNetLib;
using Microsoft.Xna.Framework;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;

namespace Adventure
{
    internal sealed class LiteSession : Session, INetEventListener
    {
        private readonly NetManager _manager;

        private NetPeer _server;

        public LiteSession()
        {
            Instance = this;

            _manager = new NetManager(this)
            {
                IPv6Enabled = false,
#if DEBUG
                // Increase timeout for debugging so that breakpoints do not cause a disconnect.
                DisconnectTimeout = 1000 * 60
#endif
            };
        }

        public void OnConnectionRequest(ConnectionRequest request)
        {
            Debug.WriteLine("Connection request received");

            if (_manager.ConnectedPeersCount == 3) 
            {
                Debug.WriteLine("Connection request rejected because server is full");
                request.Reject();
                return;
            }

            Debug.WriteLine("Connection request accepted");
            request.Accept();
        }

        public void OnPeerConnected(NetPeer peer)
        {
            InvokePeerConnected(peer.Id);
        }

        public void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
        {
            InvokePeerDisconnected(peer.Id);
        }

        public void OnNetworkReceive(NetPeer peer, NetPacketReader reader, byte channelNumber, DeliveryMethod deliveryMethod)
        {
            InvokeEventReceived(peer.Id, new Message(reader.GetRemainingBytes()));
        }

        public void OnNetworkReceiveUnconnected(IPEndPoint remoteEndPoint, NetPacketReader reader, UnconnectedMessageType messageType)
        {
            // Don't care
        }

        public void OnNetworkError(IPEndPoint endPoint, SocketError socketError)
        {
            Debug.WriteLine($"Network error received: {socketError}");
        }

        public void OnNetworkLatencyUpdate(NetPeer peer, int latency)
        {
            Latency = latency;
        }

        public override void HostSession(int port)
        {
            StopSession();
            _manager.Start(port);
            IsServer = true;
        }

        public override void JoinSession(string ipAddress, int port)
        {
            StopSession();
            _manager.Start();
            _server = _manager.Connect(ipAddress, port, "dummy_key");
            IsServer = false;
        }

        public override void StopSession()
        {
            if (_server != null)
            {
                _server.Disconnect();
                _server = null;
            }
            else
            {
                _manager.DisconnectAll();
            }

            _manager.Stop();
            IsServer = false;
        }

        public override void Update(GameTime gameTime)
        {
            Tick = Timestep.IncrementTick(Tick);
            _manager.PollEvents();
        }

        public override void ClientSendReliable(global::Adventure.Message message)
        {
            Send(message, DeliveryMethod.ReliableOrdered);
        }

        public override void ClientSendUnreliable(global::Adventure.Message message)
        {
            Send(message, DeliveryMethod.Unreliable);
        }

        public override void ServerSendReliable(int id, global::Adventure.Message message)
        {
            Send(id, message, DeliveryMethod.ReliableOrdered);
        }

        public override void ServerSendUnreliable(int id, global::Adventure.Message message)
        {
            Send(id, message, DeliveryMethod.Unreliable);
        }

        public override void ServerSendReliable(global::Adventure.Message message)
        {
            SendToAll(message, DeliveryMethod.ReliableOrdered);
        }

        public override void ServerSendUnreliable(Message message)
        {
            SendToAll(message, DeliveryMethod.Unreliable);
        }

        private void Send(Message message, DeliveryMethod devivery)
        {
            _server.Send(message.ToReadonlySpan(), devivery);
        }

        private void Send(int id, Message message, DeliveryMethod devivery)
        {
            _manager.GetPeerById(id).Send(message.ToReadonlySpan(), devivery);
        }

        private void SendToAll(Message message, DeliveryMethod devivery)
        {
            _manager.SendToAll(message.ToReadonlySpan(), devivery);
        }
    }
}
