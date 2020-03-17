using Microsoft.Xna.Framework;
using Reaper.Engine;
using Reaper.Engine.Behaviors;
using System;
using System.Linq;

namespace Reaper.Objects
{
    public class ThugBehavior : Behavior
    {
        private PlatformerBehavior _platformer;
        private SpriteSheetBehavior _spriteSheet;
        private Action<GameTime> _currentState;
        private float _movement;

        private PlayerBehavior _player;

        public ThugBehavior(WorldObject owner) : base(owner)
        {
        }

        public override void OnOwnerCreated()
        {
            _platformer = Owner.GetBehavior<PlatformerBehavior>();
            _spriteSheet = Owner.GetBehavior<SpriteSheetBehavior>();
            _currentState = GoToPatrol;

            _player = Owner.Layout.GetWorldObjectOfType<PlayerBehavior>();
        }

        public override void Tick(GameTime gameTime)
        {
            _currentState.Invoke(gameTime);
        }

        private void GoToPatrol(GameTime gameTime)
        {
            _spriteSheet.Play("Walk");
            _movement = Owner.IsMirrored ? -1f : 1f;
            _currentState = Patrol;
            _platformer.MaxSpeed = 50f;
        }

        private void Patrol(GameTime gameTime)
        {
            _platformer.Move(_movement);

            if (IsGoingToFall() || IsRunningIntoWall())
            {
                _movement *= -1f;
                Owner.IsMirrored = !Owner.IsMirrored;
            }

            if (HasPlayerInSight()) 
            {
                GoToChase(gameTime);
            }
        }

        private void GoToChase(GameTime gameTime)
        {
            _spriteSheet.Play("Walk");
            _platformer.MaxSpeed = 70f;
            _currentState = Chase;

        }

        private void Chase(GameTime gameTime)
        {
            _platformer.Move(_movement);

            if (!HasPlayerInSight())
            {
                GoToPatrol(gameTime);
            }
        }

        private bool IsGoingToFall()
        {
            return !Owner.Layout.TestOverlapSolidOffset(Owner, Owner.IsMirrored ? -Owner.Bounds.Width : Owner.Bounds.Width, 1, out var worldObject)
                || worldObject.Bounds.Top > Owner.Bounds.Bottom;
        }

        private bool IsRunningIntoWall()
        {
            float reach = Owner.Bounds.Width * 0.5f + 1;

            return Owner.Layout.TestOverlapSolidOffset(Owner, Owner.IsMirrored ? -reach : reach, 0);
        }

        private bool HasPlayerInSight()
        {
            int reach = 256;
            int x = Owner.IsMirrored ? Owner.Bounds.Left - reach : Owner.Bounds.Right + 1;
            Rectangle lineOfSight = new Rectangle(x, (int)Math.Round(Owner.Position.Y - Owner.Bounds.Height * 0.5f), reach, 16);

            return lineOfSight.Intersects(_player.Owner.Bounds);
        }
    }
}
