using Engine;
using Engine.Extensions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

using static Adventure.Constants;

namespace Adventure.Entities
{
    public class TopDownPlayer : Character
    {
        public const int ClientInputMessageIntervalMilliseconds = 33;
        public const int ClientInputBufferCapacity = 100;
        public const float Speed = 50f;
        public const float MaxSpeed = 0.75f;

        public override EntityType Type => EntityType.Player;

        private int _clientInputMessageTimerMilliseconds = 0;
        private int _remoteInterpolationTimerTicks = 0;

        public Sprite Sprite { get; set; }
        public Animator Animator { get; set; }
        public Vector2 FaceDirection { get; private set; }

        public int ServerLastProcessedClientInput { get; set; }
        public Queue<Vector2Snapshot> ServerInputBuffer { get; private set; } = new();
        public Queue<ClientLocalSnapshot> ClientSnapshotBuffer { get; private set; } = new();

        public TopDownPlayer()
        {
            IsNetEntity = true;
        }

        public override void Spawn()
        {
            Sprite = new Sprite(Store.Gfx.Player);
            Animator = new Animator(Sprite, PlayerAnimations.Frames);
            Collider = new Collider(this, new BoxCollisionShape(12, 16));
            Collider.Layer = EntityLayers.Player;
            Collider.Enable();
        }

        public override void Update(GameTime gameTime)
        {
            var deltaTime = gameTime.GetDeltaTime();

            if (IsLocal)
            {
                var movementInput = Input.GetVector(Keys.A, Keys.D, Keys.W, Keys.S);

                ApplyMovementInput(movementInput, deltaTime);

                if (IsClient)
                {
                    // Keep the input buffer circular and let go of old records if we exceed the max.
                    var localSnapshot = new ClientLocalSnapshot
                    {
                        Input = new Vector2Snapshot(Session.Instance.Tick, movementInput),
                        Position = new Vector2Snapshot(Session.Instance.Tick, Position)
                    };

                    ClientSnapshotBuffer.Enqueue(localSnapshot);
                    if (ClientSnapshotBuffer.Count > 100)
                    {
                        ClientSnapshotBuffer.Dequeue();
                    }

                    _clientInputMessageTimerMilliseconds += (int)gameTime.ElapsedGameTime.TotalMilliseconds;

                    if (_clientInputMessageTimerMilliseconds >= ClientInputMessageIntervalMilliseconds)
                    {
                        _clientInputMessageTimerMilliseconds = 0;

                        if (ClientSnapshotBuffer.Count > 0)
                        {
                            ClientSendInputMessage();
                        }
                    }
                }
            }
            else
            {
                if (IsServer)
                {
                    while (ServerInputBuffer.TryDequeue(out var input))
                    {
                        ApplyMovementInput(input.Value, Adventure.Time.GetDeltaTime());
                    }
                }
                else
                {
                    _remoteInterpolationTimerTicks = Timestep.IncrementTick(_remoteInterpolationTimerTicks);

                    var elapsed = Timestep.TickDiff(_remoteInterpolationTimerTicks, Adventure.Instance.ClientLastProcessedSnapshotTick) * Timestep.FixedDelta;
                    var percent = Math.Clamp(elapsed / (Adventure.SnapshotSeconds + Session.Instance.Latency), 0f, 1f);

                    Position = Vector2.Lerp(ClientInterpolateFrom, ClientInterpolateTo, percent);
                    Position = Vector2.Round(Position);

                    Collider.Update();
                }
            }

            Animate(gameTime);
        }

        public override void Draw(Renderer renderer, GameTime gameTime)
        {
            renderer.Draw(Sprite, Position - new Vector2(8, 8));
            renderer.DrawString(Store.Fonts.Default, OwnerId.ToString(), Position + new Vector2(0, -25), Color.White);
        }

        private void ApplyMovementInput(Vector2 input, float deltaTime)
        {
            SlideMove(input * Speed * deltaTime);
        }

