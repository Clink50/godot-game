using System.Collections.Generic;
using Godot;

[Tool]
public partial class ChunkMapGenerator : Node2D
{
	private TileMapLayer _groundTileMap;
	private TileMapLayer _propsTileMap;
	private Vector2 _currentChunk;
	private Player _player;

	private const int ChunkSize = 20; // 20x20 grid

    public override void _Ready()
    {
		_player = GetTree().CurrentScene.GetNode<Player>("Player");
		_groundTileMap = GetNode<TileMapLayer>("Ground");
		_propsTileMap = GetNode<TileMapLayer>("Props");
		_currentChunk = new Vector2(0, 0);
		// _player.PositionChanged += CheckPlayerPosition;
		GenerateMap();
    }

	public void OnLeftBoundaryAreaEntered(Area2D area)
	{
		if (area.Name == "ChunkDetector")
		{
			GenerateChunk((int)_currentChunk.X - 1, (int)_currentChunk.Y);
			_currentChunk = new Vector2((int)_currentChunk.X - 1, (int)_currentChunk.Y);
		}
	}

	public void OnLeftBoundaryAreaExited(Area2D area)
	{
		UnloadChunk((int)_currentChunk.X, (int)_currentChunk.Y);
	}

	public void GenerateMap()
	{
		// Loop through each chunk position in the 3x3 grid
		for (var chunkX = -1; chunkX <= 1; chunkX++)
		{
			for (var chunkY = -1; chunkY <= 1; chunkY++)
			{
				GenerateChunk(chunkX, chunkY);
			}
		}
	}

	// Generate a single chunk
	private void GenerateChunk(int chunkX, int chunkY)
	{
		// Offset for this chunk's position
		int xOffset = chunkX * ChunkSize;
		int yOffset = chunkY * ChunkSize;

		// Generate ground tiles for the chunk
		for (var x = 0; x < ChunkSize; x++)
		{
			for (var y = 0; y < ChunkSize; y++)
			{
				var globalX = x + xOffset;
				var globalY = y + yOffset;

				_groundTileMap.SetCell(new Vector2I(globalX, globalY), 0, new Vector2I(GD.RandRange(0, 2), 11));
			}
		}

		// Now place props randomly in the chunk
		var placedProps = new HashSet<Vector2I>(); // Ensure no duplicate positions within the chunk
		var tileSource = _propsTileMap.TileSet.GetSource(1);

		while (placedProps.Count < 20)
		{
			var randomX = GD.RandRange(0, ChunkSize - 1);
			var randomY = GD.RandRange(0, ChunkSize - 1);

			var globalPosition = new Vector2I(randomX + xOffset, randomY + yOffset);

			// Check if position is already used for a prop
			if (!placedProps.Contains(globalPosition))
			{
				var randomPropId = GD.RandRange(0, tileSource.GetTilesCount() - 1);
				var randomProp = tileSource.GetTileId(randomPropId);

				_propsTileMap.SetCell(globalPosition, 1, randomProp);
				placedProps.Add(globalPosition);
			}
		}
	}

	private void UnloadChunk(int chunkX, int chunkY)
	{
		// Offset for this chunk's position
		int xOffset = chunkX * ChunkSize;
		int yOffset = chunkY * ChunkSize;

		// Generate ground tiles for the chunk
		for (var x = 0; x < ChunkSize; x++)
		{
			for (var y = 0; y < ChunkSize; y++)
			{
				var globalX = x + xOffset;
				var globalY = y + yOffset;

				_groundTileMap.SetCell(new Vector2I(globalX, globalY), -1);
			}
		}
	}
}
