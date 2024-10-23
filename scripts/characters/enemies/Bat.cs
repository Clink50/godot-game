using Godot;

public partial class Bat : CharacterBody2D
{
	[Export] private EnemyResource _enemyResource;
	private HitboxComponent _hitbox;
	private CharacterBody2D _player;
	private Vector2 _velocity;
	private float _currentSpeed;
	private float _currentHealth;
	private float _currentDamage;

	public override void _Ready()
	{
		_player = GetTree().CurrentScene.GetNode<CharacterBody2D>("Player");

		_currentSpeed = _enemyResource.Speed;
		_currentHealth = _enemyResource.MaxHealth;
		_currentDamage = _enemyResource.Damage;

		_hitbox = GetNode<HitboxComponent>("HitboxComponent");
		_hitbox.DamageTaken += OnDamageTaken;
	}

	public override void _PhysicsProcess(double delta)
    {
		Velocity = (_player.Position - Position).Normalized() * _enemyResource.Speed;
		MoveAndSlide();
    }

	public void OnDamageTaken(float damage)
	{
		_currentHealth -= damage;

		if (_currentHealth <= 0)
		{
			EmitSignal(SignalName.TreeExited);
			QueueFree();
		}
	}
}
