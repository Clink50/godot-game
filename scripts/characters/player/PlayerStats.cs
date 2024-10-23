using Godot;
using Godot.Collections;

public partial class PlayerStats : Node2D
{
	[Export] private CharacterResource _characterResource;
    [Export] private int _experience = 0;
    [Export] private int _level = 1;
	[Export] private int _experienceCap;
	[Export] private Array<LevelRangeResource> _levelRanges;

	public WeaponType currentWeapon;
	public float currentHealth;
	public float currentRecovery;
	public float currentSpeed;
	public float currentMight;
	public float currentProjectileSpeed;

	public override void _Ready()
	{
		currentWeapon = _characterResource.CurrentWeapon;
		currentHealth = _characterResource.MaxHealth;
		currentRecovery = _characterResource.Recovery;
		currentSpeed = _characterResource.Speed;
		currentMight = _characterResource.Might;
		currentProjectileSpeed = _characterResource.ProjectileSpeed;

		_experienceCap = _levelRanges[0].experienceCapIncrease;
	}

	public void AddExperience(int amount)
    {
        _experience += amount;
        CheckLevelUp();
    }

    private void CheckLevelUp()
    {
        if (_experience >= _experienceCap)
        {
            _level++;
            _experience -= _experienceCap;

			int experienceCapIncrease = 0;

			foreach (var range in _levelRanges)
			{
				if (_level >= range.startLevel && _level <= range.endLevel)
				{
					experienceCapIncrease = range.experienceCapIncrease;
					break;
				}
			}

			_experienceCap += experienceCapIncrease;
        }
    }
}
