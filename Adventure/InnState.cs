using Adventure.Content;
using Adventure.Entities;
using Engine;
using Microsoft.Xna.Framework;

namespace Adventure
{
    public class InnState : AdventureStateBase
    {
        public InnState(Adventure adventure)
        {
            Adventure = adventure;
            World = new World();
        }

        public override AdventureState Type => AdventureState.Inn;
        public Adventure Adventure { get; }
        public World World { get; }
        public SpawnPosition SpawnPosition { get; private set; }

        public override void Start()
        {
            // Note: Level data is really just a list of entities.
            // Can have some level files that only contain sets of entities that need to be spawned for certain scenarios.
            // Below, we are loading the static entities that will not be net synced.
            // But other files can contain the net entities that will sync from host to clients.
            World.Spawn(Adventure.Content.Load<LevelData>("Levels/world/level_0").GetEntities());

            // Only the server spawns in dynamic entities.
            if (!Adventure.Session.IsServer) 
            {
                return;
            }

            SpawnPosition = World.FindEntityOfType<SpawnPosition>();

            // A scenario could be randomly chosen.
            // var scenario = 1;
            // var scanarioPath = $"Levels/world/scenario{scenario}";

            // Other dynamic entities are loaded in. These are only loaded by the server.
            // The clients will load these objects once they receive the state snapshot.
            // World.Spawn(Adventure.Content.Load<LevelData>(scanarioPath).GetEntities());

            // Players should be spawned from spawn points.
            AddPlayer(Adventure.Player);

            foreach (var player in Adventure.OtherPlayers) 
            {
                AddPlayer(player);
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
            var cursorPosition = Vector2.Floor(Input.MousePosition) - cursorOffset;
            renderer.Draw(Store.Gfx.Cursor, cursorPosition, source, Color.White);
            renderer.End();
        }

        public override void AddPlayer(PlayerProfile player)
        {
            if (Session.Instance.IsServer)
            {
                var entity = new TopDownPlayer();
                entity.OwnerId = player.Id;
                entity.Position = SpawnPosition.Position;
                World.Spawn(entity);
            }
        }

        public override void RemovePlayer(PlayerProfile player)
        {
            if (Session.Instance.IsServer)
            {
                foreach (var entity in World)
                {
                    if (entity.OwnerId == player.Id)
                    {
                        World.Destroy(entity);
                    }
                }
            }
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
