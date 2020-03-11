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
            _worldObject.AddBehavior<T>();
            return this;
        }

        public WorldObjectBuilder WithBehavior<T, U>(U state) where T : Behavior<U>, new()
        {
            _worldObject.AddBehavior<T, U>(state);
            return this;
        }

        public WorldObjectBuilder WithEffect(string name, bool isEnabled) 
        {
            _worldObject.EffectFilePath = name;
            _worldObject.IsEffectEnabled = isEnabled;
            return this;
        }
    }
}
