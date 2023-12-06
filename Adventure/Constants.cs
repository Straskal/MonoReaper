namespace Adventure
{
    public static class Constants
    {
        public static class Layers
        {
            public const string Entities = "worldobjects";
            public const string Solids = "solids";
            public const string Background = "background";
        }

        public static class AssetPaths
        {
            public const string Layouts = "layouts/";
            public const string Tilesets = "art/tilesets/";
        }

        // Need to re-evaluate the layer support. I feel like this will get out of hand. Maybe not. Dunno.
        public static class BoxLayers
        {
            public const int None = 1 << 0;
            public const int Player = 1 << 1;
            public const int Enemy = 1 << 2;
            public const int Damageable = 1 << 3;
            public const int Wall = 1 << 4;
        }

        public static class EntityLayers
        {
            public const int Player = BoxLayers.Player | BoxLayers.Damageable;
            public const int PlayerProjectile = BoxLayers.Player;
            public const int Enemy = BoxLayers.Enemy | BoxLayers.Damageable;
            public const int Solid = BoxLayers.Wall;
        }
    }
}
