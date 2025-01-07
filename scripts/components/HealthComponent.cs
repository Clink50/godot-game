using Godot;

public partial class HealthComponent : Node2D
{
    [Export]
    private float MAX_HEALTH = 100;
    public float health;

    public override void _Ready()
    {
        health = MAX_HEALTH;
    }

    public void Damage(float damage)
    {
        health -= damage;

        if (health <= 0)
        {
            GetParent().QueueFree();
        }
    }
}