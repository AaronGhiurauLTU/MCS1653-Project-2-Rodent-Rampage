using Godot;
using System;

public partial class MusicManager : AudioStreamPlayer
{
	public enum Song
	{
		Boss,
		Background,
		Victory,
		Defeat
	}
	private static MusicManager instance;
	private static Song currentSong = Song.Background;
	[Export] private AudioStream backgroundMusic, bossMusic, victoryMusic, defeatMusic;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		instance = this;
	}

	public static void SetBusVolume(string bus, float volume)
	{
		int busIndex = AudioServer.GetBusIndex(bus);
		AudioServer.SetBusVolumeDb(busIndex, volume);
	}

	public static void PlaySong(Song song)
	{
		if (currentSong == song)
			return;

		switch (song)
		{
			case Song.Boss:
				instance.Stream = instance.bossMusic;
				break;
			case Song.Background:
				instance.Stream = instance.backgroundMusic;
				break;
			case Song.Victory:
				instance.Stream = instance.victoryMusic;
				break;
			case Song.Defeat:
				instance.Stream = instance.defeatMusic;
				break;
		}
		
		currentSong = song;
		instance.Play();
	}
}
