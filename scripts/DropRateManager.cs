using Godot;
using Godot.Collections;

public partial class DropRateManager : Node2D
{
	private Node2D _parent;
	[Export] private Array<DropResource> _drops;

	public override void _Ready()
	{
		_parent = GetParent() as Node2D;
		_parent.TreeExited += OnDestroy;
	}

	public void OnDestroy()
	{
		var randomNumber = GD.RandRange(0f, 100f);
		Array<DropResource> possibleDrops = new();

		foreach (var drop in _drops)
		{
			if (randomNumber <= drop.dropRate)
			{
				possibleDrops.Add(drop);
			}
		}

		if (possibleDrops.Count > 0)
		{
			var randomDrop = possibleDrops.PickRandom();
			var dropInstance = randomDrop.item.Instantiate() as Node2D;
			dropInstance.Position = _parent.Position;
			_parent.CallDeferred("add_sibling", dropInstance);
		}
	}
}
