using Engine;
using Microsoft.Xna.Framework;

namespace Adventure
{
    public enum AdventureState : byte
    {
        None,
        Inn,
        Travel,
        MaxStates
    }

    public abstract class AdventureStateBase 
    {
        public abstract AdventureState Type { get; }

        public virtual void Start() 
        {
        }

        public virtual void Stop() 
        {
        }

        public virtual void Update(GameTime gameTime) 
        {
        }

        public virtual void Draw(GameTime gameTime, Renderer renderer) 
        {
        }

        public virtual void AddPlayer(PlayerProfile player) 
        {
        }

        public virtual void RemovePlayer(PlayerProfile player) 
        {
        }

        public virtual void ServerWriteToInitialMessage(Message buffer)
        {
        }

        public virtual void ClientReadFromInitialMessage(Message buffer)
        {
        }

        public virtual void ServerWriteToSnapshot(Message buffer) 
        {
        }

        public virtual void ClientReadFromSnapshot(Message buffer) 
        {
        }

        public virtual void ReceiveStateMessage(Message buffer) 
        {
        }

        public virtual void ReceiveEntityMessage(int entityId, Message buffer)
        {
        }
    }
}
