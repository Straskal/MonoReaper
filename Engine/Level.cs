using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Engine.Graphics;
using Engine.Collision;

namespace Engine
{
    /// <summary>
    /// This class represents a level in the game, which handles tracking entities and components.
    /// </summary>
    public class Level : Screen
    {
        private Coroutine _coroutine;

        public Level(App application, int cellSize, int width, int height) : base(application)
        {
            Content = new ContentManagerExtended(application.Services, application.Content.RootDirectory);
            Entities = new Entities(this);
            Width = width;
            Height = height;
            Camera = new Camera(application.BackBuffer);
            Partition = new Partition(cellSize);
        }

        internal ContentManagerExtended Content
        {
            get;
        }

        internal Entities Entities 
        {
            get;
        }

        public Camera Camera 
        {
            get;
        }

        public Partition Partition 
        {
            get;
        }

        public int Width 
        {
            get; 
        }

        public int Height 
        {
            get;
        }

        public LevelLoadStatus Status 
        { 
            get; 
            private set;
        }

        public void Spawn(Entity entity)
        {
            Entities.Spawn(entity, entity.Position);
        }

        public void Spawn(Entity entity, Vector2 position)
        {
            Entities.Spawn(entity, position);
        }

        public void Destroy(Entity entity)
        {
            Entities.Destroy(entity);
        }

        public override void Start()
        {
            if (Status == LevelLoadStatus.NotStarted)
            {
                IEnumerator LoadRoutine()
                {
                    Status = LevelLoadStatus.Loading;
                    yield return Entities.Start();
                    Status = LevelLoadStatus.Loaded;
                }

                _coroutine = Application.StartCoroutine(LoadRoutine());
            }
        }

        public override void Stop()
        {
            Application.StopCoroutine(_coroutine);
            Entities.Stop();
            Content.Unload();
        }

        public override void Update(GameTime gameTime)
        {
            if (Status == LevelLoadStatus.Loaded)
            {
                Entities.Update(gameTime);
            }
        }

        public override void Draw(Renderer renderer, GameTime gameTime)
        {
            if (Status == LevelLoadStatus.Loaded)
            {
                renderer.BeginDraw(Camera.TransformationMatrix);
                Entities.Draw(renderer, gameTime);
                renderer.EndDraw();
            }
        }
    }
}
