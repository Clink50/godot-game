using System.Collections.Generic;
using System.Linq;
using Godot;
using Godot.Collections;

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

	// List of placed trees
	private HashSet<Vector2> _placedTreePositions = new();

	public override void _Ready()
	{
		GD.Randomize();
		_noise = _noiseHeightTexture.GetNoise();
		_treeNoise = _noiseTreeTexture.GetNoise();

		// Check if the noise instance has a 'Seed' property
		var seedProperty = _noise.GetPropertyList().Any(p => (string)p["name"] == "seed");

		if (seedProperty)
		{
			// Set the seed dynamically
			_noise.Set("seed", (int)GD.Randi());
		}
		else
		{
			GD.Print("This noise type does not have a Seed property.");
		}

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
				noises.Add(noiseValue);
			}
		}

		var minNoiseValue = noises.Min();
		var maxNoiseValue = noises.Max();

		for (int x = -_width / 2; x < _width / 2; x++)
		{
			for (int y = -_height / 2; y < _height / 2; y++)
			{
				var rawNoiseValue = _noise.GetNoise2D(x, y);
				var noiseValue = NormalizeNoise(rawNoiseValue, minNoiseValue, maxNoiseValue);
				var treeNoiseValue = _treeNoise.GetNoise2D(x, y);

				// All Ground Type
				if (noiseValue >= 0.45f)
				{
					// Sand starts here
					if (noiseValue > 0.5f && noiseValue < 0.6f && treeNoiseValue > 0.8f)
					{
						// _environmentLayer.SetCell(new Vector2I(x, y), _treeSourceId, _sceneCollectionAtlasCoords, _palmTreeSceneIds.PickRandom());
						PlaceTree(new Vector2I(x, y), _palmTreeSceneIds.PickRandom());
					}

					// Grass starts here
					if (noiseValue > 0.65f)
					{
						// Sand to Grass tiles
						_grassTiles.Add(new Vector2I(x, y));

						// Not edge of sand
						if (noiseValue > 0.7f)
						{
							if (noiseValue > 0.8f && treeNoiseValue > 0.8f)
							{
								// _environmentLayer.SetCell(new Vector2I(x, y), _treeSourceId, _sceneCollectionAtlasCoords, 1);
								PlaceTree(new Vector2I(x, y), 1);
							}

							// Different types of grass tiles go here
							_ground2Layer.SetCell(new Vector2I(x, y), _tileSetSourceId, _grassAtlasTiles.PickRandom());
						}

						// Cliffs start here
						if (noiseValue > 0.9)
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
	}

	// Check within a 3 tile radius to see if another tree is nearby, if so then skip the placement so
	// there aren't trees bunched together or on top of each other
	public void PlaceTree(Vector2I tilePlacement, int treeId)
	{
		// Check if there are any trees within the 3-tile radius
		for (int x = -2; x <= 2; x++)
		{
			for (int y = -2; y <= 2; y++)
			{
				var checkPosition = tilePlacement + new Vector2I(x, y);

				// If a tree is within this tile, skip placing a new tree here
				if (_placedTreePositions.Contains(checkPosition))
				{
					return; // Skip this placement
				}
			}
		}

		_environmentLayer.SetCell(tilePlacement, _treeSourceId, _sceneCollectionAtlasCoords, treeId);
		_placedTreePositions.Add(tilePlacement);
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

	private static float NormalizeNoise(float value, float minValue, float maxValue)
		=> (value - minValue)/(maxValue - minValue);
}

// Normalize min and max of noise values to get 0 and 1 so you don't have to change
// the values for each seed or type of noise
// func _normalise(val, min, max):
//    return (val - min)/(max - min)
