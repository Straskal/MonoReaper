using Engine;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Adventure
{
    public abstract class Entity
    {
        public int Id { get; set; }
        public int OwnerId { get; set; }
        public World World { get; set; }
        public abstract EntityType Type { get; }
        public bool IsActive { get; set; }
        public HashSet<string> Tags { get; } = new();
        public Vector2 Position { get; set; }
        public Collider Collider { get; set; }
        public int DrawOrder { get; set; }
        public bool IsNetEntity { get; set; }
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

        public virtual void ReadFromEntityMessage(Message message) 
        {
        }

        public virtual void ServerWriteToSnapshot(Message message)
        {

        }

        public virtual void ClientReadFromSnapshot(Message message)
        {

        }
    }
}
