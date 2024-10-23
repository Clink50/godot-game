using Godot;

public partial class EnemyStats : Node2D
{
	[Export] private EnemyResource _enemyResource;

	public WeaponType currentWeapon;
	public float currentSpeed;
	public float currentHealth;
	public float currentDamage;

	public override void _Ready()
	{
		currentSpeed = _enemyResource.Speed;
		currentHealth = _enemyResource.MaxHealth;
		currentDamage = _enemyResource.Damage;
	}
}
