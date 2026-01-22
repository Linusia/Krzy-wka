using Godot;
using System;

public partial class startg : TextureButton
{
	public override void _Pressed()
	{
		GetTree().ChangeSceneToFile("res://sceny/menu.tscn");
	}
}
