using System.Collections.Generic;
using Godot;

public partial class MapController : Node2D
{
	public List<Node2D> terrainChunks;
	private Player _player;

	public override void _Ready()
	{
		_player = GetNode<Player>("Player");
	}

	public override void _Process(double delta)
	{
		_player.PositionChanged += ChunkChecker;
	}

	private void ChunkChecker(string direction)
	{
		if (direction == "right")
		{

		}
	}
}
