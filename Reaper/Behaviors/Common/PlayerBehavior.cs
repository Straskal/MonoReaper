using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Reaper.Engine;
using Reaper.Engine.Behaviors;
using Reaper.Engine.Singletons;
using Reaper.Objects.Common;
using Reaper.Singletons;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Reaper.Objects.Player
{
    public class PlayerBehavior : Behavior
    {
        private const int MAX_COSECUTIVE_ATTACKS = 3;

        private SpriteSheetBehavior _animationBehavior;
        private PlatformerBehavior _platformerBehavior;

        private Action<float> _currentState;

        private Vector2 _groundBeforeFall;
        private bool _wasMirroredBeforeFall;

        private int _currentAttackIndex;
        private bool _hasCheckedForHits;

        private SoundEffect _jumpSound;
        private SoundEffect _hitSound;
        private SoundEffect _swingSound;

        private Input.AxisAction _moveAction;
        private Input.PressedAction _jumpAction;
        private Input.PressedAction _attackAction;
        private Input.PressedAction _toggleFullscreenAction;
        private Input.PressedAction _exitGameAction;

        private WorldObject _poof;
        private SpriteSheetBehavior _poofAnimation;

        public PlayerBehavior(WorldObject owner) : base(owner) { }

        public override void Load(ContentManager contentManager)
        {
            _jumpSound = contentManager.Load<SoundEffect>("sounds/jump8");
            _hitSound = contentManager.Load<SoundEffect>("sounds/hi");
            _swingSound = contentManager.Load<SoundEffect>("sounds/swing");
        }

        public override void OnOwnerCreated()
        {
            _animationBehavior = Owner.GetBehavior<SpriteSheetBehavior>();
            _platformerBehavior = Owner.GetBehavior<PlatformerBehavior>();

            var input = Game.Singletons.Get<Input>();

            _moveAction = input.GetAction<Input.AxisAction>("move");
            _jumpAction = input.GetAction<Input.PressedAction>("jump");
            _attackAction = input.GetAction<Input.PressedAction>("attack");
            _toggleFullscreenAction = input.GetAction<Input.PressedAction>("toggleFullscreen");
            _exitGameAction = input.GetAction<Input.PressedAction>("exitGame");

            GoToIdle();
        }

        public override void OnLayoutStarted()
        {
            var poofDefinition = new WorldObjectDefinition();
            poofDefinition.SetSize(16, 16);
            poofDefinition.SetOrigin(new Point(8, 16));
            poofDefinition.MakeDecal();

            poofDefinition.AddBehavior(wo => new SpriteSheetBehavior(wo, new[] 
            {
                new SpriteSheetBehavior.Animation
                {
                    Name = "poof",
                    ImageFilePath = "art/player/poof",
                    SecPerFrame = 0.1f,
                    Loop = false,
                    Origin = new Vector2(8, 16),
                    Frames = new []
                    {
                        new SpriteSheetBehavior.Frame(0, 0, 16, 16),
                        new SpriteSheetBehavior.Frame(16, 0, 16, 16),
                        new SpriteSheetBehavior.Frame(32, 0, 16, 16),
                        new SpriteSheetBehavior.Frame(0, 0, 0, 0),
                    }
                },
            }));

            _poof = Layout.Spawn(poofDefinition, Vector2.Zero);
            _poofAnimation = _poof.GetBehavior<SpriteSheetBehavior>();
        }

        public override void Tick(GameTime gameTime)
        {
            if (_toggleFullscreenAction.WasPressed())
                Game.ToggleFullscreen();

            if (_exitGameAction.WasPressed())
                Game.Exit();

            if (Game.Singletons.Get<Input>().GetAction<Input.PressedAction>("dialogue").WasPressed())
                Game.Singletons.Get<Dialogue>().StartDialogue();

            _currentState.Invoke((float)gameTime.ElapsedGameTime.TotalSeconds);
        }

        public override void PostTick(GameTime gameTime)
        {
            UpdateFallRespawnPosition();
            HandleFallingOutsideLayout();
        }

        private void UpdateFallRespawnPosition()
        {
            var groundRay = new Rectangle(
                (int)Owner.Position.X - Owner.Origin.X,
                (int)Owner.Position.Y,
                16, 128);

            var ground = Layout.Grid.QueryBounds(groundRay)
                .Where(wo => wo != Owner && wo.IsSolid)
                .OrderBy(wo => Vector2.Distance(Owner.Position, wo.Position))
                .FirstOrDefault();

            if (ground != null)
            {
                _wasMirroredBeforeFall = Owner.IsMirrored;
                _groundBeforeFall.X = Owner.Position.X;
                _groundBeforeFall.Y = ground.Bounds.Top;
            }
        }

        private void HandleFallingOutsideLayout()
        {
            if (Owner.Position.Y > Layout.Height)
            {
                Owner.Position = _groundBeforeFall + new Vector2(_wasMirroredBeforeFall ? 32 : -32, 0);
                Owner.UpdateBBox();
            }
        }

        private void GoToIdle()
        {
            _animationBehavior.Play("Idle");
            _currentState = Idle;
        }

        private void Idle(float elapesedTime)
        {
            if (_moveAction.GetAxis() != 0f)
            {
                GoToMove();
            }
            else if (_jumpAction.WasPressed())
            {
                GoToJump();
            }
            else if (_platformerBehavior.IsFalling())
            {
                GoToFall();
            }
            else if (_attackAction.WasPressed())
            {
                GoToAttack();
            }
        }

        private void GoToMove()
        {
            _animationBehavior.Play("Run");
            _currentState = Move;
        }

        private void Move(float elapesedTime)
        {
            float movement = _moveAction.GetAxis();

            if (movement < 0f)
            {
                Owner.IsMirrored = true;
            }
            if (movement > 0f)
            {
                Owner.IsMirrored = false;
            }

            _platformerBehavior.Move(movement);

            if (movement == 0f)
            {
                GoToIdle();
            }
            if (_jumpAction.WasPressed())
            {
                GoToJump();
            }
            else if (_attackAction.WasPressed())
            {
                GoToAttack();
            }
        }

        private void GoToJump()
        {
            _jumpSound.Play(0.02f, -0.7f, 0f);
            _platformerBehavior.Jump();
            _animationBehavior.Play("Jump");
            _currentState = Jump;
            _poof.Position = Owner.Position + new Vector2(Owner.IsMirrored ? 14f : -14f, 0f);
            _poof.IsMirrored = Owner.IsMirrored;
            _poofAnimation.Color = Color.White * 0.8f;
             _poofAnimation.Play("poof");
        }

        private void Jump(float elapesedTime)
        {
            float movement = _moveAction.GetAxis();

            if (movement < 0f)
            {
                Owner.IsMirrored = true;
            }
            if (movement > 0f)
            {
                Owner.IsMirrored = false;
            }
            if (_jumpAction.IsDown())
            {
                _platformerBehavior.Jump();
            }

            _platformerBehavior.Move(movement);

            if (_platformerBehavior.IsFalling())
            {
                GoToFall();
            }
            else if (_attackAction.WasPressed())
            {
                GoToAttack();
            }
        }

        private void GoToFall()
        {
            _animationBehavior.Play("Fall");
            _currentState = Fall;
        }

        private void Fall(float elapesedTime)
        {
            float movement = _moveAction.GetAxis();

            if (movement < 0f)
            {
                Owner.IsMirrored = true;
            }
            if (movement > 0f)
            {
                Owner.IsMirrored = false;
            }

            _platformerBehavior.Move(movement);

            if (_platformerBehavior.IsOnGround())
            {
                _poof.Position = Owner.Position + new Vector2(Owner.IsMirrored ? 14f : -14f, 0f);
                _poof.IsMirrored = Owner.IsMirrored;
                _poofAnimation.Color = Color.White * 0.4f;
                _poofAnimation.Play("poof");

                if (_platformerBehavior.IsMoving())
                {
                    GoToMove();
                }
                else
                {
                    GoToIdle();
                }
            }
            else if (_attackAction.WasPressed())
            {
                GoToAttack();
            }
        }

        private void GoToAttack()
        {
            _hasCheckedForHits = false;
            _swingSound.Play();
            Freeze();
            _animationBehavior.PlayFromBeginning($"attack_{_currentAttackIndex}");
            _currentState = Attack;
        }

        private void Attack(float elapsedTime)
        {
            if (IsOnAttackFrame() && !_hasCheckedForHits)
            {
                var overlaps = QueryAttackBounds();

                if (overlaps.Any())
                    _hitSound.Play();

                foreach (var overlap in overlaps)
                {
                    if (IsAttackable(overlap, out var damageable))
                        continue;

                    damageable.Damage(new Damage { Amount = 1 });
                }

                _hasCheckedForHits = true;
            }
            else if (CanFollowupAttack() && _attackAction.WasPressed())
            {
                _currentAttackIndex++;

                GoToAttack();
            }
            else if (_animationBehavior.IsFinished)
            {
                Unfreeze();
                _currentAttackIndex = 0;

                GoToIdle();
            }
        }

        private bool IsOnAttackFrame() 
        {
            return _animationBehavior.CurrentFrame == 2;
        }

        private IEnumerable<WorldObject> QueryAttackBounds() 
        {
            var bounds = new Rectangle(
                Owner.IsMirrored ? Owner.Bounds.Left - 16 : Owner.Bounds.Right + 16,
                (int)Math.Round(Owner.Position.Y - 16),
                16, 16);

            return Owner.Layout.Grid.QueryBounds(bounds);
        }

        private bool IsAttackable(WorldObject worldObject, out DamageableBehavior damageable) 
        {
            return !worldObject.TryGetBehavior(out damageable) || worldObject == Owner;
        }

        private bool CanFollowupAttack() 
        {
            return _animationBehavior.CurrentFrame > 3 && _currentAttackIndex < (MAX_COSECUTIVE_ATTACKS - 1);
        }

        public float _prev;

        private void Freeze()
        {
            _platformerBehavior.Velocity = Vector2.Zero;

            if (_currentAttackIndex == 0) 
                _prev = _platformerBehavior.GravityAcceleration;

            _platformerBehavior.GravityAcceleration = 0f;
        }

        private void Unfreeze() 
        {
            _platformerBehavior.GravityAcceleration = _prev;
        }
    }
}
