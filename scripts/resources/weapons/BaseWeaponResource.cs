using Godot;

public partial class BaseWeaponResource : Resource
{
    private float _damage;
	[Export]
	public float Damage
	{
		get => _damage;
		private set => _damage = value;
	}

    private float _speed;
	[Export]
	public float Speed
	{
		get => _speed;
		private set => _speed = value;
	}

    private int _pierce;
	[Export]
	public int Pierce
	{
		get => _pierce;
		private set => _pierce = value;
	}

}
