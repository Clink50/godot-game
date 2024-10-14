using Godot;

public partial class Player : CharacterBody2D
{
	[Export] private float _speed = 150f;
	[Export] private float _speedMultiplier = 2;
	private Sprite2D _playerSprite;
	private AnimationPlayer _animationPlayer;

	public Vector2 CurrentVelocity { get; set; }
	public Vector2 LastVelocity { get; set; } = Vector2.Right;

	public override void _Ready()
	{
		_playerSprite = GetNode<Sprite2D>("Sprite2D");
		_animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		SetupWeaponAttack("Knife");
	}

	public override void _PhysicsProcess(double delta)
	{
		// Get input for movement directions
		float horizontalDirection = Input.GetAxis("left", "right");
		float verticalDirection = Input.GetAxis("up", "down");

		// Calculate and normalize the velocity
		CurrentVelocity = new Vector2(horizontalDirection, verticalDirection).Normalized();

		// Store the last non-zero velocity component for direction retention
		if (horizontalDirection != 0)
		{
			LastVelocity = new(CurrentVelocity.X, 0);
		}

		if (verticalDirection != 0)
		{
			LastVelocity = new(0, CurrentVelocity.Y);
		}

		if (horizontalDirection != 0 && verticalDirection != 0)
		{
			LastVelocity = new(CurrentVelocity.X, CurrentVelocity.Y);
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
		CurrentVelocity *= _speed;

		// Check if the player is running and apply speed multiplier
		if (Input.IsActionPressed("run"))
		{
			CurrentVelocity *= _speedMultiplier;
		}

		// Set the player's CurrentVelocity and move
		Velocity = CurrentVelocity;
		MoveAndSlide();
	}

	private void SetupWeaponAttack(string weaponType)
	{
		Timer attackTimer = new()
		{
			OneShot = false,
			Autostart = true,
		};

		if (weaponType == "Knife")
		{
			attackTimer.WaitTime = 1f;
		}
		else if (weaponType == "Garlic")
		{
			attackTimer.WaitTime = 3f;
		}

		AddChild(attackTimer);
		attackTimer.Timeout += () => OnAttackTimerTimeout(weaponType);
	}

	private void OnAttackTimerTimeout(string weaponType)
	{
		var weaponInstance = GD.Load<PackedScene>($"res://scenes/weapons/{weaponType}Weapon.tscn").Instantiate() as IBaseWeapon;
		weaponInstance.Activate(this);
	}
}
