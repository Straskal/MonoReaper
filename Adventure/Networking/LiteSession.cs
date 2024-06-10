using LiteNetLib;
using Microsoft.Xna.Framework;
using System;
using System.Net;
using System.Net.Sockets;

namespace Adventure.Networking
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
                DisconnectTimeout = 1000 * 60
#endif
            };
        }

        public void OnConnectionRequest(ConnectionRequest request)
        {
            Console.WriteLine("Connection Request");

            // TODO: Basic accept is for testing.
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
        }

        public void OnNetworkError(IPEndPoint endPoint, SocketError socketError)
        {
        }

        public void OnNetworkLatencyUpdate(NetPeer peer, int latency)
        {
            Latency = latency;
        }

        public override void Host(int port)
        {
            StopSession();
            _manager.Start(port);
            IsServer = true;
        }

        public override void Join(string ipAddress, int port)
        {
            StopSession();
            _manager.Start();
            _server = _manager.Connect(ipAddress, port, "key");
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
