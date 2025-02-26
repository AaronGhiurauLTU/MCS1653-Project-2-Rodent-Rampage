using Godot;
using System;

public partial class TowerSelectMenu : Control
{
	[Signal] public delegate void TowerSelectionCancelledEventHandler();
	[Signal] public delegate void TowerPlacedEventHandler(string towerName);
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
