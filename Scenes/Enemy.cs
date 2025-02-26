using Godot;
using System;

public partial class Enemy : CharacterBody2D
{
	[Export] private float Speed = 300.0f;
	[Export] private AnimatedSprite2D animatedSprite;
	public EnemySpawner enemySpawner;
	private int currentNodeIndex = 0;
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
				GD.Print("reached end");
				QueueFree();
			}
		}
		
		Vector2 velocity = (enemySpawner.pathfindingNodes[currentNodeIndex] - Position).Normalized() * Speed;
		
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
}
