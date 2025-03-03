using Godot;
using System;

public partial class Enemy : CharacterBody2D
{
	[Export] private float speed = 300.0f;
	[Export] private int maxHealth = 3;
	[Export] private AnimatedSprite2D animatedSprite;
	[Export] private TextureProgressBar healthBar;
	public EnemySpawner enemySpawner;
	private int currentNodeIndex = 0;
	private float currentHealth;

	public override void _Ready()
	{
		currentHealth = maxHealth;
		healthBar.Visible = false;
	}

	public void TakeDamage(int damageAmount)
	{
		currentHealth = Math.Max(0, currentHealth - damageAmount);

		if (currentHealth < maxHealth)
		{
			healthBar.Visible = true;
			healthBar.Value = currentHealth / maxHealth;	
		}

		// handle enemy death
		if (currentHealth == 0)
		{
			QueueFree();
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		if ((Position - enemySpawner.pathfindingNodes[currentNodeIndex]).Length() < 5)
		{
			if (currentNodeIndex < enemySpawner.nodeCount - 1)
			{
				currentNodeIndex++;
			}
			else
			{
				var cheese = enemySpawner.GetNode<AnimatedSprite2D>("%Cheese");
				cheese?.QueueFree();
				Engine.TimeScale = 0.0;
				
				enemySpawner.GetNode<GameManager>("%GameManager").gameOverMenu.Visible = true;

				//QueueFree();
			}
		}
		
		Vector2 velocity = (enemySpawner.pathfindingNodes[currentNodeIndex] - Position).Normalized() * speed;
		
		if (velocity.X > 0)
		{
			animatedSprite.FlipH = true;
		}
		else if (velocity.X < 0)
		{
			animatedSprite.FlipH = false;
		}

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
