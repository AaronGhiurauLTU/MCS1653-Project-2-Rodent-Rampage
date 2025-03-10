using Godot;
using System;
using System.Diagnostics;

public partial class GameManager : Node2D
{
	[Export] private TileMapLayer tileMap;
	[Export] private TowerSelectMenu towerSelectMenu;
	[Export] public Menu gameOverMenu;

	// amount of cash the player starts with
	[Export] private int startingCash = 3;

	// the label that displays the current cash amount
	[Export] private Label cashLabel;
	private int currentCash = 0;

	// null indicates no tile is highlighted, store the tilemap coordinates of the tile the mouse is over
	private Vector2I? currentTileCoordinates;

	// store the atlas coordinates of the current tile that the mouse is over
	private Vector2I? currentTileAtlasCoordinates;

	private string placeableTiles = "Placeable";

	private string catScenePath = "res://Scenes/Cat.tscn",
		owlScenePath = "res://Scenes/Owl.tscn";

	private PackedScene catScene,
		owlScene;

	private enum State
	{
		placing,
		selectingTower
	}

	private State currentState;

	private void ExitPlacing()
	{
		
	}

	private void ExitSelectingTower()
	{
		towerSelectMenu.Visible = false;
	}

	private void EnterPlacing()
	{
		Engine.TimeScale = 1;
	}

	private void EnterSelectingTower()
	{
		Engine.TimeScale = 0;
		towerSelectMenu.Visible = true;
	}

	// handle exiting the old state then entering the new one
	private void ChangeState(State newState)
	{
		if (currentState == newState)
			return;

		ExitState(newState);

		// enter the new state
		switch (newState)
		{
			case State.placing:
				EnterPlacing();
				break;
			case State.selectingTower:
				EnterSelectingTower();
				break;
		}
	}

	// exit the current state and switch to the new state
	private void ExitState(State newState)
	{
		switch (currentState)
		{
			case State.placing:
				ExitPlacing();
				break;
			case State.selectingTower:
				ExitSelectingTower();
				break;
		}

		currentState = newState;
	}


	private bool IsPlacable { get { return currentTileCoordinates != null; } }

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		ChangeState(State.placing);
		towerSelectMenu.TowerSelectionCancelled += TowerSelectionCancelled;
		towerSelectMenu.TowerPlaced += TowerPlaced;

		catScene = GD.Load<PackedScene>(catScenePath);
		owlScene = GD.Load<PackedScene>(owlScenePath);

		gameOverMenu.Visible = false;

		UpdateCash(startingCash);
	}

	private void TowerSelectionCancelled()
	{
		ChangeState(State.placing);
	}

	private void TowerPlaced(string towerName)
	{
		// the position for the tower
		Vector2 position = (tileMap.MapToLocal((Vector2I)currentTileCoordinates) * tileMap.Scale.X / 2) + tileMap.GetParent<Node2D>().Position;
		Tower newTower = null;
	
		switch (towerName)
		{
			case "tower1":
				
				newTower = (Tower)catScene.Instantiate();
				GD.Print("tower 1");
				break;
			case "tower2":
				newTower = (Tower)owlScene.Instantiate();
				GD.Print("tower 2");
				break;
		}

		// ensure the player has enough cash to place the tower, and if not, delete the tower and do nothing else
		if (!UpdateCash(-1 * (newTower?.Cost ?? int.MaxValue)))
		{
			newTower?.QueueFree();
			return;
		}

		newTower.Position = position;
		AddChild(newTower);

		// make the tile unplaceable for where the new tower is
		tileMap.SetCell((Vector2I)currentTileCoordinates, 1,  new Vector2I(3, 4));

		currentTileCoordinates = null;
		currentTileAtlasCoordinates = null;

		ChangeState(State.placing);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(double delta)
	{
		if (currentState != State.placing)
		{
			return;
		}

		Vector2 mousePos = GetViewport().GetMousePosition();

		Vector2I tilePos = tileMap.LocalToMap(tileMap.ToLocal(mousePos));

		TileData tileData = tileMap.GetCellTileData(tilePos);
		
		string tileName = (string)tileData?.GetCustomData("Name");

		// check the name of the tile to see if it is the current placeable tiles name or is highlighted
		if (tileName == placeableTiles || tileName == "Highlight")
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

		if (Input.IsActionJustPressed("leftClick") && IsPlacable)
		{
			ChangeState(State.selectingTower);
		}
	}

	private void ResetPreviousTile()
	{
		// set the previously highlighted tile back to what it was originally
		if (IsPlacable && currentState == State.placing)
		{
			tileMap.SetCell((Vector2I)currentTileCoordinates, 1,  currentTileAtlasCoordinates);
		}
	}
	
	// update the cash amount and the label and returns false if there wasn't enough cash 
	public bool UpdateCash(int cashChange)
	{
		if (currentCash + cashChange < 0)
		{
			return false;
		}

		currentCash += cashChange;
		cashLabel.Text = "Money: $" + currentCash;
		return true;
	}
}
