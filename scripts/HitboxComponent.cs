using Godot;

public partial class HitboxComponent : Area2D
{
	[Export]
	private HealthComponent _healthComponent;

	public void Damage(float damage)
	{
		_healthComponent.Damage(damage);
	}

	public void OnAreaEntered(Area2D area)
    {
        GD.Print("Area hit by: " + area.Name);  // Prints the name of the object that hit the tree
    }
}
