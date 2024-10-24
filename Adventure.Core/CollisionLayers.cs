namespace Adventure.Core
{
    public enum CollisionLayers : uint
    {
        None = 1 << 0,
        Player = 1 << 1,
        Enemy = 1 << 2,
        Damageable = 1 << 3,
        Solid = 1 << 4,
        Interactable = 1 << 5,
        Damage = 1 << 6,
        Projectile = 1 << 7,
        Trigger = 1 << 8
    }
}
