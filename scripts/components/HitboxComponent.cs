using Godot;

public partial class HitboxComponent : Area2D
{
    [Signal]
    public delegate void DamageTakenEventHandler(float damage);

    [Export]
    private HealthComponent _healthComponent;

    public void Damage(float damage)
    {
        if (_healthComponent != null)
        {
            _healthComponent.Damage(damage);
        }
        else
        {
            EmitSignal(SignalName.DamageTaken, damage);
        }
    }
}