using Godot;
using System;
using System.Diagnostics;

public partial class GameManager : Node2D
{
	[Export] private TileMapLayer tileMap;
	[Export] private TowerSelectMenu towerSelectMenu;
	[Export] public Menu gameOverMenu, gameWonMenu;
	[Export] private AnimationPlayer tower1ButtonAnim, tower2buttonAnim;

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

	private bool canPlace = false;
	private enum State
	{
		placing,
		selectingTower
	}

	private State currentState;
	public int currentHealth = 5;

	private void ExitPlacing()
	{
		
	}

	private void ExitSelectingTower()
	{
		towerSelectMenu.Visible = false;
		MusicManager.SetBusVolume("Music", 0);
	}

	private void EnterPlacing()
	{
		Engine.TimeScale = 1;
	}

	private void EnterSelectingTower()
	{
		// Engine.TimeScale = 0;
		// MusicManager.SetBusVolume("Music", -5);
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

		gameOverMenu.Visible = false;
		gameWonMenu.Visible = false;

		UpdateCash(startingCash);

		MusicManager.PlaySong(MusicManager.Song.Background);
		MusicManager.SetBusVolume("Music", 0);
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
		AnimationPlayer animationPlayer = null;

		switch (towerName)
		{
			case "tower1":	
				newTower = (Tower)towerSelectMenu.catScene.Instantiate();
				animationPlayer = tower1ButtonAnim;
				break;
			case "tower2":
				newTower = (Tower)towerSelectMenu.owlScene.Instantiate();
				animationPlayer = tower2buttonAnim;
				break;
		}

		// ensure the player has enough cash to place the tower, and if not, delete the tower and do nothing else
		if (!UpdateCash(-1 * (newTower?.Cost ?? int.MaxValue)))
		{
			newTower?.QueueFree();
			animationPlayer.Play("flash red");
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
		if (currentState != State.placing || Engine.TimeScale == 0)
			return;

		Vector2 mousePos = GetViewport().GetMousePosition();

		Vector2I tilePos = tileMap.LocalToMap(tileMap.ToLocal(mousePos));

		TileData tileData = tileMap.GetCellTileData(tilePos);
		
		string tileName = (string)tileData?.GetCustomData("Name");

		// check the name of the tile to see if it is the current placeable tiles name or is highlighted
		if (tileName != null)
		{
			// no need to update anything if the mouse stayed over the same tile last frame
			if (currentTileCoordinates != tilePos)
			{
				ResetPreviousTile();

				// update the current tile information
				currentTileCoordinates = tilePos;
				currentTileAtlasCoordinates = tileMap.GetCellAtlasCoords(tilePos);

				if (tileName == placeableTiles)
				{
					canPlace = true;
					// set cell to the highlighted cell
					tileMap.SetCell(tilePos, 1,  new Vector2I(0, 0));
				}
				else
				{
					canPlace = false;
					// set cell to the highlighted cell
					tileMap.SetCell(tilePos, 1,  new Vector2I(1, 0));
				}

			}
		} 
		else
		{
			canPlace = false;
			ResetPreviousTile();
			currentTileCoordinates = null;
			currentTileAtlasCoordinates = null;
		}

		if (Input.IsActionJustPressed("leftClick") && IsPlacable && canPlace)
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
