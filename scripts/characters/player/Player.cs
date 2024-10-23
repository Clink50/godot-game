using Godot;

public partial class Player : CharacterBody2D
{
	[Export] private float _speedMultiplier = 2;
	private PlayerStats _playerStats;
	private Sprite2D _playerSprite;
	private AnimationPlayer _animationPlayer;
	private Timer _attackTimer;
	private Area2D _itemDetectionArea;

	public Vector2 LastVelocity { get; private set; } = Vector2.Right;

	public override void _Ready()
	{
		_playerSprite = GetNode<Sprite2D>("Sprite2D");
		_animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		_playerStats = GetNode<PlayerStats>("PlayerStats");

		_itemDetectionArea = GetNode<Area2D>("ItemCollector");
    	_itemDetectionArea.AreaEntered += OnAreaEntered;

		SetupWeaponAttack(_playerStats.currentWeapon);
	}

	public override void _PhysicsProcess(double delta)
	{
		// Get input for movement directions
		float horizontalDirection = Input.GetAxis("left", "right");
		float verticalDirection = Input.GetAxis("up", "down");

		// Calculate and normalize the velocity
		var currentVelocity = new Vector2(horizontalDirection, verticalDirection);

		if (currentVelocity.Length() > 1)
		{
			currentVelocity = currentVelocity.Normalized();
		}

		// Apply speed to the movement
		currentVelocity *= _playerStats.currentSpeed;

		// Store the last non-zero velocity component for direction retention
		if (horizontalDirection != 0)
		{
			LastVelocity = new(currentVelocity.X, 0);
		}

		if (verticalDirection != 0)
		{
			LastVelocity = new(0, currentVelocity.Y);
		}

		if (horizontalDirection != 0 && verticalDirection != 0)
		{
			LastVelocity = new(currentVelocity.X, currentVelocity.Y);
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

		// Check if the player is running and apply speed multiplier
		if (Input.IsActionPressed("run"))
		{
			currentVelocity *= _speedMultiplier;
		}

		Velocity = currentVelocity;

		// Set the player's currentVelocity and move
		MoveAndSlide();
	}

	#region Attack Setup

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

	#endregion

	#region Item Collection

	// Used for picking up an item that was dropped
	private void OnAreaEntered(Area2D area)
	{
		if (area is ICollectible collectible)
		{
			collectible.Collect(this);
		}
	}

	#endregion

	#region Take Damage

	public void OnDamageTaken(float damage)
	{
		_playerStats.currentHealth -= damage;

		if (_playerStats.currentHealth < 0)
		{
			Kill();
		}
	}

	public void Kill()
	{
		GD.Print("Player is dead.");
	}

	#endregion
}
