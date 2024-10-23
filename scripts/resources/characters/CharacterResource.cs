using Godot;

[GlobalClass]
public partial class CharacterResource : Resource
{
	private WeaponType _currentWeapon;
	[Export]
	public WeaponType CurrentWeapon
	{
		get => _currentWeapon;
		private set => _currentWeapon = value;
	}

	private float _maxHealth;
	[Export]
	public float MaxHealth
	{
		get => _maxHealth;
		private set => _maxHealth = value;
	}

	private float _recovery;
	[Export]
	public float Recovery
	{
		get => _recovery;
		private set => _recovery = value;
	}


	private float _speed;
	[Export]
	public float Speed
	{
		get => _speed;
		private set => _speed = value;
	}

	private float _might;
	[Export]
	public float Might
	{
		get => _might;
		private set => _might = value;
	}

	private float _projectileSpeed;
	[Export]
	public float ProjectileSpeed
	{
		get => _projectileSpeed;
		private set => _projectileSpeed = value;
	}
}
