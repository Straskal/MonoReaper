using Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Diagnostics;

namespace Adventure
{
    public sealed class Adventure : Game
    {
        public const int ResolutionWidth = 256;
        public const int ResolutionHeight = 256;
        public const int HostPort = 7777;
        public const int MaxConnections = 3;
        public const int SnapshotMilliseconds = 100;
        public const float SnapshotSeconds = SnapshotMilliseconds / 1000f;

        private int _snapshotTimerMilliseconds = 0;
        private int _nextPlayerId = 1;

        public Adventure()
        {
            Instance = this;
            Window.Title = "The Trail of Adventure";
            Window.AllowUserResizing = true;
            Window.ClientSizeChanged += (s, e) => RenderTarget?.SyncWithBackBuffer();
            IsMouseVisible = false;
            Session = new LiteSession();
            Content = new ContentManager(Services, "Content");
            GraphicsDeviceManager = new GraphicsDeviceManager(this);
            GraphicsDeviceManager.HardwareModeSwitch = false;
            GraphicsDeviceManager.IsFullScreen = false;
            GraphicsDeviceManager.PreferredBackBufferWidth = ResolutionWidth;
            GraphicsDeviceManager.PreferredBackBufferHeight = ResolutionHeight;
            IsFixedTimeStep = true;
            Session.PeerConnected += OnPeerConnected;
            Session.PeerDisconnected += OnPeerDisconnected;
            Session.EventReceived += OnReceiveMessage;
            OtherPlayers = new List<PlayerProfile>();
        }

        public static Adventure Instance { get; private set; }
        public static GameTime Time { get; private set; }

        public PlayerProfile Player { get; private set; }
        public List<PlayerProfile> OtherPlayers { get; }
        public Session Session { get; }
        public GraphicsDeviceManager GraphicsDeviceManager { get; }
        public Renderer Renderer { get; private set; }
        public FixedResolutionRenderTarget2D RenderTarget { get; private set; }
        public CoroutineRunner Coroutines { get; } = new();
        public AdventureStateBase CurrentState { get; private set; }
        public AdventureStateBase NextState { get; private set; }
        public int ClientLastProcessedSnapshotTick { get; private set; }

        protected override void Initialize()
        {
            // Internally calls LoadContent()
            base.Initialize();

            Renderer = new Renderer(GraphicsDevice);
            RenderTarget = new FixedResolutionRenderTarget2D(GraphicsDevice, ResolutionWidth, ResolutionHeight);

            // TODO: Load profile from disk.
            Player = new PlayerProfile();
            Player.Id = 0;
            Player.Name = "Character name";
            Player.IsProfileLoaded = true;

            #region LOCAL TESTING

            try
            {
                Session.JoinSession("127.0.0.1", HostPort);
                CurrentState = new BlankState();
            }
            catch
            {
                Session.HostSession(HostPort);
                CurrentState = new InnState(this);
                CurrentState.Start();
            }

            #endregion
        }

        protected override void LoadContent()
        {
            Store.Fonts.Default = Content.Load<SpriteFont>("fonts/font");
            Store.Gfx.Cursor = Content.Load<Texture2D>("art/cursor");
            Store.Gfx.Player = Content.Load<Texture2D>("art/player/player");
            Store.Gfx.Fire = Content.Load<Texture2D>("art/player/fire");
            Store.Gfx.Barrel = Content.Load<Texture2D>("art/common/barrel");
            Store.Gfx.Explosion = Content.Load<Texture2D>("art/common/explosion-1");
            Store.Gfx.LargeDoor = Content.Load<Texture2D>("art/large_door");
            Store.Gfx.PressurePlate = Content.Load<Texture2D>("art/pressure_plate");
            Store.Vfx.SolidColor = Content.Load<Effect>("shaders/SolidColor");
            Store.Sfx.Shoot = Content.Load<SoundEffect>("audio/fireball_shoot");
            Store.Sfx.Explosion = Content.Load<SoundEffect>("audio/explosion4");

            base.LoadContent();
        }

