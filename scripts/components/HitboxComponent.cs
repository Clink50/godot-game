using Godot;

public partial class HitboxComponent : Area2D
{
	[Export]
	private HealthComponent _healthComponent;

	public void Damage(float damage)
	{
		_healthComponent.Damage(damage);
	}
}
