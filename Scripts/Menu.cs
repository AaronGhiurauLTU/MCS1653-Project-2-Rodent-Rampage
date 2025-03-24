using Godot;
using System;

public partial class Menu : Control
{
	[Export] private MarginContainer instructionsContainer, mainMenuContainer;

	public override void _Ready()
	{
		if (Name == "MainMenu")
		{
			MusicManager.PlaySong(MusicManager.Song.Background);
		}
	}
	private void OnPlayPressed()
	{
		Engine.TimeScale = 1;
		GetTree().ChangeSceneToFile("res://Scenes/Level.tscn");
	}

	private void OnMainMenuPressed()
	{
		Engine.TimeScale = 1;
		GD.Print("main menu");
		GetTree().ChangeSceneToFile("res://Scenes/MainMenu.tscn");
	}

	private void OnQuitPressed()
	{
		GetTree().Quit();
	}

	private void OnInstructionsPressed()
	{
		instructionsContainer.Visible = true;
		mainMenuContainer.Visible = false;	
	}
	private void OnInstructionsBackPressed()
	{
		instructionsContainer.Visible = false;
		mainMenuContainer.Visible = true;
	}

	private void OnContinuePressed()
	{
		Visible = false;
		Engine.TimeScale = 1;
		MusicManager.SetBusVolume("Music", 0);
	}
}
