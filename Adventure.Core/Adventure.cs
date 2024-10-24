using Engine;
using Engine.Extensions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Adventure.Core
{
    public sealed class Adventure : Game
    {
        public const int ResolutionWidth = 256;
        public const int ResolutionHeight = 256;

        private readonly List<Actor> _actors = [];
        private readonly List<Actor> _actorsToUpdate = [];
        private readonly List<Actor> _actorsToDespawn = [];
        private readonly List<Actor> _actorsToCollide = [];

        public Adventure()
        {
            Instance = this;
            Window.Title = "Adventure Game 2000";
            Window.AllowUserResizing = true;
            IsMouseVisible = false;
            Content = new ContentManager(Services, "Content");
            GraphicsDeviceManager = new GraphicsDeviceManager(this);
            GraphicsDeviceManager.HardwareModeSwitch = false;
            GraphicsDeviceManager.IsFullScreen = false;
            GraphicsDeviceManager.PreferredBackBufferWidth = ResolutionWidth;
            GraphicsDeviceManager.PreferredBackBufferHeight = ResolutionHeight;
            IsFixedTimeStep = true;
        }

        public static Adventure Instance { get; private set; }
        public static GameTime Time { get; private set; }
        public static Player Player { get; private set; }
        public static bool IsPaused { get; set; }
        public GraphicsDeviceManager GraphicsDeviceManager { get; }
        public VirtualBackBuffer VirtualBackBuffer { get; private set; }
        public Renderer Renderer { get; private set; }
        public Camera Camera { get; private set; }
        public CoroutineRunner Coroutines { get; } = new();

        protected override void Initialize()
        {
            VirtualBackBuffer = new VirtualBackBuffer(GraphicsDevice, ResolutionWidth, ResolutionHeight);
            Window.ClientSizeChanged += (o, e) => VirtualBackBuffer.FitToNativeBackBuffer();
            Renderer = new Renderer(GraphicsDevice);
            Camera = new Camera(ResolutionWidth, ResolutionHeight);

            base.Initialize();

            var player = new Player();
            player.Position = new Vector2(100f, 100f);
            Spawn(player);

            var barrel = new Barrel();
            barrel.Position = new Vector2(10f, 10f);
            Spawn(barrel);

            var barrel2 = new Barrel();
            barrel2.Position = new Vector2(40f, 10f);
            Spawn(barrel2);
        }

        protected override void LoadContent()
        {
            ContentCache.Fonts.Default = Content.Load<SpriteFont>("fonts/font");
            ContentCache.Gfx.Cursor = Content.Load<Texture2D>("art/cursor");
            ContentCache.Gfx.Player = Content.Load<Texture2D>("art/player/player");
            ContentCache.Gfx.Fire = Content.Load<Texture2D>("art/player/fire");
            ContentCache.Gfx.Barrel = Content.Load<Texture2D>("art/common/barrel");
            ContentCache.Gfx.Explosion = Content.Load<Texture2D>("art/common/explosion-1");
            ContentCache.Gfx.LargeDoor = Content.Load<Texture2D>("art/large_door");
            ContentCache.Gfx.PressurePlate = Content.Load<Texture2D>("art/pressure_plate");
            ContentCache.Vfx.SolidColor = Content.Load<Effect>("shaders/SolidColor");
            ContentCache.Sfx.Shoot = Content.Load<SoundEffect>("audio/fireball_shoot");
            ContentCache.Sfx.Explosion = Content.Load<SoundEffect>("audio/explosion4");

            base.LoadContent();
        }

        protected override void UnloadContent()
        {
            VirtualBackBuffer.Dispose();
            Renderer.Dispose();
            Content.Unload();

            base.UnloadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            var deltaTime = gameTime.GetDeltaTime();

            Time = gameTime;
            Input.Update(VirtualBackBuffer);

            if (!IsPaused)
            {
                Coroutines.Update();

                foreach (var actor in _actorsToUpdate)
                {
                    actor.Update(deltaTime);
                }

                if (_actorsToDespawn.Count > 0) 
                {
                    foreach (var actor in _actorsToDespawn) 
                    {
                        _actors.Remove(actor);
                        _actorsToUpdate.Remove(actor);
                    }

                    _actorsToDespawn.Clear();
                }
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            var deltaTime = gameTime.GetDeltaTime();

            // Set render target to virtual backbuffer and use virtual viewport.
            Renderer.SetTarget(VirtualBackBuffer);
            Renderer.SetViewport(VirtualBackBuffer.Viewport);
            Renderer.Clear(Color.Black);

            Renderer.BeginDraw(Camera.TransformationMatrix);

            foreach (var actor in _actors) 
            {
                actor.Draw(deltaTime);
            }

            foreach (var actor in _actors)
            {
                actor.DebugDraw(deltaTime);
            }

            Renderer.EndDraw();

            // Set target to screen and set viewport to letterboxed viewport.
            Renderer.SetTarget(null);
            Renderer.SetViewport(VirtualBackBuffer.NativeViewport);
            Renderer.Clear(Color.Black);
            Renderer.BeginDraw(VirtualBackBuffer.ScaleMatrix);

            // Draw the virtual backbuffer to the screen.
            Renderer.Draw(VirtualBackBuffer, Vector2.Zero);
            Renderer.EndDraw();
        }

        #region Actor

        public void Spawn(Actor actor)
        {
            if (actor.Game != null)
            {
                // TODO: Log error
                return;
            }

            actor.Game = this;
            _actors.Add(actor);
            actor.Spawn();
        }

        public void Despawn(Actor actor)
        {
            if (_actorsToDespawn.Contains(actor))
            {
                // TODO: Log error
                return;
            }

            _actorsToDespawn.Add(actor);
        }

        public void EnableUpdate(Actor actor) 
        {
            if (!_actorsToUpdate.Contains(actor)) 
            {
                _actorsToUpdate.Add(actor);
            }
        }

        public void DisableUpdate(Actor actor) 
        {
            _actorsToUpdate.Remove(actor);
        }

        #endregion

        #region Collision

        public void EnableCollision(Actor actor)
        {
            _actorsToCollide.Add(actor);
        }

        public void DisableCollision(Actor actor)
        {
            _actorsToCollide.Remove(actor);
        }

        public List<Actor> Overlap(Vector2 point)
        {
            var result = new List<Actor>();

            foreach (var actor in _actorsToCollide)
            {
                if (actor.Shape.Overlaps(point))
                {
                    result.Add(actor);
                }
            }

            return result;
        }

        public List<Actor> Overlap(Vector2 point, CollisionLayers layerMask)
        {
            var result = new List<Actor>();

            foreach (var actor in _actorsToCollide)
            {
                if (actor.CheckLayer(layerMask) && actor.Shape.Overlaps(point))
                {
                    result.Add(actor);
                }
            }

            return result;
        }

        public List<Actor> Overlap(Vector2 point, CollisionLayers layerMask, Actor ignore)
        {
            var result = new List<Actor>();

            foreach (var actor in _actorsToCollide)
            {
                if (actor.CheckLayer(layerMask) && actor != ignore && actor.Shape.Overlaps(point))
                {
                    result.Add(actor);
                }
            }

            return result;
        }

        public List<Actor> Overlap(CircleF circle, CollisionLayers layerMask)
        {
            var result = new List<Actor>();

            foreach (var actor in _actorsToCollide)
            {
                if (actor.CheckLayer(layerMask) && actor.Shape.Overlaps(circle))
                {
                    result.Add(actor);
                }
            }

            return result;
        }

        public List<Actor> Overlap(CircleF circle, CollisionLayers layerMask, Actor ignore)
        {
            var result = new List<Actor>();

            foreach (var actor in _actorsToCollide)
            {
                if (actor.CheckLayer(layerMask) && actor != ignore && actor.Shape.Overlaps(circle))
                {
                    result.Add(actor);
                }
            }

            return result;
        }

        public List<Actor> Overlap(RectangleF rectangle, CollisionLayers layerMask)
        {
            var result = new List<Actor>();

            foreach (var actor in _actorsToCollide)
            {
                if (actor.CheckLayer(layerMask) && actor.Shape.Overlaps(rectangle))
                {
                    result.Add(actor);
                }
            }

            return result;
        }

        public List<Actor> Overlap(RectangleF rectangle, CollisionLayers layerMask, Actor ignore)
        {
            var result = new List<Actor>();

            foreach (var actor in _actorsToCollide)
            {
                if (actor.CheckLayer(layerMask) && actor != ignore && actor.Shape.Overlaps(rectangle))
                {
                    result.Add(actor);
                }
            }

            return result;
        }

        public List<Actor> Overlap(Actor actor, CollisionLayers layerMask)
        {
            var result = new List<Actor>();

            foreach (var other in _actorsToCollide)
            {
                if (other.CheckLayer(layerMask) && other.Shape.Overlaps(actor.Shape))
                {
                    result.Add(actor);
                }
            }

            return result;
        }

        public List<T> Overlap<T>(CircleF circle, CollisionLayers layerMask, Actor ignore) where T : Actor
        {
            var result = new List<T>();

            foreach (var actor in _actorsToCollide)
            {
                if (actor.CheckLayer(layerMask) && actor != ignore && actor is T t && actor.Shape.Overlaps(circle))
                {
                    result.Add(t);
                }
            }

            return result;
        }

        public List<T> Overlap<T>(RectangleF rectangle, CollisionLayers layerMask, Actor ignore) where T : Actor
        {
            var result = new List<T>();

            foreach (var actor in _actorsToCollide)
            {
                if (actor.CheckLayer(layerMask) && actor != ignore && actor is T t && actor.Shape.Overlaps(rectangle))
                {
                    result.Add(t);
                }
            }

            return result;
        }

        public List<T> Overlap<T>(Actor actor, CollisionLayers layerMask) where T : Actor
        {
            var result = new List<T>();

            foreach (var other in _actorsToCollide)
            {
                if (other.CheckLayer(layerMask) && other is T t && other.Shape.Overlaps(actor.Shape))
                {
                    result.Add(t);
                }
            }

            return result;
        }

        public Actor Raycast(Actor actor, Vector2 direction, CollisionLayers layerMask, out Collision collision)
        {
            collision = Collision.Empty;
            Actor result = null;
            var path = new Segment(actor.Position, direction);

            foreach (var other in _actorsToCollide)
            {
                if (other != actor && other.CheckLayer(layerMask) && actor.Shape.Intersects(other.Shape, path, out var intersection) && intersection.Time < collision.Intersection.Time)
                {
                    result = other;
                    collision = new Collision(direction, intersection);
                }
            }

            return result;
        }

        public Actor Raycast(Vector2 position, Vector2 direction, CollisionLayers layerMask, Actor ignore)
        {
            var collision = Collision.Empty;
            Actor result = null;

            var segment = new Segment(position, direction);
            var broadphaseRectangle = new RectangleF(
                MathF.Min(position.X, position.X + direction.X),
                MathF.Min(position.Y, position.Y + direction.Y),
                MathF.Max(position.X, position.X + direction.X),
                MathF.Max(position.Y, position.Y + direction.Y));

            foreach (var collider in Overlap(broadphaseRectangle, layerMask, ignore))
            {
                if (collider.Shape.Intersects(segment, out var intersection) && intersection.Time < collision.Intersection.Time)
                {
                    result = collider;
                    collision = new Collision(direction, intersection);
                }
            }

            return result;
        }

        #endregion Collision
    }
}
