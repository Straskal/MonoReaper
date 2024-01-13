namespace Adventure
{
    public static class Constants
    {
        public static class BoxLayers
        {
            public const int None = 1 << 0;
            public const int Player = 1 << 1;
            public const int Enemy = 1 << 2;
            public const int Damageable = 1 << 3;
            public const int Solid = 1 << 4;
            public const int Interactable = 1 << 5;
            public const int Damage = 1 << 6;
            public const int Projectile = 1 << 7;
            public const int Trigger = 1 << 8;
        }

        public static class EntityLayers
        {
            public const int Player = BoxLayers.Player | BoxLayers.Damageable;
            public const int PlayerProjectile = BoxLayers.Player | BoxLayers.Damage;
            public const int Projectile = BoxLayers.Damage;
            public const int Enemy = BoxLayers.Enemy | BoxLayers.Damageable;
            public const int Solid = BoxLayers.Solid;
            public const int Trigger = BoxLayers.Trigger;
        }
    }
}
