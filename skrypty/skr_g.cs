using Godot;
using System;

public partial class skr_g : TextureButton
{
	[Export]
	public int NumerLevelu = 47;

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
