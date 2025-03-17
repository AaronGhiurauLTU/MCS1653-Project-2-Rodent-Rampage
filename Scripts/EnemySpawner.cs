using Godot;
using System;
using System.Collections.Generic;

public partial class EnemySpawner : Node2D
{
	public Vector2[] pathfindingNodes;
	public int nodeCount;
	[Export] private string mouseScenePath = "res://Scenes/Mouse.tscn";
	[Export] private string ratScenePath = "res://Scenes/Rat.tscn";
	private PackedScene mouseScene, ratScene, nextEnemy;
	[Export] private Timer nextEnemyTimer;

	private bool lastEnemySpawned = false;
	public bool LastEnemySpawned { get { return lastEnemySpawned; } }
	private Queue<Tuple<float, string>> waves = new Queue<Tuple<float, string>>(new Tuple<float, string>[] {
		new(1, "mouse"),
		new(1, "mouse"),
		new(1, "mouse"),
		new(1, "mouse"),
		new(1, "mouse"),
		new(1, "mouse"),
		new(1, "mouse"),
		new(1, "mouse"),

		new(5, "mouse"),
		new(1, "mouse"),
		new(1, "mouse"),
		new(0.5f, "mouse"),
		new(0.5f, "mouse"),
		new(0.5f, "mouse"),
		new(0.5f, "mouse"),
		new(0.5f, "mouse"),
		new(0.5f, "mouse"),
		new(0.5f, "mouse"),
		new(0.5f, "rat"),
	});

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

		mouseScene = GD.Load<PackedScene>(mouseScenePath);
		ratScene = GD.Load<PackedScene>(ratScenePath);

		GetNextEnemy();
	}
	private void GetNextEnemy()
	{
		if (waves.Count == 0)
		{
			nextEnemy = null;
			nextEnemyTimer.Stop();
			lastEnemySpawned = true;
			return;
		}

		Tuple<float, string> info = waves.Dequeue();
		nextEnemyTimer.Start(info.Item1);
		
		switch (info.Item2)
		{
			case "mouse":
				nextEnemy = mouseScene;
				break;
			case "rat":
				nextEnemy = ratScene;
				break;
		}
	}
	private void OnNextEnemyTimeout()
	{
		if (nextEnemy == null)
			return;

		Enemy instance = (Enemy)nextEnemy.Instantiate();
		instance.enemySpawner = this;
		AddChild(instance);

		GetNextEnemy();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
