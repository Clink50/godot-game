using Godot;

public partial class Bat : CharacterBody2D
{
    private EnemyStats _enemyStats;
    private CharacterBody2D _player;
    private Vector2 _velocity;

    public override void _Ready()
    {
        _player = GetTree().CurrentScene.GetNode<CharacterBody2D>("Player");
        _enemyStats = GetNode<EnemyStats>("EnemyStats");
    }

    public override void _PhysicsProcess(double delta)
    {
        Velocity = (_player.Position - Position).Normalized() * _enemyStats.currentSpeed;

        var collision = MoveAndCollide(Velocity * (float)delta);

        if (collision?.GetCollider() is Player player)
        {
            GD.Print("Enemy collided with player!");
            player.OnDamageTaken(_enemyStats.currentDamage);  // Apply damage to the player
        }
    }

    public void OnDamageTaken(float damage)
    {
        _enemyStats.currentHealth -= damage;

        if (_enemyStats.currentHealth <= 0)
        {
            EmitSignal(SignalName.TreeExited);
            QueueFree();
        }
    }
}