        protected override void UnloadContent()
        {
            RenderTarget.Dispose();
            Renderer.Dispose();
            Content.Unload();

            base.UnloadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            Time = gameTime;
            Input.Update(RenderTarget);
            Coroutines.Update();
            Session.Update(gameTime);
            CurrentState.Update(gameTime);

            if (!Session.IsServer)
            {
                return;
            }

            if (NextState != null)
            {
                CurrentState.Stop();
                CurrentState = NextState;
                CurrentState.Start();
                NextState = null;

                ServerSendStateChangeEvent();

                _snapshotTimerMilliseconds = 0;
            }

            _snapshotTimerMilliseconds += (int)gameTime.ElapsedGameTime.TotalMilliseconds;

            if (_snapshotTimerMilliseconds >= SnapshotMilliseconds)
            {
                _snapshotTimerMilliseconds = 0;

                ServerSendSnapshotEvent();
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.SetRenderTarget(RenderTarget);
            GraphicsDevice.Viewport = RenderTarget.Viewport;
            GraphicsDevice.Clear(Color.Black);
            CurrentState.Draw(Time, Renderer);
            GraphicsDevice.SetRenderTarget(null);
            GraphicsDevice.Viewport = RenderTarget.LetterboxViewport;
            GraphicsDevice.Clear(Color.Black);
            Renderer.Begin(RenderTarget.ScaleMatrix);
            Renderer.Draw(RenderTarget, Vector2.Zero, Color.White);
            Renderer.End();
        }

        private void OnPeerConnected(int peerId)
        {
            var peer = new PlayerProfile();

            OtherPlayers.Add(peer);

            if (Session.IsServer) 
            {
                peer.PeerId = peerId;
                peer.Id = _nextPlayerId++;

                ServerSendWelcomeMessage(peer);
            }
            else
            {
                ClientSendProfileMessage();
            }
        }

        private void OnPeerDisconnected(int peerId)
        {
            if (Session.IsServer)
            {
                PlayerProfile player = null;

                foreach (var other in OtherPlayers) 
                {
                    if (other.PeerId == peerId) 
                    {
                        player = other;
                        return;
                    }
                }

                if (player == null) 
                {
                    Debug.WriteLine($"Peer {peerId} has disconnected, but no player profile was found.");
                    return;
                }

                CurrentState.RemovePlayer(player);
                OtherPlayers.Remove(player);

                SendPlayerDisconnectedMessage(player.Id);
            }
            else
            {
                OtherPlayers.Clear();
                Session.StopSession();
                Session.HostSession(HostPort);
                CurrentState = new InnState(this);
                CurrentState.Start();
            }
        }

        private void OnReceiveMessage(int peerId, Message message)
        {
            var type = (MessageType)message.ReadByte();

            switch (type)
            {
                case MessageType.WelcomeMessage:
                    ReceiveWelcomeMessage(message);
                    break;
                case MessageType.ProfileMessage:
                    ReceiveProfileMessage(peerId, message);
                    break;
                case MessageType.PlayerDisconnectedMessage:
                    ReceivePlayerDisconnectedMessage(message);
                    break;
                case MessageType.StateChangedMessage:
                    ReceiveStateChange(message);
                    break;
                case MessageType.StateMessage:
                    CurrentState.ReceiveStateMessage(message);
                    break;
                case MessageType.SnapshotMessage:
                    ReceiveSnapshot(message);
                    break;
                case MessageType.EntityMessage:
                    var entityId = message.ReadInt();
                    CurrentState.ReceiveEntityMessage(entityId, message);
                    break;
            }
        }

        private void SendPlayerDisconnectedMessage(int playerId)
        {
            var message = new Message();
            message.Write((byte)MessageType.PlayerDisconnectedMessage);
            message.Write(playerId);

            Session.ServerSendReliable(message);
        }

        private void ReceivePlayerDisconnectedMessage(Message message) 
        {
            var playerId = message.ReadInt();

            PlayerProfile player = null;

            foreach (var other in OtherPlayers)
            {
                if (other.Id == playerId)
                {
                    player = other;
                    return;
                }
            }

            if (player == null) 
            {
                Debug.WriteLine($"Player {playerId} has disconnected, but no player profile was found.");
                return;
            }

            CurrentState.RemovePlayer(player);
            OtherPlayers.Remove(player);
        }

        private void ServerSendWelcomeMessage(PlayerProfile player)
        {
            var message = new Message();
            message.Write((byte)MessageType.WelcomeMessage);

            // Give player their ID.
            message.Write(player.Id);

            // Write host profile.
            message.Write(Player.Name);

            // Write other remote profiles.
            message.Write(OtherPlayers.Count - 1);
            foreach (var other in OtherPlayers)
            {
                if (other.Id != player.Id)
                {
                    message.Write(other.Id);
                    message.Write(other.Name);
                }
            }

            // Write the initial game state.
            message.Write((int)CurrentState.Type);
            CurrentState.ServerWriteToInitialMessage(message);

            Session.ServerSendReliable(player.PeerId, message);
        }

        private void ReceiveWelcomeMessage(Message message)
        {
            Debug.Assert(OtherPlayers.Count == 1);

            Player.Id = message.ReadInt();

            OtherPlayers[0].Id = 0;
            OtherPlayers[0].Name = message.ReadString();
            OtherPlayers[0].IsProfileLoaded = true;

            var otherPlayerCount = message.ReadInt();

            for (int i = 0; i < otherPlayerCount; i++)
            {
                var otherPlayer = new PlayerProfile();
                otherPlayer.Id = message.ReadInt();
                otherPlayer.Name = message.ReadString();
                otherPlayer.IsProfileLoaded = true;

                OtherPlayers.Add(otherPlayer);
            }

            var stateType = (AdventureState)message.ReadInt();

            switch (stateType)
            {
                case AdventureState.Inn:
                    CurrentState = new InnState(this);
                    break;
            }

            CurrentState.Start();
            CurrentState.ClientReadFromInitialMessage(message);
        }

        private void ClientSendProfileMessage()
        {
            var message = new Message();
            message.Write((byte)MessageType.ProfileMessage);
            message.Write(Player.Name);

            Session.ClientSendReliable(message);
        }

        private void ServerSendProfileMessage(PlayerProfile player)
        {
            var message = new Message();
            message.Write((byte)MessageType.ProfileMessage);
            message.Write(player.Id);
            message.Write(player.Name);

            foreach (var other in OtherPlayers)
            {
                if (other.PeerId != player.PeerId)
                {
                    Session.ServerSendReliable(other.PeerId, message);
                }
            }
        }

        private void ReceiveProfileMessage(int peerId, Message message)
        {
            if (Session.IsServer)
            {
                PlayerProfile player = null;

                foreach (var otherPlayer in OtherPlayers)
                {
                    if (otherPlayer.PeerId == peerId)
                    {
                        player = otherPlayer;
                        break;
                    }
                }

                if (player == null)
                {
                    // TODO: Log?
                    return;
                }

                player.Name = message.ReadString();
                player.IsProfileLoaded = true;
                CurrentState.AddPlayer(player);

                ServerSendProfileMessage(player);

            }
            else
            {
                var id = message.ReadInt();
                var name = message.ReadString();

                PlayerProfile player = null;

                foreach (var otherPlayer in OtherPlayers)
                {
                    if (otherPlayer.Id == id)
                    {
                        player = otherPlayer;
                        break;
                    }
                }

                // If we don't have a profile yet, then create it instead of updating it.
                player ??= new PlayerProfile();
                player.Id = id;
                player.Name = name;
                player.IsProfileLoaded = true;

                OtherPlayers.Add(player);
            }
        }

        private void ServerSendSnapshotEvent()
        {
            var message = new Message();
            message.Write((byte)MessageType.SnapshotMessage);
            message.Write(Session.Tick);
            message.Write((byte)CurrentState.Type);
            CurrentState.ServerWriteToSnapshot(message);
            Session.ServerSendUnreliable(message);
        }

        private void ReceiveSnapshot(Message message)
        {
            var serverTick = message.ReadInt();
            if (Timestep.TickDiff(serverTick, ClientLastProcessedSnapshotTick) <= 0)
            {
                return;
            }

            var state = (AdventureState)message.ReadByte();
            if (state == CurrentState.Type)
            {
                ClientLastProcessedSnapshotTick = serverTick;
                CurrentState.ClientReadFromSnapshot(message);
            }
        }

        private void ServerSendStateChangeEvent()
        {
            var message = new Message();
            message.Write((byte)MessageType.StateChangedMessage);
            message.Write((byte)CurrentState.Type);
            CurrentState.ServerWriteToInitialMessage(message);
            Session.ServerSendReliable(message);
        }

        private void ReceiveStateChange(Message message)
        {
            switch ((AdventureState)message.ReadByte())
            {
                case AdventureState.Inn:
                    CurrentState = new InnState(this);
                    break;
            }

            CurrentState.Start();
            CurrentState.ClientReadFromInitialMessage(message);
        }
    }
}
