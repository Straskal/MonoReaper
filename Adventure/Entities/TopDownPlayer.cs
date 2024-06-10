using Adventure.Networking;
using Engine;
using Engine.Extensions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

using static Adventure.Constants;

namespace Adventure.Entities
{
    public class TopDownPlayer : Entity
    {
        public const float Speed = 50f;
        public const float MaxSpeed = 0.75f;

        public override EntityType Type => EntityType.Player;

        private const int InputRequestIntervalMilliseconds = 33;
        private int _inputRequestTimerMilliseconds = 0;

        public Sprite Sprite { get; set; }
        public Animator Animator { get; set; }
        public Vector2 FaceDirection { get; private set; }

        public int ServerLastProcessedClientInput { get; set; }
        public Queue<Input> ServerInputBuffer { get; private set; } = new();
        public RingBuffer<Input> ClientInputBuffer { get; private set; } = new(50);

        public override void Spawn()
        {
            Sprite = new Sprite(Store.Gfx.Player);
            Animator = new Animator(Sprite, PlayerAnimations.Frames);
            Collider = new Collider(this, new BoxCollisionShape(9f, 8f));
            Collider.Offset = new Vector2(0f, 4f);
            Collider.Layer = EntityLayers.Player;
            Collider.Enable();
        }

        public void SetMovementInput(Vector2 movementInput)
        {
            var movementLength = movementInput.LengthSquared();

            if (movementLength > 1f)
            {
                movementInput.Normalize();
            }

            ClientInputBuffer.Push(new Input(Adventure.Instance.Session.Tick, movementInput));
        }

        public void ApplyMovementInput(Vector2 input, float deltaTime)
        {
            Position += input * Speed * deltaTime;
            Position = Vector2.Round(Position);
        }

        private int SS_INTERPOLATE_TIMER = 0;

        public override void Update(GameTime gameTime)
        {
            SS_INTERPOLATE_TIMER = Timestep.IncrementTick(SS_INTERPOLATE_TIMER);

            var deltaTime = gameTime.GetDeltaTime();

            if (IsLocal)
            {
                var movementInput = Engine.Input.GetVector(Keys.A, Keys.D, Keys.W, Keys.S);

                ApplyMovementInput(movementInput, deltaTime);

                if (IsClient)
                {
                    ClientInputBuffer.Push(new Input(Session.Instance.Tick, movementInput));
                }

                if (IsClient)
                {
                    _inputRequestTimerMilliseconds += (int)gameTime.ElapsedGameTime.TotalMilliseconds;
                    
                    if (_inputRequestTimerMilliseconds >= InputRequestIntervalMilliseconds)
                    {
                        _inputRequestTimerMilliseconds = 0;

                        ClientSendInputMessage();
                    }
                }
            }
            else
            {
                if (IsServer)
                {
                    while (ServerInputBuffer.TryDequeue(out var input))
                    {
                        Position += input.Movement * Speed * deltaTime;
                        Position = Vector2.Round(Position);
                    }
                }
                else 
                {
                    var diff = Timestep.TickDiff(SS_INTERPOLATE_TIMER, Adventure.Instance.ClientLastProcessedSnapshotTick);
                    var elapsed = diff * Timestep.FixedDelta;
                    var percent = elapsed / (AdventureSettings.SnapshotSeconds + Session.Instance.Latency);

                    if (percent <= 1f)
                    {
                        Position = Vector2.Lerp(ClientInterpolateFrom, ClientInterpolateTo, percent);
                        Position = Vector2.Round(Position);
                    }
                }
            }

            Animate(gameTime);
        }

        public override void Draw(Renderer renderer, GameTime gameTime)
        {
            renderer.Draw(Sprite, Position);
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

        public void ClientSendInputMessage()
        {
            if (ClientInputBuffer.Count == 0) 
            {
                return;
            }

            var message = new Message();
            message.Write((byte)MessageType.EntityMessage);
            message.Write(Id);
            message.Write((byte)EntityMessageType.InputRequest);
            message.Write(ClientInputBuffer.Count);

            foreach (var input in ClientInputBuffer)
            {
                message.Write(input.Tick);
                message.Write(input.Movement);
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
                    ServerInputBuffer.Enqueue(new Input(tick, movement));
                    ServerLastProcessedClientInput = tick;
                }
            }
        }

        public override void ReadFromEntityMessage(Message buffer)
        {
            var type = (EntityMessageType)buffer.ReadByte();

            switch (type)
            {
                case EntityMessageType.InputRequest:
                    ServerReadInputMessage(buffer);
                    break;
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
            SS_INTERPOLATE_TIMER = Adventure.Instance.ClientLastProcessedSnapshotTick;

            var positionX = buffer.ReadSingle();
            var positionY = buffer.ReadSingle();
            var lastProcessedInputTick = buffer.ReadInt();

            if (IsClient && IsRemote)
            {
                ClientInterpolateFrom = ClientInterpolateTo;
                ClientInterpolateTo = new Vector2(positionX, positionY);
                return;
            }

            while (ClientInputBuffer.TryPeek(out var input))
            {
                if (Timestep.TickDiff(input.Tick, lastProcessedInputTick) > 0)
                {
                    break;
                }

                ClientInputBuffer.Pop();
            }

            // Reset position and then replay all inputs.
            Position = new Vector2(positionX, positionY);

            foreach (var input in ClientInputBuffer)
            {
                ApplyMovementInput(input.Movement, Adventure.Time.GetDeltaTime());
            }
        }
    }

    public struct Input
    {
        public Input(int tick, Vector2 movement)
        {
            Tick = tick;
            Movement = movement;
        }

        public int Tick;
        public Vector2 Movement;
    }
}
