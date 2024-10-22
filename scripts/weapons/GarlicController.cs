using Godot;

public partial class GarlicController : Node2D, IBaseWeapon
{
	[Signal]
	public delegate void GarlicDespawnedEventHandler();

	[Export]
	public GarlicWeaponResource _garlicWeaponResource;
	private float _currentDamage;

	public override void _Ready()
	{
		_currentDamage = _garlicWeaponResource.Damage;
	}

	public void Activate(Player player)
	{
		player.StopAttackTimer();
		player.AddChild(this);
	}

	public void OnHitboxAreaEntered(Area2D area)
	{
		if (area is HitboxComponent hitboxComponent)
		{
			hitboxComponent.Damage(_currentDamage);
		}
	}

	public void OnDespawnTimerTimeout()
	{
		EmitSignal(SignalName.GarlicDespawned);
		QueueFree();
	}
}
