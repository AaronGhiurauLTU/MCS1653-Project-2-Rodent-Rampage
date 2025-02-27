using Godot;
using System;
using System.Collections.Generic;
public partial class Tower : Node2D
{
	[Export] private bool rotateTowardsEnemies = true;
	[Export] private float rotationSpeed = 7f;
	[Export] private Timer attackCooldownTimer;
	private HashSet<Enemy> enemiesInRange = new();
	protected Enemy firstEnemy = null;
	protected bool canAttack = true;
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
			// can later optimize by skipping enemies that have more nodes remaining to avoid the need to sum the distances
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

		if (firstEnemy != null && rotateTowardsEnemies)
		{
			float unformattedAngle = Mathf.RadToDeg((firstEnemy.GetParent<Node2D>().ToGlobal(firstEnemy.Position) - Position).Angle());
			float targetAngle = unformattedAngle;

			// TODO: make it adjust the angle to whatever is closer so it doesn't rotation fully for something negative if the angle is currently positive and vice versa
			if (targetAngle < 0)
			{
				targetAngle = 360 + targetAngle;
			}

			RotationDegrees = Mathf.MoveToward(RotationDegrees < 0 ? 360 + RotationDegrees : RotationDegrees, targetAngle, rotationSpeed);

			if (Math.Abs(unformattedAngle - RotationDegrees) < 5 && canAttack)
			{
				canAttack = false;
				Attack();
				attackCooldownTimer.Start();
			}
		}
	}

	private void OnAttackCooldownTimeout()
	{
		canAttack = true;
	}
}
