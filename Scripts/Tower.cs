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

	// gets overridden by derived classes
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
			/* unadjusted represents angles between -180 to 180 degrees and adjusted represents angles between 0 and 360 degrees
			 * what this code does is compares the adjusted versus unadjusted angles to select the pair of current and target rotations 
			 * that have the smallest differences so the tower take the shortest path of rotation */
			float unadjustedTargetRotation = Mathf.RadToDeg((firstEnemy.GetParent<Node2D>().ToGlobal(firstEnemy.Position) - Position).Angle());
			
			float adjustedTargetRotation = AdjustAngle(unadjustedTargetRotation);

			float unadjustedRotation = UnAdjustAngle(RotationDegrees);

			float adjustedRotation = AdjustAngle(RotationDegrees);

			float currentTargetAngle = 0,
				currentRotationAngle = 0, 
				minimumAngleDifference = float.PositiveInfinity;

			UpdateMinimumAngle(unadjustedTargetRotation, unadjustedRotation, ref minimumAngleDifference,
				ref currentTargetAngle, ref currentRotationAngle);
		
			UpdateMinimumAngle(adjustedTargetRotation, unadjustedRotation, ref minimumAngleDifference,
				ref currentTargetAngle, ref currentRotationAngle);
			
			UpdateMinimumAngle(unadjustedTargetRotation, adjustedRotation, ref minimumAngleDifference,
				ref currentTargetAngle, ref currentRotationAngle);
			
			UpdateMinimumAngle(adjustedTargetRotation, adjustedRotation, ref minimumAngleDifference,
				ref currentTargetAngle, ref currentRotationAngle);
			
			// rotate the tower by the rotation speed
			RotationDegrees = Mathf.MoveToward(currentRotationAngle, currentTargetAngle, rotationSpeed);

			// attack if the tower is facing the enemy enough and the attack is off cooldown
			if (Math.Abs(currentRotationAngle - currentTargetAngle) < 5 && canAttack)
			{
				canAttack = false;
				Attack();
				attackCooldownTimer.Start();
			}
		}
	}

	private void UpdateMinimumAngle(float targetAngle, float rotation, ref float minimumAngleDifference, 
		ref float currentTargetAngle, ref float currentRotationAngle)
	{
		float currentDifference = Math.Abs(targetAngle - rotation);

		if (currentDifference < minimumAngleDifference)
		{
			minimumAngleDifference = currentDifference;
			currentTargetAngle = targetAngle;
			currentRotationAngle = rotation;
		}
	}

	private float AdjustAngle(float angle)
	{
		if (angle < 0)
		{
			angle += 360;
		}

		return angle;
	}

	private float UnAdjustAngle(float angle)
	{
		if (angle > 180)
		{
			angle -= 360;
		}
		
		return angle;
	}

	private void OnAttackCooldownTimeout()
	{
		canAttack = true;
	}
}
