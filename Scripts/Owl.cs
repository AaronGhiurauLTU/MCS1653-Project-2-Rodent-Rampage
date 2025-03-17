using Godot;
using System;

public partial class Owl : Tower
{
	[Export] private AnimatedSprite2D owl;
	[Export] private DamageArea damageArea;
	[Export] private AnimationPlayer attackAnimationPlayer;
	[Export] private float movementSpeed = 40;

	protected override void Attack()
	{
		attackAnimationPlayer.Play("Attack");
	}
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		base._Ready();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		base._PhysicsProcess(delta);

		if (firstEnemy != null && canAttack)
		{
			// get the difference vector between the owl and the first enemy
			Vector2 difference = firstEnemy.GetParent<Node2D>().ToGlobal(firstEnemy.Position) - damageArea.GetParent<Node2D>().ToGlobal(damageArea.Position);

			// attack if the enemy is close enough
			if (difference.Length() < 10)
			{
				InitializeAttack();
			}
			else
			{
				// move towards the target
				owl.Position += difference.Normalized() * movementSpeed * (float)(delta * Engine.TimeScale);
			}
		}
	}
}