        private void Animate(GameTime gameTime)
        {
            if (Math.Abs(FaceDirection.X) > Math.Abs(FaceDirection.Y))
            {
                if (FaceDirection.X < 0f)
                {
                    Animator.Play("walk_left");
                }
                else
                {
                    Animator.Play("walk_right");
                }
            }
            else
            {
                if (FaceDirection.Y < 0f)
                {
                    Animator.Play("walk_up");
                }
                else
                {
                    Animator.Play("walk_down");
                }
            }

            Animator.RunFrame(gameTime);
        }

        public override void ReadFromEntityMessage(Message buffer)
        {
            var type = (EntityMessageType)buffer.ReadByte();

            switch (type)
            {
                case EntityMessageType.InputMessage:
                    ServerReadInputMessage(buffer);
                    break;
            }
        }

        public void ClientSendInputMessage()
        {
            var message = new Message();
            message.Write((byte)MessageType.EntityMessage);
            message.Write(Id);
            message.Write((byte)EntityMessageType.InputMessage);
            message.Write(ClientSnapshotBuffer.Count);

            foreach (var localSnapshot in ClientSnapshotBuffer)
            {
                message.Write(localSnapshot.Input.Tick);
                message.Write(localSnapshot.Input.Value);
            }

            Session.Instance.ClientSendUnreliable(message);
        }

        public void ServerReadInputMessage(Message message)
        {
            var count = message.ReadInt();
            for (int i = 0; i < count; i++)
            {
                var tick = message.ReadInt();
                var movement = message.ReadVector2();

                if (Timestep.TickDiff(tick, ServerLastProcessedClientInput) > 0)
                {
                    ServerInputBuffer.Enqueue(new Vector2Snapshot(tick, movement));
                    ServerLastProcessedClientInput = tick;
                }
            }
        }

        public override void ServerWriteToSnapshot(Message buffer)
        {
            buffer.Write(Position.X);
            buffer.Write(Position.Y);
            buffer.Write(ServerLastProcessedClientInput);
        }

        public override void ClientReadFromSnapshot(Message buffer)
        {
            var positionX = buffer.ReadSingle();
            var positionY = buffer.ReadSingle();
            var lastProcessedInputTick = buffer.ReadInt();

            if (IsRemote)
            {
                // Reset interpolation timer to the latest snapshot server tick.
                _remoteInterpolationTimerTicks = Adventure.Instance.ClientLastProcessedSnapshotTick;

                ClientInterpolateFrom = ClientInterpolateTo;
                ClientInterpolateTo = new Vector2(positionX, positionY);
                return;
            }

            if (ClientSnapshotBuffer.Count == 0)
            {
                // We don't have anything to compare against, so lets just take what the server gave us.
                // TODO: This feels jank.
                Position = new Vector2(positionX, positionY);
                Collider.Update();
                return;
            }

            ClientLocalSnapshot? snapshotAtTick = null;

            while (ClientSnapshotBuffer.TryPeek(out var localSnapshot))
            {
                if (Timestep.TickDiff(localSnapshot.Input.Tick, lastProcessedInputTick) > 0)
                {
                    break;
                }

                snapshotAtTick = ClientSnapshotBuffer.Dequeue();
            }

            if (snapshotAtTick != null)
            {
                const float Threshold = 1f;

                var diffX = snapshotAtTick.Value.Position.Value.X - positionX;
                var diffY = snapshotAtTick.Value.Position.Value.Y - positionY;

                // If the client position is too far from what the server has, then correct to the server position and reapply all local inputs.
                if (diffX > Threshold || diffY > Threshold)
                {
                    // Reset position and then replay all inputs.
                    Position = new Vector2(positionX, positionY);

                    foreach (var localSnapshot in ClientSnapshotBuffer)
                    {
                        ApplyMovementInput(localSnapshot.Position.Value, Adventure.Time.GetDeltaTime());
                    }
                }
            }
        }
    }

    public struct ClientLocalSnapshot
    {
        public Vector2Snapshot Input;
        public Vector2Snapshot Position;
    }

    public struct Vector2Snapshot
    {
        public Vector2Snapshot(int tick, Vector2 value)
        {
            Tick = tick;
            Value = value;
        }

        public int Tick;
        public Vector2 Value;
    }
}
