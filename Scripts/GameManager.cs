using Godot;
using System;

public partial class GameManager : Node2D
{
	TileMapLayer tileMap;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		tileMap = GetNode<TileMapLayer>("%Background");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(double delta)
	{
		// offset mouse position by half the tile size for accurate detection
		Vector2 mousePos = GetViewport().GetMousePosition() + Vector2.Down * 20;

		Vector2I tilePos = tileMap.LocalToMap(tileMap.ToLocal(mousePos));

		TileData tileData = tileMap.GetCellTileData(tilePos);

		if (tileData is not null)
		{
			GD.Print(tileData.GetCustomData("Name"));
		} 
	}
}
