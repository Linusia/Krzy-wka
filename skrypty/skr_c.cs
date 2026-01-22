using Godot;
using System;

public partial class skr_c : TextureButton
{
	[Export]
	public int NumerLevelu = 3;

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
