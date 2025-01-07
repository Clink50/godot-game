using Godot;

[GlobalClass]
public partial class LevelRangeResource : Resource
{
    [Export] public int startLevel;
    [Export] public int endLevel;
    [Export] public int experienceCapIncrease;
}