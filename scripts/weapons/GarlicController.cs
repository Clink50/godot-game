using Godot;

public partial class GarlicController : Node2D, IBaseWeapon
{
	[Signal]
	public delegate void GarlicDespawnedEventHandler();

	[Export]
	public GarlicWeaponResource _garlicWeaponResource;

	public void Activate(Player player)
	{
		player.StopAttackTimer();
		player.AddChild(this);
	}

	public void OnDespawnTimerTimeout()
	{
		EmitSignal(SignalName.GarlicDespawned);
		QueueFree();
	}
}
