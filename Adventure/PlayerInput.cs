using Microsoft.Xna.Framework;

namespace Adventure
{
    public readonly struct PlayerInput
    {
        public PlayerInput(Vector2 move, Vector2 aim, bool shoot)
        {
            Move = move;
            Aim = aim;
            Shoot = shoot;
        }

        public readonly Vector2 Move;
        public readonly Vector2 Aim;
        public readonly bool Shoot;
    }
}
