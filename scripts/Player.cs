using Godot;

public partial class Player : CharacterBody2D
{
	[Export] private float _speed = 150.0f;
	[Export] private float _speedMultiplier = 1;
	private Sprite2D _playerSprite;
	private AnimationPlayer _animationPlayer;
	private string _previousDirection;

	[Signal]
	public delegate void PositionChangedEventHandler(Vector2 position);

	public override void _Ready()
	{
		_playerSprite = GetNode<Sprite2D>("Sprite2D");
		_animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
	}

	public override void _PhysicsProcess(double delta)
	{
		var horizontalDirection = Input.GetAxis("left", "right");
		var verticalDirection = Input.GetAxis("up", "down");

		_speedMultiplier = Input.IsActionPressed("run") ? 2 : 1;

		Vector2 velocity = new Vector2(horizontalDirection, verticalDirection).Normalized();

		if (horizontalDirection != 0 || verticalDirection != 0)
		{
			// Moving right
			_playerSprite.FlipH = horizontalDirection < 0;
			_animationPlayer.Play("sideWalk");

			var currentDirection = GetPlayerDirection(horizontalDirection, verticalDirection);
			if (_previousDirection != currentDirection)
			{
				_previousDirection = currentDirection;
				EmitSignal(SignalName.PositionChanged, Position);
			}
		}
		else
		{
			// Idle
			_animationPlayer.Stop();
		}

		Velocity = velocity * _speed * _speedMultiplier;
		MoveAndSlide();
	}

	private static string GetPlayerDirection(float horizontalDirection, float verticalDirection)
	{
		return true switch
		{
			var _ when horizontalDirection > 0 && verticalDirection == 0 => "right",
			var _ when horizontalDirection < 0 && verticalDirection == 0 => "left",
			var _ when horizontalDirection == 0 && verticalDirection > 0 => "down",
			var _ when horizontalDirection == 0 && verticalDirection < 0 => "up",
			var _ when horizontalDirection > 0 && verticalDirection < 0 => "topRight",
			var _ when horizontalDirection < 0 && verticalDirection < 0 => "topLeft",
			var _ when horizontalDirection > 0 && verticalDirection > 0 => "bottomRight",
			var _ when horizontalDirection < 0 && verticalDirection > 0 => "bottomLeft",
			_ => "idle",
		};
	}
}
