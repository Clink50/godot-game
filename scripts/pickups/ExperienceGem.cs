using Godot;

public partial class ExperienceGem : Area2D, ICollectible
{
    [Export] private int _experienceGranted;

    public void Collect(Node2D body)
    {
        if (body is CharacterBody2D player)
        {
            var playerStats = player.GetNode<PlayerStats>("PlayerStats");
            playerStats.AddExperience(_experienceGranted);
            QueueFree();
        }
    }
}