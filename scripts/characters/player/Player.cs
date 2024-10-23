using Godot;

public partial class Player : CharacterBody2D
{
	[Export] private CharacterResource _characterResource;
	[Export] private float _speedMultiplier = 2;
	private Sprite2D _playerSprite;
	private AnimationPlayer _animationPlayer;
	private Timer _attackTimer;

	public Vector2 CurrentVelocity { get; set; }
	public Vector2 LastVelocity { get; set; } = Vector2.Right;

	private float _currentHealth;
	private float _currentRecovery;
	private float _currentSpeed;
	private float _currentMight;
	private float _currentProjectileSpeed;

	public override void _Ready()
	{
		_playerSprite = GetNode<Sprite2D>("Sprite2D");
		_animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");

		_currentHealth = _characterResource.MaxHealth;
		_currentRecovery = _characterResource.Recovery;
		_currentSpeed = _characterResource.Speed;
		_currentMight = _characterResource.Might;
		_currentProjectileSpeed = _characterResource.ProjectileSpeed;

		SetupWeaponAttack(_characterResource.StartingWeapon);
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
		CurrentVelocity *= _currentSpeed;

		// Check if the player is running and apply speed multiplier
		if (Input.IsActionPressed("run"))
		{
			CurrentVelocity *= _speedMultiplier;
		}

		// Set the player's CurrentVelocity and move
		Velocity = CurrentVelocity;
		MoveAndSlide();
	}

	private void SetupWeaponAttack(WeaponType weaponType)
    {
		_attackTimer = new()
		{
			OneShot = false,
			Autostart = true,
			WaitTime = weaponType == WeaponType.Knife ? 1f : 2f,
			Name = "AttackTimer"
		};
		AddChild(_attackTimer);
        _attackTimer.Timeout += () => OnAttackTimerTimeout(weaponType);
    }

	private void OnAttackTimerTimeout(WeaponType weaponType)
	{
		var weaponInstance = GD.Load<PackedScene>($"res://scenes/weapons/{weaponType}Weapon.tscn").Instantiate() as IBaseWeapon;

		if (weaponType == WeaponType.Garlic)
		{
			(weaponInstance as GarlicController).GarlicDespawned += StartAttackTimer;
		}

		weaponInstance.Activate(this);
	}

	public void StartAttackTimer()
	{
		_attackTimer.Start();
	}

	public void StopAttackTimer()
	{
		_attackTimer.Stop();
	}

	private void OnBodyEntered(Node2D body)
    {
        if (body is ICollectible collectible)
        {
            collectible.Collect();
        }
    }
}
