using Godot;
using System;

public partial class DamageArea : Area2D
{
	[Export] private int damage = 1;
	private void OnEnemyEntered(Node2D body)
	{
		if (body is Enemy enemy)
		{
			enemy.TakeDamage(damage);
		}
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
