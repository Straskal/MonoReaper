﻿using Microsoft.Xna.Framework;
using Reaper.Engine;
using Reaper.Objects.Common;
using Reaper.Singletons;
using System;

namespace Reaper.Behaviors.Common
{
    public class PlayerBehavior : Behavior
    {
        private SpriteSheetBehavior _spriteSheetBehavior;
        private InputManager.AxisAction _horizontalAction;
        private InputManager.AxisAction _verticalAction;
        private InputManager.AxisAction _attackHorizontalAction;
        private InputManager.AxisAction _attackVerticalAction;

        private float _attackTimer = 0.5f;
        private float _attackTime;

        public PlayerBehavior(WorldObject owner) : base(owner) { }

        public Vector2 Direction { get; private set; } = Reaper.Direction.Down;
        public float Speed { get; set; } = 100f;

        public override void OnOwnerCreated()
        {
            _spriteSheetBehavior = Owner.GetBehavior<SpriteSheetBehavior>();

            var input = Game.Singletons.Get<InputManager>();
            _horizontalAction = input.GetAction<InputManager.AxisAction>("horizontal");
            _verticalAction = input.GetAction<InputManager.AxisAction>("vertical");
            _attackHorizontalAction = input.GetAction<InputManager.AxisAction>("attackHorizontal");
            _attackVerticalAction = input.GetAction<InputManager.AxisAction>("attackVertical");
        }

        public override void Tick(GameTime gameTime)
        {
            float elapsedTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            Vector2 movement = new Vector2(_horizontalAction.GetAxis(), _verticalAction.GetAxis());

            if (movement != Vector2.Zero)
            {
                Direction = movement;

                if (movement.LengthSquared() > 0f)
                    movement.Normalize();

                movement *= Speed * elapsedTime;

                if (Layout.Grid.IsCollidingAtOffsetOk(Owner, movement.X, 0f, out var overlapX))
                {
                    movement.X += overlapX.Depth.X;
                }

                if (Layout.Grid.IsCollidingAtOffsetOk(Owner, 0f, movement.Y, out var overlapY))
                {
                    movement.Y += overlapY.Depth.Y;
                }

                Owner.Move(movement.X, movement.Y);
            }

            _spriteSheetBehavior.Play(movement == Vector2.Zero ? "idle" : "walk");

            var attackDirection = new Vector2(_attackHorizontalAction.GetAxis(), _attackVerticalAction.GetAxis());

            if (attackDirection != Vector2.Zero)
            {
                if (gameTime.TotalGameTime.TotalSeconds > _attackTime)
                {
                    var proj = Layout.Spawn(Projectile.Definition(), Owner.Position);
                    proj.GetBehavior<ProjectileBehavior>().Direction = attackDirection;
                    proj.GetBehavior<ProjectileBehavior>().Speed = 200f;

                    _attackTime = (float)gameTime.TotalGameTime.TotalSeconds + _attackTimer;
                }
            }
        }
    }
}
