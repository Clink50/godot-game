using Godot;

public partial class KnifeController : Node2D
{
    [Export] private float _throwSpeed = 400f; // Speed at which the knife moves
	[Export] private int _currentPierce = 2;

	private Vector2 _throwDirection;
    private Vector2 _velocity;

    public override void _Process(double delta)
    {
        // Move the knife based on the velocity
        Position += _velocity * (float)delta;
    }

	// New method to handle knife throw direction and orientation internally
	public void ThrowInDirection(Vector2 direction)
	{
		_velocity = direction.Normalized() * _throwSpeed;
		UpdateOrientation(direction);
	}

	public void UpdateOrientation(Vector2 direction)
	{
		var scale = Scale;
		var rotation = RotationDegrees;

		switch (true)
		{
			case var _ when direction.X > 0 && direction.Y == 0:
				scale.X *= 1;
				scale.Y *= 1;
				break;
			case var _ when direction.X < 0 && direction.Y == 0:
				scale.X *= -1;
				scale.Y *= -1;
				break;
			case var _ when direction.X == 0 && direction.Y < 0:
				scale.X *= -1;
				break;
			case var _ when direction.X == 0 && direction.Y > 0:
				scale.Y *= -1;
				break;
			case var _ when direction.X < 0 && direction.Y < 0:
				rotation = -90f;
				break;
			case var _ when direction.X > 0 && direction.Y < 0:
				rotation = 0f;
				break;
			case var _ when direction.X < 0 && direction.Y > 0:
				rotation = 0f;
				scale.X *= -1;
				scale.Y *= -1;
				break;
			case var _ when direction.X > 0 && direction.Y > 0:
				rotation = -90f;
				scale.X *= -1;
				scale.Y *= -1;
				break;
		}

		Scale = scale;
		RotationDegrees = rotation;
	}

	public void OnHitboxAreaEntered(Area2D area)
	{
		if (area is HitboxComponent hitboxComponent)
		{
			hitboxComponent.Damage(10);
			ReducePierce();
		}
	}

	public void ReducePierce()
	{
		if (_currentPierce <= 0)
		{
			QueueFree();
		}
		else
		{
			_currentPierce--;
		}
	}

	public void OnDespawnTimerTimeout()
	{
		QueueFree();
	}
}
