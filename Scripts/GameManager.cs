using Godot;
using System;

public partial class GameManager : Node2D
{
	[Export] private TileMapLayer tileMap;

	// null indicates no tile is highlighted, store the tilemap coordinates of the tile the mouse is over
	private Vector2I? currentTileCoordinates;

	// store the atlas coordinates of the current tile that the mouse is over
	private Vector2I? currentTileAtlasCoordinates;

	private string placeableTiles = "Background";

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(double delta)
	{
		// offset mouse position by half the tile size for accurate detection
		Vector2 mousePos = GetViewport().GetMousePosition() + (Vector2.Down * tileMap.TileSet.TileSize.X / 2);

		Vector2I tilePos = tileMap.LocalToMap(tileMap.ToLocal(mousePos));

		TileData tileData = tileMap.GetCellTileData(tilePos);
		
		// check the name of the tile to see if it is the current placeable tiles name or is highlighted
		if ((string)tileData?.GetCustomData("Name") == placeableTiles || (string)tileData?.GetCustomData("Name") == "Highlight")
		{
			// no need to update anything if the mouse stayed over the same tile last frame
			if (currentTileCoordinates != tilePos)
			{
				ResetPreviousTile();

				// update the current tile information
				currentTileCoordinates = tilePos;
				currentTileAtlasCoordinates = tileMap.GetCellAtlasCoords(tilePos);

				// set cell to the highlighted cell
				tileMap.SetCell(tilePos, 1,  new Vector2I(0, 0));
			}
		} 
		else
		{
			ResetPreviousTile();
			currentTileCoordinates = null;
			currentTileAtlasCoordinates = null;
		}
	}

	private void ResetPreviousTile()
	{
		// set the previously highlighted tile back to what it was originally
		if (currentTileCoordinates != null)
		{
			tileMap.SetCell((Vector2I)currentTileCoordinates, 1,  currentTileAtlasCoordinates);
		}
	}
}
