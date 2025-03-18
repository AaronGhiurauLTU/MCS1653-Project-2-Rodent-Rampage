using Godot;
using System;

public partial class MusicManager : AudioStreamPlayer
{
	static public MusicManager instance;
	[Export] private AudioStream backgroundMusic, victoryMusic;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		instance = this;
	}

	public static void PlayBackgroundMusic()
	{
		instance.Stream = instance.backgroundMusic;
		instance.Play();
	}

	public static void PlayVictoryMusic()
	{
		instance.Stream = instance.victoryMusic;
		instance.Play();
	}
}
