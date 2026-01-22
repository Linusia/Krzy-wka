using Godot;
using System;

public partial class MenuLvlPrzycisk : TextureButton
{
	public override void _Pressed()
	{
		GetTree().ChangeSceneToFile("res://sceny/lvl_menu.tscn");
	}
}
