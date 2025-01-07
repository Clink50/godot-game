using Godot;

[GlobalClass]
public partial class EnemyResource : Resource
{
    private float _speed;
    [Export]
    public float Speed
    {
        get => _speed;
        private set => _speed = value;
    }

    private float _maxHealth;
    [Export]
    public float MaxHealth
    {
        get => _maxHealth;
        private set => _maxHealth = value;
    }

    private float _damage;
    [Export]
    public float Damage
    {
        get => _damage;
        private set => _damage = value;
    }
}