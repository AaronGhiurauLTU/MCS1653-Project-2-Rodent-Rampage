using Godot;
using System;
using System.Collections.Generic;

public partial class EnemySpawner : Node2D
{
	public Vector2[] pathfindingNodes;
	public int nodeCount;
	[Export] private string mouseScenePath = "res://Scenes/Mouse.tscn";
	[Export] private string ratScenePath = "res://Scenes/Rat.tscn";
	[Export] private GameManager gameManager;
	private PackedScene mouseScene, ratScene;
	[Export] private Timer nextEnemyTimer;

	private bool lastEnemySpawned = false;
	public int bossesRemaining = 0;
	public int enemiesRemaining;
	public bool LastEnemySpawned { get { return lastEnemySpawned; } }

	class EnemyInformation
	{
		public readonly float delay;
		public readonly string name;
		public readonly bool isBoss;

		public EnemyInformation(float delay, string name, bool isBoss = false)
		{
			this.delay = delay;
			this.name = name;
			this.isBoss = isBoss;
		}
	}

	private EnemyInformation nextEnemy;
	private Queue<EnemyInformation> waves = new Queue<EnemyInformation>(new EnemyInformation[] {
		new(5, "mouse"),
		new(1.5f, "mouse"),
		new(1.5f, "mouse"),
		new(1.5f, "mouse"),
		new(1.5f, "mouse"),
		new(1.5f, "mouse"),
		new(1.5f, "mouse"),
		new(1.5f, "mouse"),

		new(8, "mouse"),
		new(1, "mouse"),
		new(1, "mouse"),
		new(1, "mouse"),
		new(1, "mouse"),
		new(1, "mouse"),
		new(1, "mouse"),
		new(1, "mouse"),
		new(1, "mouse"),
		new(1, "mouse"),
		new(1, "mouse"),
		new(1, "mouse"),
		new(1, "mouse"),
		new(1, "mouse"),

		new(8, "mouse"),
		new(1, "mouse"),
		new(1, "mouse"),
		new(0.5f, "mouse", true),
		new(0.5f, "mouse", true),
		new(0.5f, "mouse", true),
		new(0.5f, "mouse", true),
		new(0.5f, "mouse", true),
		new(0.5f, "mouse", true),
		new(0.5f, "mouse", true),
		new(0.5f, "rat", true),

		new(8, "mouse"),
		new(0.5f, "mouse"),
		new(0.5f, "mouse"),
		new(0.5f, "mouse"),
		new(0.5f, "mouse"),
		new(0.5f, "mouse"),
		new(0.5f, "mouse"),
		new(0.5f, "mouse"),
		new(0.5f, "mouse"),
		new(0.5f, "mouse"),
		new(0.5f, "mouse"),
		new(0.5f, "mouse"),
		new(0.5f, "mouse"),
		new(0.5f, "mouse"),
		new(0.5f, "mouse"),
		new(2f, "rat"),
		new(2f, "rat"),
		new(2f, "mouse"),
		new(0.5f, "mouse"),
		new(0.5f, "mouse"),
		new(0.5f, "mouse"),
		new(2f, "rat"),
		new(2f, "rat"),
		new(2f, "mouse"),
		new(0.5f, "mouse"),
		new(0.5f, "mouse"),
		new(0.5f, "mouse"),
		new(0.5f, "mouse"),
		new(0.5f, "mouse"),
		new(0.5f, "mouse"),
		new(0.5f, "mouse"),
		new(0.5f, "rat", true),
		new(0, "mouse", true),
		new(0.5f, "rat", true),
		new(0, "mouse", true),
		new(0.5f, "rat", true),
		new(0, "mouse", true),
		new(0.5f, "rat", true),
		new(0, "mouse", true),
		new(0.5f, "rat", true),
		new(0, "mouse", true),
		new(0.5f, "rat", true),
		new(0, "mouse", true),
		new(0.5f, "rat", true),
		new(0, "mouse", true),
		new(0.5f, "rat", true),
		new(0, "mouse", true),
		new(0.5f, "rat", true),
		new(0, "mouse", true),
		new(0.5f, "rat", true),
		new(0, "mouse", true),
		new(0.5f, "rat", true),
		new(0, "mouse", true),
		new(0.5f, "rat", true),
		new(0, "mouse", true),
		new(0.5f, "rat", true),
		new(0, "mouse", true),
		new(0.5f, "rat", true),
		new(0, "mouse", true),
		new(0.5f, "rat", true),
		new(0, "mouse", true),
		new(0.5f, "rat", true),
		new(0, "mouse", true),
		new(0.5f, "rat", true),
		new(0, "mouse", true),
		new(0.5f, "rat", true),
		new(0, "mouse", true),
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

		enemiesRemaining = waves.Count;
		gameManager.enemyCountLabel.Text = "Rodents Remaining: " + enemiesRemaining;
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

		EnemyInformation info = waves.Dequeue();
		nextEnemyTimer.Start(info.delay);
		
		if (info.isBoss)
		{
			bossesRemaining++;
		}

		nextEnemy = info;
	}
	private void OnNextEnemyTimeout()
	{
		if (nextEnemy == null)
			return;

		PackedScene scene = null;
		switch (nextEnemy.name)
		{
			case "mouse":
				scene = mouseScene;
				break;
			case "rat":
				scene = ratScene;
				break;
		}
		Enemy instance = (Enemy)scene.Instantiate();
		instance.enemySpawner = this;

		if (nextEnemy.isBoss)
		{
			instance.isBoss = true;
			MusicManager.PlaySong(MusicManager.Song.Boss);
		}

		AddChild(instance);

		GetNextEnemy();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
