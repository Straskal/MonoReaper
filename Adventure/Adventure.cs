using Adventure.Networking;
using Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Adventure
{
    public static class AdventureSettings
    {
        public const int HostPort = 7777;
        public const int SnapshotMilliseconds = 100;
        public const float SnapshotSeconds = SnapshotMilliseconds / 1000f;
        public const int ResolutionWidth = 256;
        public const int ResolutionHeight = 256;
    }

    public sealed class Adventure : Game
    {
        private int _snapshotTimerMilliseconds = 0;
        private int _currentPlayerId = 1;

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
            GraphicsDeviceManager.PreferredBackBufferWidth = AdventureSettings.ResolutionWidth;
            GraphicsDeviceManager.PreferredBackBufferHeight = AdventureSettings.ResolutionHeight;
            IsFixedTimeStep = true;
            Session.PeerConnected += OnPeerConnected;
            Session.PeerDisconnected += OnPeerDisconnected;
            Session.EventReceived += OnReceiveMessage;
            RemotePlayers = new List<PlayerProfile>();
        }

        public static Adventure Instance { get; private set; }
        public static GameTime Time { get; private set; }

        public PlayerProfile Player { get; private set; }
        public List<PlayerProfile> RemotePlayers { get; }
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
            RenderTarget = new FixedResolutionRenderTarget2D(GraphicsDevice, AdventureSettings.ResolutionWidth, AdventureSettings.ResolutionHeight);

            // TODO: Load profile from disk.
            Player = new PlayerProfile();
            Player.Id = 0;
            Player.Name = "Character name";
            Player.IsProfileLoaded = true;

            // Initialize state after content has loaded.
            try
            {
                Session.Join("127.0.0.1", AdventureSettings.HostPort);
                CurrentState = new BlankState();
            }
            catch
            {
                Session.Host(AdventureSettings.HostPort);
                CurrentState = new Inn(this);
                CurrentState.Start();
            }
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

            if (_snapshotTimerMilliseconds >= AdventureSettings.SnapshotMilliseconds)
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
            var profile = new PlayerProfile();
            profile.Id = _currentPlayerId++;
            profile.IsProfileLoaded = false;
            RemotePlayers.Add(profile);

            if (Session.IsServer)
            {
                ServerSendWelcomeMessage(profile);
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
                var player = RemotePlayers.SingleOrDefault(p => p.PeerId == peerId);
                if (player != null)
                {
                    CurrentState.PlayerLeft(player);
                    RemotePlayers.Remove(player);
                    ServerSendPlayerDisconnectedMessage(player.Id);
                }
            }
            else
            {
                RemotePlayers.Clear();
                Session.Host(AdventureSettings.HostPort);
                CurrentState = new Inn(this);
                CurrentState.Start();
            }
        }

        private void OnReceiveMessage(int peerId, Message message)
        {
            var type = (MessageType)message.ReadByte();

            switch (type)
            {
                case MessageType.ProfileMessage:
                    ReceiveProfileMessage(peerId, message);
                    break;
                case MessageType.PlayerDisconnected:
                    ReceivePlayerDisconnectedMessage(message);
                    break;
                case MessageType.WelcomeMessage:
                    ClientReceiveWelcomeMessage(message);
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

        private void ServerSendPlayerDisconnectedMessage(int playerId)
        {
            var message = new Message();
            message.Write((byte)MessageType.PlayerDisconnected);
            message.Write(playerId);
            Session.ServerSendReliable(message);
        }

        private void ReceivePlayerDisconnectedMessage(Message message) 
        {
            var playerId = message.ReadInt();
            var player = RemotePlayers.SingleOrDefault(p => p.Id == playerId);

            if (player != null)
            {
                CurrentState.PlayerLeft(player);
                RemotePlayers.Remove(player);
            }
        }

        private void ServerSendWelcomeMessage(PlayerProfile player)
        {
            var welcome = new Message();
            welcome.Write((byte)MessageType.WelcomeMessage);
            welcome.Write(player.Id);
            welcome.Write(Player.Name);

            welcome.Write(RemotePlayers.Count - 1);
            foreach (var other in RemotePlayers)
            {
                if (other.Id != player.Id)
                {
                    welcome.Write(other.Id);
                    welcome.Write(other.Name);
                }
            }

            // Write the current initial state.
            welcome.Write((int)CurrentState.Type);
            CurrentState.ServerWriteToInitialMessage(welcome);
            Session.ServerSendReliable(player.PeerId, welcome);
        }

        private void ClientReceiveWelcomeMessage(Message message)
        {
            Player.Id = message.ReadInt();

            var hostProfile = RemotePlayers.SingleOrDefault() ?? throw new Exception();
            hostProfile.Name = message.ReadString();
            hostProfile.IsProfileLoaded = true;

            var otherPlayerCount = message.ReadInt();

            for (int i = 0; i < otherPlayerCount; i++)
            {
                var profile = new PlayerProfile();
                profile.PeerId = message.ReadInt();
                profile.Name = message.ReadString();
                profile.IsProfileLoaded = true;
                RemotePlayers.Add(profile);
            }

            var stateType = (AdventureState)message.ReadInt();

            switch (stateType)
            {
                case AdventureState.Inn:
                    CurrentState = new Inn(this);
                    break;
            }

            CurrentState.Start();
            CurrentState.ClientReadFromInitialMessage(message);
        }

        private void ClientSendProfileMessage()
        {
            var profileMessage = new Message();
            profileMessage.Write((byte)MessageType.ProfileMessage);
            profileMessage.Write(Player.Name);
            Session.ClientSendReliable(profileMessage);
        }

        private void ReceiveProfileMessage(int peerId, Message message)
        {
            if (Session.IsServer)
            {
                PlayerProfile profile = null;

                foreach (var player in RemotePlayers)
                {
                    if (player.PeerId == peerId)
                    {
                        profile = player;
                        break;
                    }
                }

                if (profile == null)
                {
                    // TODO: Log?
                    return;
                }

                profile.Name = message.ReadString();
                profile.IsProfileLoaded = true;

                var broadcast = new Message();
                broadcast.Write((int)MessageType.ProfileMessage);
                broadcast.Write(peerId);
                broadcast.Write(profile.Name);

                foreach (var other in RemotePlayers)
                {
                    if (other.PeerId != profile.PeerId)
                    {
                        Session.ServerSendReliable(other.PeerId, broadcast);
                    }
                }

                CurrentState.PlayerJoined(profile);
            }
            else
            {
                var id = message.ReadInt();
                var name = message.ReadString();

                PlayerProfile profile = null;

                foreach (var player in RemotePlayers)
                {
                    if (player.PeerId == id)
                    {
                        profile = player;
                        break;
                    }
                }

                profile ??= new PlayerProfile();
                profile.PeerId = id;
                profile.Name = name;
                profile.IsProfileLoaded = true;

                RemotePlayers.Add(profile);
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

        private void ReceiveSnapshot(Message buffer)
        {
            var tick = buffer.ReadInt();
            if (Timestep.TickDiff(tick, ClientLastProcessedSnapshotTick) <= 0)
            {
                return;
            }

            ClientLastProcessedSnapshotTick = tick;

            var state = (AdventureState)buffer.ReadByte();
            if (state == CurrentState.Type)
            {
                CurrentState.ClientReadFromSnapshot(buffer);
            }
        }

        private void ServerSendStateChangeEvent()
        {
            var buffer = new Message();
            buffer.Write((byte)MessageType.StateChangedMessage);
            buffer.Write((byte)CurrentState.Type);
            CurrentState.ServerWriteToInitialMessage(buffer);
            Session.ServerSendReliable(buffer);
        }

        private void ReceiveStateChange(Message buffer)
        {
            switch ((AdventureState)buffer.ReadByte())
            {
                case AdventureState.Inn:
                    CurrentState = new Inn(this);
                    break;
            }

            CurrentState.Start();
            CurrentState.ClientReadFromInitialMessage(buffer);
        }
    }
}
