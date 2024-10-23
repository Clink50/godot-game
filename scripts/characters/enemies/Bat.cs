using Godot;

public partial class Bat : CharacterBody2D
{
	private EnemyStats _enemyStats;
	private HitboxComponent _hitbox;
	private CharacterBody2D _player;
	private Vector2 _velocity;

	public override void _Ready()
	{
		_player = GetTree().CurrentScene.GetNode<CharacterBody2D>("Player");
		_enemyStats = GetNode<EnemyStats>("EnemyStats");

		_hitbox = GetNode<HitboxComponent>("HitboxComponent");
		_hitbox.DamageTaken += OnDamageTaken;
	}

	public override void _PhysicsProcess(double delta)
    {
		Velocity = (_player.Position - Position).Normalized() * _enemyStats.currentSpeed;
		MoveAndSlide();
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
