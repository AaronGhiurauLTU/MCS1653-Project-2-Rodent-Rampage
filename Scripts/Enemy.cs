using Godot;
using System;

public partial class Enemy : CharacterBody2D
{
	[Export] private float speed = 300.0f;
	[Export] private int maxHealth = 3;
	[Export] private AnimatedSprite2D animatedSprite;
	[Export] private TextureProgressBar healthBar;
	[Export] private int cashDropped = 1;
	[Export] private AnimationPlayer animationPlayer;
	public EnemySpawner enemySpawner;
	private int currentNodeIndex = 0;
	public float currentHealth;
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

	public void Destroy(bool slain = true)
	{
		enemySpawner.enemiesRemaining--;

		if (enemySpawner.enemiesRemaining == 0)
		{
			gameManager.ExitSelectingTower();
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

		if (slain)
		{
			gameManager.UpdateCash(cashDropped);
			animatedSprite.RotationDegrees = 0;
			healthBar.QueueFree();
			animationPlayer.Play("death");
		}
		else
		{
			QueueFree();
		}
	}

	public void TakeDamage(int damageAmount)
	{
		if (currentHealth <= 0)
			return;

		animationPlayer.Play("damage");
		currentHealth = Math.Max(0, currentHealth - damageAmount);

		if (currentHealth < maxHealth)
		{
			healthBar.Visible = true;
			healthBar.Value = currentHealth / maxHealth;	
		}

		// handle enemy death
		if (currentHealth == 0)
		{
			Destroy();
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		if (Engine.TimeScale == 0 || currentHealth <= 0)
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

				gameManager.currentHealth--;
				gameManager.biteSound.Play();

				if (gameManager.currentHealth == 0)
				{
					cheese?.QueueFree();
					Engine.TimeScale = 0.0;
					
					gameManager.ExitSelectingTower();
					MusicManager.SetBusVolume("Music", -5);
					MusicManager.PlaySong(MusicManager.Song.Defeat);
					gameManager.gameOverMenu.Visible = true;
				}
				else
				{
					cheese.Animation = $"{gameManager.currentHealth}";
					Destroy(false);
				}
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
