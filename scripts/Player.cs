using Godot;

public partial class Player : CharacterBody2D
{
	[Export] private float _speed = 150;
    [Export] private float _jumpForce = 500;
    [Export] private float _gravity = 20;

	private AnimationPlayer _animationPlayer;
	private Sprite2D _playerSprite;

	public override void _Ready()
	{
		_playerSprite = GetNode<Sprite2D>("Sprite2D");
		_animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		_animationPlayer.Play("idle");
	}

	public float GetInput()
	{
		var velocity = Velocity; // Set the current velocity of the player
		velocity.X = 0; // Reset the velocity

		// Get the input
		var horizontalDirection = Input.GetAxis("ui_left", "ui_right");
		var jump = Input.IsActionPressed("ui_select");
		var attack = Input.IsActionJustPressed("primaryAttack");

		if (IsOnFloor() && jump)
		{
			velocity.Y = -_jumpForce;
		}

		velocity.X = _speed * horizontalDirection;

		if (horizontalDirection != 0)
		{
			_playerSprite.FlipH = horizontalDirection == -1;
		}

		Velocity = velocity;

		return horizontalDirection;
	}

    public override void _PhysicsProcess(double delta)
    {
		if (!IsOnFloor())
		{
			var velocity = Velocity;
			velocity.Y += _gravity;

			// Limit the gravity
			if (velocity.Y > 1000)
			{
				velocity.Y = 1000;
			}

			Velocity = velocity;
		}

		var horizontalDirection = GetInput();

		MoveAndSlide();

		UpdateAnimations(horizontalDirection);
    }

	public void UpdateAnimations(float horizontalDirection)
	{
		if (IsOnFloor() && horizontalDirection == 0)
		{
			_animationPlayer.Play("idle");
		}
		else
		{
			_animationPlayer.Play("walk");
		}
	}
}
