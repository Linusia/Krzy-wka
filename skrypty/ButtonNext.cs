using Godot;

public partial class ButtonNext : TextureButton
{
	public delegate void NextLevelEventHandler();
	public event NextLevelEventHandler NextLevelPressed;
	
	public override void _Ready()
	{
		Pressed += OnPressed;
	}
	
	private void OnPressed()
	{
		GD.Print("ButtonNext: Następny poziom");
		NextLevelPressed?.Invoke();
	}
}
