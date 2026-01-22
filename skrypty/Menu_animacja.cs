using Godot;
using System;

public partial class Menu_animacja : TextureRect
{
	private float _czas = 0f;
	private Vector2 _startowaPozycja;
	private float _silaAnimacji = 3f;
	private float _szybkosc = 2f;
	
	public override void _Ready()
	{
		_startowaPozycja = Position;
	}
	
	public override void _Process(double delta)
	{
		_czas += (float)delta;
		
		float ruchX = Mathf.Sin(_czas * _szybkosc) * _silaAnimacji;
		float ruchY = Mathf.Cos(_czas * _szybkosc * 0.7f) * _silaAnimacji;
		
		Position = _startowaPozycja + new Vector2(ruchX, ruchY);
		
	}
}
