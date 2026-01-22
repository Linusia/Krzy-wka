using Godot;
using System;

public partial class PowrotDoMenu : TextureButton
{
	public override void _Pressed()
	{
		GetTree().ChangeSceneToFile("res://sceny/menu.tscn");
	}
}
