using Godot;
using System;

public partial class Enemy : CharacterBody2D
{
	[Export] private float speed = 300.0f;
	[Export] private int maxHealth = 3;
	[Export] private AnimatedSprite2D animatedSprite;
	[Export] private TextureProgressBar healthBar;
	[Export] private int cashDropped = 1;
	public EnemySpawner enemySpawner;
	private int currentNodeIndex = 0;
	private float currentHealth;
	private GameManager gameManager;
	public bool isBoss;
	public override void _Ready()
	{
		currentHealth = maxHealth;
		healthBar.Visible = false;
		gameManager = enemySpawner.GetNode<GameManager>("%GameManager");
		animatedSprite.FlipH = true;
		healthBar.Size = new Vector2(66 * (maxHealth / 3), healthBar.Size.Y);
		healthBar.Position = new Vector2(-1 * healthBar.Size.X / 2, healthBar.Position.Y);
	}

	public void TakeDamage(int damageAmount)
	{
		if (currentHealth <= 0)
			return;

		currentHealth = Math.Max(0, currentHealth - damageAmount);

		if (currentHealth < maxHealth)
		{
			healthBar.Visible = true;
			healthBar.Value = currentHealth / maxHealth;	
		}

		// handle enemy death
		if (currentHealth == 0)
		{
			gameManager.UpdateCash(cashDropped);
			enemySpawner.enemiesRemaining--;

			if (enemySpawner.enemiesRemaining == 0)
			{
				gameManager.gameWonMenu.Visible = true;
				Engine.TimeScale = 0.0;
				MusicManager.PlaySong(MusicManager.Song.Victory);
			}
			else if (isBoss)
			{
				enemySpawner.bossesRemaining--;
				
				if (enemySpawner.bossesRemaining <= 0)
				{
					MusicManager.PlaySong(MusicManager.Song.Background);
				}
			}

			QueueFree();
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		if (Engine.TimeScale == 0)
			return;

		if ((Position - enemySpawner.pathfindingNodes[currentNodeIndex]).Length() < 5)
		{
			if (currentNodeIndex < enemySpawner.nodeCount - 1)
			{
				currentNodeIndex++;
			}
			else // reached the end of the path
			{
				var cheese = enemySpawner.GetNode<AnimatedSprite2D>("%Cheese");
				cheese?.QueueFree();
				Engine.TimeScale = 0.0;
				
				gameManager.gameOverMenu.Visible = true;
			}
		}
		
		Vector2 velocity = (enemySpawner.pathfindingNodes[currentNodeIndex] - Position).Normalized() * speed;
		
		// rotate with path
		animatedSprite.Rotation = velocity.Angle();

		Velocity = velocity;
		MoveAndSlide();
	}

	// calculate the distance till the end of the path
	public float GetDistanceTillEnd()
	{
		Vector2 currentPosition = Position;
		float totalDistance = 0;
		
		for (int i = currentNodeIndex; i < enemySpawner.nodeCount; i++)
		{
			totalDistance += (currentPosition - enemySpawner.pathfindingNodes[i]).Length();
			currentPosition = enemySpawner.pathfindingNodes[i];
		}

		return totalDistance;
	}
}
