using Godot;
using System;
using System.Collections.Generic;
public partial class Tower : Node2D
{
	[Export] private float rotationSpeed = 0.04f;
	private HashSet<Enemy> enemiesInRange = new();
	protected Enemy firstEnemy = null;
	private void OnRangeEntered(Node2D body)
	{
		if (body is Enemy enemy)
		{
			enemiesInRange.Add(enemy);
		}
	}

	private void OnRangeExited(Node2D body)
	{
		if (body is Enemy enemy)
		{
			enemiesInRange.Remove(enemy);
			
			if (enemy == firstEnemy)
			{
				RecalculateFirstEnemy();
			}
		}
	}

	// update the first enemy in range
	private void RecalculateFirstEnemy()
	{
		float shortestDistanceToEnd = float.PositiveInfinity;
		Enemy currentFirstEnemy = null;

		foreach (Enemy enemy in enemiesInRange)
		{
			float currentEnemyDistanceToEnd = enemy.GetDistanceTillEnd();

			if (shortestDistanceToEnd > currentEnemyDistanceToEnd)
			{
				shortestDistanceToEnd = currentEnemyDistanceToEnd;
				currentFirstEnemy = enemy;
			}
		}

		firstEnemy = currentFirstEnemy;
	}

	protected virtual void Attack()
	{

	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(double delta)
	{
		RecalculateFirstEnemy();

		if (firstEnemy != null)
		{

			float targetAngle = Mathf.RadToDeg((firstEnemy.GetParent<Node2D>().ToGlobal(firstEnemy.Position) - Position).Angle());

			if (targetAngle < 0)
			{
				targetAngle = 360 + targetAngle;
			}

			RotationDegrees = Mathf.MoveToward(RotationDegrees < 0 ? 360 + RotationDegrees : RotationDegrees, targetAngle, rotationSpeed);
		}
	}
}
