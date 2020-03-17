﻿using Microsoft.Xna.Framework;
using Reaper.Engine;
using Reaper.Engine.Behaviors;
using System;

namespace Reaper.Objects
{
    public class ThugBehavior : Behavior
    {
        private PlatformerBehavior _platformer;
        private SpriteSheetBehavior _spriteSheet;
        private Action<GameTime> _currentState;
        private DamageableBehavior _damageable;
        private float _movement;

        private PlayerBehavior _player;

        public ThugBehavior(WorldObject owner) : base(owner)
        {
        }

        public override void OnOwnerCreated()
        {
            _platformer = Owner.GetBehavior<PlatformerBehavior>();
            _spriteSheet = Owner.GetBehavior<SpriteSheetBehavior>();
            _damageable = Owner.GetBehavior<DamageableBehavior>();
            _player = Owner.Layout.GetWorldObjectOfType<PlayerBehavior>();
            _damageable.OnDamaged += OnDamaged;

            GoToPatrol();
        }

        public override void OnOwnerDestroyed()
        {
            _damageable.OnDamaged -= OnDamaged;
        }

        public override void Tick(GameTime gameTime)
        {
            _currentState?.Invoke(gameTime);
        }

        private void GoToPatrol()
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
            _platformer.MaxSpeed = 100f;
            _currentState = Chase;
        }

        private void Chase(GameTime gameTime)
        {
            _platformer.Move(_movement);

            if (HasPlayerInSight()) 
            {
                if (MathHelper.Distance(Owner.Position.X, _player.Owner.Position.X) < 32) 
                {
                    GoToAttack();
                }
            }
            else 
            {
                GoToPatrol();
            }
        }

        private void GoToAttack()
        {
            _spriteSheet.Play("stab");
            _currentState = Attack;
        }

        private void Attack(GameTime gameTime)
        {
            if (_spriteSheet.IsFinished) 
            {
                if (HasPlayerInSight())
                {
                    GoToChase(gameTime);
                }
                else
                {
                    _movement *= -1f;
                    Owner.IsMirrored = !Owner.IsMirrored;

                    GoToPatrol();
                }
            }
        }

        private void GoToStunned()
        {
            _spriteSheet.Play("Walk");
            _platformer.Freeze();
            _currentState = null;

            Owner.GetBehavior<TimerBehavior>().StartTimer(0.2f, () => 
            {
                _platformer.Unfreeze();

                if (!HasPlayerInSight())
                {
                    _movement *= -1f;
                    Owner.IsMirrored = !Owner.IsMirrored;
                }

                GoToAttack();
            });
        }

        private DamageResponse OnDamaged(Damage amount) 
        {
            GoToStunned();

            return new DamageResponse();
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
