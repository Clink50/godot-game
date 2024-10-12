using System;
using Godot;

public partial class Player : CharacterBody2D
{
	[Export] private float _speed = 150f;
	[Export] private float _speedMultiplier = 2;
	[Export] private PackedScene _knifeScene;
	private Sprite2D _playerSprite;
	private AnimationPlayer _animationPlayer;

	public Vector2 velocity;
	public Vector2 lastVelocity;

	public override void _Ready()
	{
		_playerSprite = GetNode<Sprite2D>("Sprite2D");
		_animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		lastVelocity = Vector2.Right;
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
		var knifeInstance = (KnifeWeapon)_knifeScene.Instantiate();
		knifeInstance.Position = new Vector2(Position.X, Position.Y - 8);
		SetKnifeOrientation(knifeInstance);
		knifeInstance.SetDirection(lastVelocity);
		GetParent().AddChild(knifeInstance);
	}

	public void SetKnifeOrientation(KnifeWeapon knife)
	{
		var scale = knife.Scale;
		var rotation = knife.RotationDegrees;

		switch (GetPlayerDirection())
		{
			case "left":
				scale.X *= -1;
				scale.Y *= -1;
				break;
			case "up":
				scale.X *= -1;
				break;
			case "down":
				scale.Y *= -1;
				break;
			case "topLeft":
				rotation = -90f;
				break;
			case "topRight":
				rotation = 0f;
				break;
			case "bottomLeft":
				rotation = 0f;
				scale.X *= -1;
				scale.Y *= -1;
				break;
			case "bottomRight":
				rotation = -90f;
				scale.X *= -1;
				scale.Y *= -1;
				break;
		}

		knife.Scale = scale;
		knife.RotationDegrees = rotation;
	}

	public string GetPlayerDirection()
	{
		return true switch
		{
			var _ when lastVelocity.X > 0 && lastVelocity.Y == 0 => "right",
			var _ when lastVelocity.X < 0 && lastVelocity.Y == 0 => "left",
			var _ when lastVelocity.X == 0 && lastVelocity.Y > 0 => "down",
			var _ when lastVelocity.X == 0 && lastVelocity.Y < 0 => "up",
			var _ when lastVelocity.X > 0 && lastVelocity.Y < 0 => "topRight",
			var _ when lastVelocity.X < 0 && lastVelocity.Y < 0 => "topLeft",
			var _ when lastVelocity.X > 0 && lastVelocity.Y > 0 => "bottomRight",
			var _ when lastVelocity.X < 0 && lastVelocity.Y > 0 => "bottomLeft",
			_ => "idle",
		};
	}
}
