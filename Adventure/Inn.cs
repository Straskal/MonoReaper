using Adventure.Content;
using Adventure.Entities;
using Engine;
using Microsoft.Xna.Framework;

namespace Adventure
{
    public class Inn : AdventureStateBase
    {
        public Inn(Adventure adventure)
        {
            Adventure = adventure;
            World = new World();
        }

        public override AdventureState Type => AdventureState.Inn;
        public Adventure Adventure { get; }
        public World World { get; }
        public TopDownPlayer LocalPlayer { get; set; }

        public override void Start()
        {
            if (Adventure.Session.IsServer)
            {
                World.Spawn(Adventure.Content.Load<LevelData>("Levels/world/level_0").GetEntities());
            }

            LocalPlayer = World.FindLocalEntity<TopDownPlayer>();
        }

        public override void Stop()
        {
        }

        public override void AddPlayer(PlayerProfile player)
        {
            if (Session.Instance.IsServer)
            {
                var entity = new TopDownPlayer();
                entity.OwnerId = player.Id;
                World.Spawn(entity);
            }
        }

        public override void RemovePlayer(PlayerProfile player)
        {
            if (Session.Instance.IsServer)
            {
                var entity = World.FindFirst<TopDownPlayer>(p => p.OwnerId == player.Id);
                if (entity != null) 
                {
                    World.Destroy(entity);
                }
            }
        }

        public override void Update(GameTime gameTime)
        {
            World.Update(gameTime);
        }

        public override void Draw(GameTime gameTime, Renderer renderer)
        {
            renderer.Begin();
            World.Draw(renderer, gameTime);
            var source = new Rectangle(8, 0, 8, 8);
            var cursorOffset = source.Size.ToVector2() / 2f;
            var cursorPosition = Vector2.Floor(Engine.Input.MousePosition) - cursorOffset;
            renderer.Draw(Store.Gfx.Cursor, cursorPosition, source, Color.White);
            renderer.End();
        }

        public override void ServerWriteToInitialMessage(Message message)
        {
            World.ServerWriteToSnapshot(message);
        }

        public override void ClientReadFromInitialMessage(Message message)
        {
            World.ClientReadFromSnapshot(message);
        }

        public override void ServerWriteToSnapshot(Message message)
        {
            World.ServerWriteToSnapshot(message);
        }

        public override void ClientReadFromSnapshot(Message message)
        {
            World.ClientReadFromSnapshot(message);
        }

        public override void ReceiveEntityMessage(int entityId, Message message)
        {
            World.ReadFromEntityMessage(entityId, message);
        }
    }
}
