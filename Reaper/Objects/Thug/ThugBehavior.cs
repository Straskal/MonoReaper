using Microsoft.Xna.Framework;
using Reaper.Engine;
using Reaper.Engine.Behaviors;

namespace Reaper.Objects
{
    public class ThugBehavior : Behavior
    {
        private PlatformerBehavior _platformer;
        private SpriteSheetBehavior _spriteSheet;
        private float _movement;

        public ThugBehavior(WorldObject owner) : base(owner)
        {
        }

        public override void OnOwnerCreated()
        {
            _platformer = Owner.GetBehavior<PlatformerBehavior>();
            _spriteSheet = Owner.GetBehavior<SpriteSheetBehavior>();

            _spriteSheet.Play("Walk");

            _movement = Owner.IsMirrored ? -1f : 1f;
        }

        public override void Tick(GameTime gameTime)
        {
            _platformer.Move(_movement);

            if (IsGoingToFall() || IsRunningIntoWall()) 
            {
                _movement *= -1f;
                Owner.IsMirrored = !Owner.IsMirrored;
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
    }
}
