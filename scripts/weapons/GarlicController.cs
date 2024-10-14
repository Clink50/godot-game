using Godot;

public partial class GarlicController : Node2D, IBaseWeapon
{
	public void Activate(Player player)
	{
		GD.Print("Garlic activated.");
	}
}
