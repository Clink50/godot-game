using System.Collections.Generic;
using System.Linq;
using Godot;
using Godot.Collections;

public partial class ProcGenWorld : Node2D
{
	[Export]
	private NoiseTexture2D _noiseHeightTexture; // Water -> Sand -> Grass -> Cliffs
	private Noise _noise;

	[Export]
	private NoiseTexture2D _noiseEnvironmentTexture; // Environment / Trees
	private Noise _environmentNoise;

	private TileMapLayer _waterLayer;
	private TileMapLayer _ground1Layer;
	private TileMapLayer _ground2Layer;
	private TileMapLayer _cliffLayer;
	private TileMapLayer _environmentLayer;

	private Camera2D _camera2d;

	private int _width = 200;
	private int _height = 200;

	private int _sourceId = 1;
	private Vector2I _waterAtlas = new(0, 1);
	private Vector2I _landAtlas = new(0, 0);

	private Array<Vector2I> _sandTiles = new();
	private Array<Vector2I> _grassTiles = new();
	private Array<Vector2I> _cliffTiles = new();
	private int _sandTerrainIndex = 0;
	private int _grassTerrainIndex = 1;
	private int _cliffTerrainIndex = 3;

	private Array<Vector2I> _grassAtlasTiles = new()
	{
		new Vector2I(1, 0),
		new Vector2I(2, 0),
		new Vector2I(3, 0),
		new Vector2I(4, 0),
		new Vector2I(5, 0),
	};

	private Array<Vector2I> _palmTreeAtlas = new()
	{
		new Vector2I(12, 2),
		new Vector2I(15, 2),
	};

	// private Array<Vector2I> _oakTreeAtlas = new()
	// {
	// 	new Vector2I(15, 6),
	// };
	[Export]
	private PackedScene _oakTreeScene;

	public override void _Ready()
	{
		GD.Randomize();
		_noise = _noiseHeightTexture.GetNoise();

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

		_environmentNoise = _noiseEnvironmentTexture.GetNoise();

		_waterLayer = GetNode<TileMapLayer>("Water");
		_ground1Layer = GetNode<TileMapLayer>("Ground1");
		_ground2Layer = GetNode<TileMapLayer>("Ground2");
		_cliffLayer = GetNode<TileMapLayer>("Cliff");
		_environmentLayer = GetNode<TileMapLayer>("Environment");

		_camera2d = GetNode<Camera2D>("Player/Camera2D");

		GenerateWorld();
	}

	private void GenerateWorld()
	{
		var treeCount = 0;
		var noises = new List<float>();
		var environmentNoises = new List<float>();

		for (int x = -_width / 2; x < _width / 2; x++)
		{
			for (int y = -_height / 2; y < _height / 2; y++)
			{
				var noiseValue = _noise.GetNoise2D(x, y);
				var environmentNoiseValue = _environmentNoise.GetNoise2D(x, y);

				noises.Add(noiseValue);
				environmentNoises.Add(noiseValue);

				if (noiseValue > 0.6f)
				{
					_cliffTiles.Add(new Vector2I(x, y));
				}

				// Placing all grass
				if (noiseValue > 0.2f)
				{
					_grassTiles.Add(new Vector2I(x, y));

					if (noiseValue > 0.3f)
					{
						// Place random grass
						_ground2Layer.SetCell(new Vector2I(x, y), _sourceId, _grassAtlasTiles.PickRandom());
					}

					if (environmentNoiseValue > 0.9f && noiseValue > 0.3f && noiseValue < 0.5f && treeCount < 6)
					{
						// _environmentLayer.SetCell(new Vector2I(x, y), _sourceId, _oakTreeAtlas.PickRandom());
						// Instantiate an oak tree
						var oakTreeInstance = (Node2D)_oakTreeScene.Instantiate();
						AddChild(oakTreeInstance);
						oakTreeInstance.GlobalPosition = new Vector2(x, y) * _environmentLayer.TileSet.TileSize;
						treeCount++;
					}
				}

				if (noiseValue > 0f)
				{
					// Add sand
					_sandTiles.Add(new Vector2I(x, y));

					// Add palm trees but not too close to the edge of the sand so it won't be up against the water
					if (noiseValue < 0.18f && environmentNoiseValue > 0.92f)
					{
						_environmentLayer.SetCell(new Vector2I(x, y), _sourceId, _palmTreeAtlas.PickRandom());
					}
				}

				// Place water on every tile
				_waterLayer.SetCell(new Vector2I(x, y), _sourceId, _waterAtlas);
			}
		}

		_ground1Layer.SetCellsTerrainConnect(_sandTiles, _sandTerrainIndex, 0);
		_ground1Layer.SetCellsTerrainConnect(_grassTiles, _grassTerrainIndex, 0);
		_cliffLayer.SetCellsTerrainConnect(_cliffTiles, _cliffTerrainIndex, 0);


		GD.Print("Terrain Max: " + noises.Max() + " - Min: " + noises.Min());
		GD.Print("Environment Max: " + environmentNoises.Max() + " - Min: " + environmentNoises.Min());
	}

	public override void _Input(InputEvent @event)
	{
		if (Input.IsActionJustPressed("zoomIn"))
		{
			var zoomValue = (float)(_camera2d.Zoom.X + 0.1);
			_camera2d.Zoom = new Vector2(zoomValue, zoomValue);
		}
		else if (Input.IsActionJustPressed("zoomOut"))
		{
			var zoomValue = (float)(_camera2d.Zoom.X - 0.1);
			if (zoomValue == 0)
			{
				zoomValue = (float)(_camera2d.Zoom.X - 0.2);
			}

			_camera2d.Zoom = new Vector2(zoomValue, zoomValue);
		}
	}
}
