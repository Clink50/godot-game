using System.Collections.Generic;
using System.Linq;
using Godot;
using Godot.Collections;

[Tool]
public partial class ProcGenWorld : Node2D
{
	[Export] private NoiseTexture2D _noiseHeightTexture;
	[Export] private NoiseTexture2D _noiseTreeTexture;
	private Noise _noise;
	private Noise _treeNoise;
	[Export] private PackedScene _oakTreeScene;
	[Export] private CharacterBody2D _player;

	// Layers
	private TileMapLayer _waterLayer;
	private TileMapLayer _groundLayer;
	private TileMapLayer _ground2Layer;
	private TileMapLayer _cliffLayer;
	private TileMapLayer _environmentLayer;

	// Map Grid
	private int _width = 100;
	private int _height = 100;

	private int _tileSetSourceId = 1;
	private int _treeSourceId = 0;
	private Vector2I _sceneCollectionAtlasCoords = new(0, 0);

	// Terrain Sets
	private int _sandTerrainSet = 0;
	private int _grassTerrainSet = 1;
	private int _cliffTerrainSet = 3;

	// Terrain Tiles
	private Vector2I _waterAtlas = new(0, 1);
	private Vector2I _landAtlas = new(0, 0);
	private Array<Vector2I> _sandTiles = new();
	private Array<Vector2I> _grassTiles = new();
	private Array<Vector2I> _cliffTiles = new();
	private Array<Vector2I> _grassAtlasTiles = new()
	{
		new Vector2I(1, 0),
		new Vector2I(2, 0),
		new Vector2I(3, 0),
		new Vector2I(4, 0),
		new Vector2I(5, 0),
	};

	// Environment Tiles
	private Array<int> _palmTreeSceneIds = new() { 2, 3 };

	public override void _Ready()
	{
		_noise = _noiseHeightTexture.GetNoise();
		_treeNoise = _noiseTreeTexture.GetNoise();

		_waterLayer = GetNode<TileMapLayer>("Water");
		_groundLayer = GetNode<TileMapLayer>("Ground");
		_ground2Layer = GetNode<TileMapLayer>("Ground2");
		_cliffLayer = GetNode<TileMapLayer>("Cliff");
		_environmentLayer = GetNode<TileMapLayer>("Environment");

		_player = GetNode<CharacterBody2D>("Player");

		GenerateWorld();
	}

	private void GenerateWorld()
	{
		var noises = new List<float>();
		var treeNoises = new List<float>();

		for (int x = -_width / 2; x < _width / 2; x++)
		{
			for (int y = -_height / 2; y < _height / 2; y++)
			{
				var noiseValue = _noise.GetNoise2D(x, y);
				var treeNoiseValue = _treeNoise.GetNoise2D(x, y);
				noises.Add(noiseValue);
				treeNoises.Add(treeNoiseValue);

				// All Ground Type
				if (noiseValue >= 0f)
				{
					// Sand starts here
					if (noiseValue > 0.05f && noiseValue < 0.17f && treeNoiseValue > 0.8f)
					{
						_environmentLayer.SetCell(new Vector2I(x, y), _treeSourceId, _sceneCollectionAtlasCoords, _palmTreeSceneIds.PickRandom());
					}

					// Grass starts here
					if (noiseValue > 0.2f)
					{
						// Sand to Grass tiles
						_grassTiles.Add(new Vector2I(x, y));

						// Not edge of sand
						if (noiseValue > 0.25f)
						{
							if (noiseValue > 0.35f && treeNoiseValue > 0.8f)
							{
								_environmentLayer.SetCell(new Vector2I(x, y), _treeSourceId, _sceneCollectionAtlasCoords, 1);
							}

							// Different types of grass tiles go here
							_ground2Layer.SetCell(new Vector2I(x, y), _tileSetSourceId, _grassAtlasTiles.PickRandom());
						}

						// Cliffs start here
						if (noiseValue > 0.4)
						{
							_cliffTiles.Add(new Vector2I(x, y));
						}
					}

					_sandTiles.Add(new Vector2I(x, y));
				}

				_waterLayer.SetCell(new Vector2I(x, y), _tileSetSourceId, _waterAtlas);
			}
		}

		_groundLayer.SetCellsTerrainConnect(_sandTiles, _sandTerrainSet, 0);
		_groundLayer.SetCellsTerrainConnect(_grassTiles, _grassTerrainSet, 0);
		_cliffLayer.SetCellsTerrainConnect(_cliffTiles, _cliffTerrainSet, 0);

		GD.Print("Noise Max: " + noises.Max());
		GD.Print("Noise Min: " + noises.Min());
		GD.Print("Tree Noise Max: " + treeNoises.Max());
		GD.Print("Tree Noise Min: " + treeNoises.Min());
	}

	public override void _Input(InputEvent @event)
	{
		var camera2d = _player.GetNode<Camera2D>("Camera2D");
		if (Input.IsActionJustPressed("zoomIn"))
		{
			var zoomValue = (float)(camera2d.Zoom.X + 0.1);
			camera2d.Zoom = new Vector2(zoomValue, zoomValue);
		}
		else if (Input.IsActionJustPressed("zoomOut"))
		{
			var zoomValue = (float)(camera2d.Zoom.X - 0.1);
			if (zoomValue == 0)
			{
				zoomValue = (float)(camera2d.Zoom.X - 0.2);
			}

			camera2d.Zoom = new Vector2(zoomValue, zoomValue);
		}
	}
}

// Normalize min and max of noise values to get 0 and 1 so you don't have to change
// the values for each seed or type of noise
// func _normalise(val, min, max):
//    return (val - min)/(max - min)
