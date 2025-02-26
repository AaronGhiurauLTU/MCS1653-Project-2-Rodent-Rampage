using Godot;
using System;

public partial class TowerSelectMenu : Control
{
	[Signal] public delegate void TowerSelectionCancelledEventHandler();
	private void OnTowerSelected(string towerName)
	{
		switch (towerName)
		{
		case "tower1":
			GD.Print("tower 1");
			break;
		case "tower2":
			GD.Print("tower 2");
			break;
		default:
			break;
		}
	}

	private void OnCancelPressed()
	{
		EmitSignal(SignalName.TowerSelectionCancelled);
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		
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
