using Godot;

[GlobalClass]
public partial class DropResource : Resource
{
	[Export] public string name;
	[Export] public PackedScene item;
	[Export] public float dropRate;
}
