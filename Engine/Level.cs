using System.Collections;
using Microsoft.Xna.Framework;

namespace Engine
{
    public class Level : Screen
    {
        private readonly EntityManager entities;
        private Coroutine coroutine;

        public Level(App application, int cellSize, int width, int height) : base(application)
        {
            entities = new EntityManager(this);
            Content = new ContentManagerExtended(application.Services, application.Content.RootDirectory);
            Width = width;
            Height = height;
            Camera = new Camera(application.BackBuffer);
            Partition = new Partition(cellSize);
        }

        internal ContentManagerExtended Content { get; }

        public Camera Camera { get; }
        public Partition Partition { get; }
        public int Width { get; }
        public int Height { get; }
        public LevelLoadStatus Status { get; private set; }

        public void Spawn(Entity entity)
        {
            entities.Spawn(entity, entity.Position);
        }

        public void Spawn(Entity entity, Vector2 position)
        {
            entities.Spawn(entity, position);
        }

        public void Destroy(Entity entity)
        {
            entities.Destroy(entity);
        }

        public override void Start()
        {
            if (Status == LevelLoadStatus.NotStarted)
            {
                IEnumerator LoadRoutine()
                {
                    Status = LevelLoadStatus.Loading;
                    yield return entities.Start();
                    Status = LevelLoadStatus.Loaded;
                }

                coroutine = Application.StartCoroutine(LoadRoutine());
            }
        }

        public override void Stop()
        {
            Application.StopCoroutine(coroutine);
            entities.Stop();
            Content.Unload();
        }

        public override void Update(GameTime gameTime)
        {
            if (Status == LevelLoadStatus.Loaded)
            {
                entities.Update(gameTime);
            }
        }

        public override void Draw(Renderer renderer, GameTime gameTime)
        {
            if (Status == LevelLoadStatus.Loaded)
            {
                renderer.BeginDraw(Camera.TransformationMatrix);
                entities.Draw(renderer, gameTime);

                if (Application.IsDebugModeEnabled) 
                {
                    entities.DebugDraw(renderer, gameTime);
                    Partition.DebugDraw(renderer);
                }
       
                renderer.EndDraw();
            }
        }
    }
}
