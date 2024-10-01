using Godot;

public partial class Orc : Area2D
{
	private int _health = 2;
	public AnimatedSprite2D _animatedSprite;
	private bool _isDead = false;

	public override void _Ready()
	{
		_animatedSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		_animatedSprite.Play("idle");
	}

	public override void _Process(double delta)
	{
		if (_health <= 0 && !_isDead)
		{
			_animatedSprite.Play("death");
			_isDead = true;
			GetNode<CollisionShape2D>("CollisionShape2D").Disabled = true;
		}

		if (_isDead)
		{
			_animatedSprite.Frame = 3;
		}
	}

	public void OnAttackCollision(Area2D collider)
	{
		_animatedSprite.Play("damageTaken");
		_health--;
	}

	public void OnAnimationFinished()
	{
		if (!_isDead)
		{
			_animatedSprite.Play("idle");
		}
	}
}
