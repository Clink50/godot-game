using Godot;

public partial class KnifeWeapon : RigidBody2D
{
	public void _PhysicsProcess()
	{
		ApplyImpulse(Vector2.Zero, new Vector2(300, 0));
	}

	public void OnHitboxComponentAreaEntered(Node body)
    {
        GD.Print("Weapon hit by: " + body.Name);  // Prints the name of the body that was hit
    }
}
