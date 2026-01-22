using Godot;
using System;

public partial class WyjścieZGryPrzycisk : TextureButton
{
	public override void _Ready()
	{
		Pressed += ZamknijGre;
	}

	void ZamknijGre()
	{
		GetTree().Quit();
	}
}
