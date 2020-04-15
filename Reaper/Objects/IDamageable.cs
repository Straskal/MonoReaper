using Microsoft.Xna.Framework;

namespace Reaper
{
    public struct Damage 
    {
        public int Amount;
        public Vector2 Direction;
    }

    public interface IDamageable
    {
        void Damage(int amount);
    }
}
