namespace ItsGood
{
    public class WorldObjectBuilder
    {
        private readonly WorldObject _worldObject;

        internal WorldObjectBuilder(WorldObject worldObject) 
        {
            _worldObject = worldObject;
        }

        public WorldObjectBuilder MakeSolid() 
        {
            _worldObject.IsSolid = true;
            return this;
        }

        public WorldObjectBuilder WithBehavior<T>() where T : Behavior, new()
        {
            _worldObject.Behaviors.Add(new T { Owner = _worldObject });
            return this;
        }

        public WorldObjectBuilder WithBehavior<T, U>(U state) where T : Behavior<U>, new()
        {
            _worldObject.Behaviors.Add(new T { Owner = _worldObject, State = state });
            return this;
        }
    }
}
