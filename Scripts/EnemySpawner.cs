using Godot;
using System;

public partial class EnemySpawner : Node2D
{
	public Vector2[] pathfindingNodes;
	public int nodeCount;
	private string enemyScenePath = "res://Scenes/Mouse.tscn";
	PackedScene scene;
	[Export] private Timer nextEnemyTimer;
	public override void _Ready()
	{
		int i = 0;
		nodeCount = GetNode<Node2D>("PathfindingNodes").GetChildCount();
		pathfindingNodes = new Vector2[nodeCount];

		foreach (Node node in GetNode<Node2D>("PathfindingNodes").GetChildren())
		{
			pathfindingNodes[i] = ((Node2D)node).Position;
			i++;
		}
		nextEnemyTimer.Start();
		scene = GD.Load<PackedScene>(enemyScenePath);
	}

	private void OnNextEnemyTimeout()
	{
		Enemy instance = (Enemy)scene.Instantiate();
		instance.enemySpawner = this;
		AddChild(instance);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
