using Adventure.Entities;

namespace Adventure
{
    public static class EntityFactory
    {
        public static Entity CreateEntityFromType(EntityType type) 
        {
            Entity entity = null;

            switch (type) 
            {
                case EntityType.Player:
                    entity = new TopDownPlayer();
                    break;
                case EntityType.Tilemap:
                    entity = null;
                    break;
            }

            return entity;
        }
    }
}
