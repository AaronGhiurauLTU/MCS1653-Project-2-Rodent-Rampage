using Godot;
using System;

public partial class TowerSelectMenu : Control
{
	[Export] private Button tower1Button, tower2Button;
	[Export] private string catScenePath = "res://Scenes/Cat.tscn",
		owlScenePath = "res://Scenes/Owl.tscn";
	[Signal] public delegate void TowerSelectionCancelledEventHandler();
	[Signal] public delegate void TowerPlacedEventHandler(string towerName);
	public PackedScene catScene,
		owlScene;
	private void OnTowerSelected(string towerName)
	{
		EmitSignal(SignalName.TowerPlaced, towerName);
	}

	private void OnCancelPressed()
	{
		EmitSignal(SignalName.TowerSelectionCancelled);
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		catScene = GD.Load<PackedScene>(catScenePath);
		owlScene = GD.Load<PackedScene>(owlScenePath);

		Tower cat = (Tower)catScene.Instantiate();

		tower1Button.Text = $"(${cat.Cost}) Cat";
		cat.QueueFree();

		Tower owl = (Tower)owlScene.Instantiate();
		tower2Button.Text = $"(${owl.Cost}) Owl";

		owl.QueueFree();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (Input.IsActionJustReleased("cancel"))
		{
			EmitSignal(SignalName.TowerSelectionCancelled);
		}
	}
}
