using Godot;
using System;

public partial class skr_f : TextureButton
{
	[Export]
	public int NumerLevelu = 6;

	public override void _Ready()
	{
		Pressed += PoKliknieciu;
		AddToGroup("LevelButtons");
	}

	void PoKliknieciu()
	{
		string sciezka = $"res://Level{NumerLevelu}.tscn";
		GetTree().ChangeSceneToFile("res://sceny/levele/LevelScene.tscn");
	}
}
