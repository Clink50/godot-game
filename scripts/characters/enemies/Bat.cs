using Godot;

public partial class Bat : CharacterBody2D
{
	[Export] private EnemyResource _enemyResource;
	private Player _player;
	private Vector2 _velocity;
	private float _currentSpeed;

	public override void _Ready()
	{
		_player = GetParent().GetNode<Player>("Player");
		_currentSpeed = _enemyResource.Speed;
	}

	public override void _PhysicsProcess(double delta)
    {
		Velocity = (_player.Position - Position).Normalized() * _enemyResource.Speed;
		MoveAndSlide();
    }
}
