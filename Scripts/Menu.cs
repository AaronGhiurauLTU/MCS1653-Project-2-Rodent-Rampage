using Godot;
using System;

public partial class Menu : Control
{
	private void OnPlayPressed()
	{
		Engine.TimeScale = 1;
		GetTree().ChangeSceneToFile("res://Scenes/Level.tscn");
	}

	private void OnMainMenuPressed()
	{
		Engine.TimeScale = 1;
		GetTree().ChangeSceneToFile("res://Scenes/MainMenu.tscn");
	}

	private void OnQuitPressed()
	{
		GetTree().Quit();
	}
}
