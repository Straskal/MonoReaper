using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;

namespace ItsGood
{
    public class Layout
    {
        private readonly List<WorldObject> _worldObjects;
        private readonly List<WorldObject> _toSpawn;
        private readonly List<WorldObject> _toDestroy;

        private IEnumerable<Behavior> _allBehaviors;

        public Layout(MainGame game) 
        {
            Game = game;
            View = new LayoutView(game);

            _worldObjects = new List<WorldObject>();
            _toSpawn = new List<WorldObject>();
            _toDestroy = new List<WorldObject>();
            _allBehaviors = new List<Behavior>();
        }

        public MainGame Game { get; }
        public LayoutView View { get; }

        public WorldObjectBuilder CreateObject(string imageFilePath, Rectangle source, Vector2 position)
        {
            var worldObject = new WorldObject(this)
            {
                ImageFilePath = imageFilePath,
                Position = position,
                Source = source,
                Color = Color.White
            };

            _toSpawn.Add(worldObject);

            return new WorldObjectBuilder(worldObject);
        }

        public void DestroyObject(WorldObject worldObject) 
        {
            if (!worldObject.MarkedForDestroy) 
            {
                worldObject.MarkForDestroy();

                _toDestroy.Add(worldObject);
            }
        }

        internal void Tick(GameTime gameTime)
        {
            InvokeBehaviorCallbacks();
            SyncWorldObjectLists();

            foreach (var behavior in _allBehaviors) 
            {
                behavior.Tick(gameTime);
            }
        }

        internal void Draw(SpriteBatch batch)
        {
            View.Sync();

            batch.Begin(
                SpriteSortMode.Deferred,
                BlendState.AlphaBlend,
                SamplerState.PointClamp,
                DepthStencilState.None,
                RasterizerState.CullNone,
                null, View.TransformationMatrix
            );

            foreach (var worldObject in _worldObjects)
            {
                batch.Draw(
                    worldObject.Image, 
                    worldObject.Position, 
                    worldObject.Source, 
                    worldObject.Color, 
                    0, 
                    new Vector2(worldObject.Source.Width * 0.5f, worldObject.Source.Height * 0.5f), 
                    Vector2.One, 
                    worldObject.IsMirrored ? SpriteEffects.FlipHorizontally: SpriteEffects.None, 
                    0
                );
            }

            batch.End();
        }

        private void InvokeBehaviorCallbacks() 
        {
            foreach (var toSpawn in _toSpawn) 
            {
                foreach (var behavior in toSpawn.Behaviors) 
                {
                    behavior.Initialize();
                }
            }

            foreach (var toDestroy in _toDestroy)
            {
                foreach (var behavior in toDestroy.Behaviors)
                {
                    behavior.OnOwnerDestroyed();
                }
            }
        }

        private void SyncWorldObjectLists()
        {
            if (_toSpawn.Count == 0 && _toDestroy.Count == 0)
                return;

            foreach (var toSpawn in _toSpawn)
            {
                toSpawn.Image = Game.Content.Load<Texture2D>(toSpawn.ImageFilePath);

                _worldObjects.Add(toSpawn);

                foreach (var behavior in toSpawn.Behaviors)
                {
                    behavior.OnOwnerCreated();
                }
            }

            _toSpawn.Clear();

            foreach (var toDestroy in _toDestroy) 
            {
                _worldObjects.Remove(toDestroy);
            }

            _toDestroy.Clear();

            _allBehaviors = _worldObjects.SelectMany(worldObject => worldObject.Behaviors);
        }
    }
}
