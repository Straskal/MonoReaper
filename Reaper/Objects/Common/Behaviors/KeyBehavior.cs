using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Reaper.Engine;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Reaper
{
    public class KeyBehavior : Behavior
    {
        private WorldObject _player;
        private List<WorldObject> _enemies;
        private Action<GameTime> _currentAction;
        private Vector2 _targetPosition;
        private SoundEffect _pickupSound;

        public KeyBehavior(WorldObject owner) : base(owner) { }

        public string Door { get; set; }
        public float Speed { get; set; } = 50f;

        public override void Load(ContentManager contentManager)
        {
            _pickupSound = contentManager.Load<SoundEffect>("audio/pickup_key");
        }

        public override void OnLayoutStarted()
        {
            _player = Layout.Objects.FindFirstWithTag("player");
            _enemies = Layout.Objects.FindWithTag("enemy").ToList();
            _targetPosition = Owner.Position;

            Owner.SpatialType = SpatialType.Pass;
            Owner.SetY(-Owner.Height * 2);

            _currentAction = CheckEnemyCount;
        }

        public override void Tick(GameTime gameTime)
        {
            _currentAction?.Invoke(gameTime);
        }

        private void CheckEnemyCount(GameTime gameTime) 
        {
            _enemies.RemoveAll(wo => wo.Destroyed);

            if (!_enemies.Any())
            {
                _currentAction = Fall;
            }
        }

        private void Fall(GameTime gameTime)
        {
            Owner.Position += new Vector2(0f, 1f) * Speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (Owner.Position.Y >= _targetPosition.Y) 
            {
                _currentAction = CheckOverlap;
            }
        }

        private void CheckOverlap(GameTime gameTime)
        {
            if (Owner.Bounds.Intersects(_player.Bounds))
            {
                foreach (var door in Layout.Objects.FindWithTag("door")) 
                {
                    door.Destroy();
                }
                _pickupSound.Play();
                Owner.Destroy();
            }
        }
    }
}
