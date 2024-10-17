using Godot;

public partial class KnifeController : Node2D, IBaseWeapon
{
	[Export] private KnifeWeaponResource _knifeWeaponResource;

	private Vector2 _throwDirection;
    private Vector2 _velocity;
	private float _currentSpeed;
	private float _currentDamage;
	private float _currentPierce;

	public override void _Ready()
	{
		_currentSpeed = _knifeWeaponResource.Speed;
		_currentDamage = _knifeWeaponResource.Damage;
		_currentPierce = _knifeWeaponResource.Pierce;
	}

    public override void _PhysicsProcess(double delta)
    {
        // Move the knife based on the velocity
        Position += _velocity * _currentSpeed * (float)delta;
    }

	public void Activate(Player player)
	{
		Position = new Vector2(player.Position.X, player.Position.Y - 8);
		ThrowInDirection(player.LastVelocity);
		player.GetParent().AddChild(this);
	}

	// New method to handle knife throw direction and orientation internally
	public void ThrowInDirection(Vector2 direction)
	{
		_velocity = direction.Normalized();
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
			hitboxComponent.Damage(_currentDamage);
			ReducePierce();
		}
	}

	public void ReducePierce()
	{
		_currentPierce--;

		if (_currentPierce <= 0)
		{
			QueueFree();
		}
	}

	public void OnDespawnTimerTimeout()
	{
		QueueFree();
	}
}
