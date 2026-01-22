using Godot;
using System;

public partial class kolejnaScena1 : TextureButton
{
	public override void _Pressed()
	{
		GetTree().ChangeSceneToFile("res://sceny/lvl_menu_s2.tscn");
	}
}
