using Godot;

public partial class KnifeWeapon : Node2D
{
    [Export] private float _throwSpeed = 400f; // Speed at which the knife moves

	private Vector2 _throwDirection;
    private Vector2 _velocity;

    public override void _Process(double delta)
    {
        // Move the knife based on the velocitay
        Position += _velocity * (float)delta;
    }

	public void OnDespawnTimerTimeout()
	{
		QueueFree();
	}

	public void OnHitboxAreaEntered(Area2D area)
	{
		if (area is HitboxComponent hitboxComponent)
		{
			hitboxComponent.Damage(10);
			QueueFree();
		}
	}

	public void SetDirection(Vector2 direction)
	{
		_velocity = direction.Normalized() * _throwSpeed;
	}
}
