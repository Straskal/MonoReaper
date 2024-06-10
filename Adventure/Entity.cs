using Adventure.Networking;
using Engine;
using LiteNetLib.Utils;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Adventure
{
    public struct EntityPositionInterpolation 
    {
        public Vector2 From;
        public Vector2 To;
    }

    public abstract class Entity
    {
        public int Id { get; set; }
        public World World { get; set; }
        public int DrawOrder { get; set; }
        public abstract EntityType Type { get; }
        public int OwnerId { get; set; }
        public bool IsActive { get; set; }
        public HashSet<string> Tags { get; } = new();
        public Vector2 Position { get; set; }
        public Collider Collider { get; set; }
        public bool IsSyncEnabled { get; set; }
        public Vector2 ClientInterpolateFrom { get; set; }
        public Vector2 ClientInterpolateTo { get; set; }

        public bool IsServer => Adventure.Instance.Session.IsServer;
        public bool IsClient => !Adventure.Instance.Session.IsServer;
        public bool IsLocal => Adventure.Instance.Player.Id == OwnerId;
        public bool IsRemote => Adventure.Instance.Player.Id != OwnerId;


        public virtual void Spawn()
        {
        }

        public virtual void Destroy()
        {
        }

        public virtual void Update(GameTime gameTime)
        {
        }

        public virtual void PostUpdate(GameTime gameTime)
        {
        }

        public virtual void Draw(Renderer renderer, GameTime gameTime)
        {
        }

        public virtual void DebugDraw(Renderer renderer)
        {
        }

        public virtual void ReadFromEntityMessage(Message buffer) 
        {
        }

        public virtual void ServerWriteToSnapshot(Message buffer)
        {

        }

        public virtual void ClientReadFromSnapshot(Message buffer)
        {

        }
    }
}
