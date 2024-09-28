using Godot;

public partial class Robot : CharacterBody2D
{
	private float _gravity = (float)ProjectSettings.GetSetting("physics/2d/default_gravity");
	[Export] private int _walkSpeed = 200; // pixels/s
	[Export] private int _jumpSpeed = 200; // pixels/s

	public override void _PhysicsProcess(double delta)
	{
		// Move down one pixel per physics frame
		// Physics frame = 60/s
		// MoveAndCollide(new Vector2(0, 1));

		var velocity = Velocity;
		// Why do we do "velocity.Y += ..." instead of just setting the "velocity.Y = ..."?
        // += is used because gravity is a continuous force that affects the character's velocity over time.
        // The delta represents the time step (how much time has passed since the last physics frame).
        // So, by doing velocity.Y += ..., you are accumulating gravity’s influence over multiple frames,
        // ***making the character fall faster the longer they’re falling (simulating realistic gravity).
		velocity.Y += (float)delta * _gravity;

		if (Input.IsActionPressed("ui_left"))
		{
			velocity.X = -_walkSpeed;
		}
		else if (Input.IsActionPressed("ui_right"))
		{
			velocity.X = _walkSpeed;
		}
		else
		{
			velocity.X = 0;
		}

		Velocity = velocity;

		// "MoveAndSlide" already takes delta time into account.
		MoveAndSlide();

		if (IsOnFloor() && Input.IsActionJustPressed("ui_select"))
		{
			velocity.Y = -_jumpSpeed;
		}
	}
}
