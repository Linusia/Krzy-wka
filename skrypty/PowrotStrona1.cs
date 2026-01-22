using Godot;
using System;

public partial class PowrotStrona1 : TextureButton
{
	public override void _Pressed()
	{
		GetTree().ChangeSceneToFile("res://sceny/lvl_menu.tscn");
	}
}
