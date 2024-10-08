using Godot;
using Godot.Collections;

[Tool]
public partial class PropRandomizer : TileMapLayer
{
	private Array<Node> _propLocations;

	public override void _Ready()
	{
		_propLocations = GetChildren();

		SpawnProps();
	}

	public void SpawnProps()
	{
		var tileSource = TileSet.GetSource(1);

		foreach (var marker in _propLocations)
		{
			var markerLocation = (marker as Marker2D).Position;
			var location = LocalToMap(markerLocation);

			var randomPropId = GD.RandRange(0, tileSource.GetTilesCount() - 1);
			var randomProp = tileSource.GetTileId(randomPropId);

			SetCell(location, 1, randomProp);
		}
	}
}
