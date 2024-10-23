using System.Linq;
using Godot;
using Godot.Collections;

public partial class PlayerStats : Node2D
{
    [Export] private int _experience = 0;
    [Export] private int _level = 1;
	[Export] private int _experienceCap;
	[Export] private Array<LevelRangeResource> _levelRanges;

	public override void _Ready()
	{
		_experienceCap = _levelRanges.First().experienceCapIncrease;
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
