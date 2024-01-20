namespace Adventure.Entities
{
    public interface IDamageable
    {
        bool Flammable { get; }

        void Damage(int amount);
    }
}
