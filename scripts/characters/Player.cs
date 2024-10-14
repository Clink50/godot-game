using Godot;

public partial class Player : CharacterBody2D
{
	[Export] private float _speed = 150f;
	[Export] private float _speedMultiplier = 2;
	private Sprite2D _playerSprite;
	private AnimationPlayer _animationPlayer;
	private Timer _knifeAttackTimer;

	public Vector2 velocity;
	public Vector2 lastVelocity;

	public override void _Ready()
	{
		_playerSprite = GetNode<Sprite2D>("Sprite2D");
		_animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		lastVelocity = Vector2.Right;
		_knifeAttackTimer = new()
		{
			OneShot = false,
			WaitTime = 1f,
			Autostart = true,
		};
		AddChild(_knifeAttackTimer);
		_knifeAttackTimer.Timeout += OnAttackTimerTimeout;
	}

	public override void _PhysicsProcess(double delta)
	{
		// Get input for movement directions
		float horizontalDirection = Input.GetAxis("left", "right");
		float verticalDirection = Input.GetAxis("up", "down");

		// Calculate and normalize the velocity
		velocity = new Vector2(horizontalDirection, verticalDirection).Normalized();

		// Store the last non-zero velocity component for direction retention
		if (horizontalDirection != 0)
		{
			lastVelocity = new(velocity.X, 0);
		}

		if (verticalDirection != 0)
		{
			lastVelocity = new(0, velocity.Y);
		}

		if (horizontalDirection != 0 && verticalDirection != 0)
		{
			lastVelocity = new(velocity.X, velocity.Y);
		}

		// Check if the player is moving
		if (horizontalDirection != 0 || verticalDirection != 0)
		{
			// Update sprite direction and animation
			_playerSprite.FlipH = horizontalDirection < 0;
			_animationPlayer.Play("sideWalk");
		}
		else
		{
			// If no movement, stop the animation
			_animationPlayer.Stop();
		}

		// Apply speed to the movement
		velocity *= _speed;

		// Check if the player is running and apply speed multiplier
		if (Input.IsActionPressed("run"))
		{
			velocity *= _speedMultiplier;
		}

		// Set the player's velocity and move
		Velocity = velocity;
		MoveAndSlide();
	}

	public void OnAttackTimerTimeout()
	{
		var knifeInstance = GD.Load<PackedScene>("res://scenes/weapons/KnifeWeapon.tscn").Instantiate() as KnifeController;
		knifeInstance.Position = new Vector2(Position.X, Position.Y - 8);
		knifeInstance.ThrowInDirection(lastVelocity);
		GetParent().AddChild(knifeInstance);
	}
}
