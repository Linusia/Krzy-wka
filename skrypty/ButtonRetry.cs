using Godot;

public partial class ButtonRetry : TextureButton
{
	public delegate void RetryLevelEventHandler();
	public event RetryLevelEventHandler RetryLevelPressed;
	
	public override void _Ready()
	{
		Pressed += OnPressed;
	}
	
	private void OnPressed()
	{
		GD.Print("ButtonRetry: Spróbuj ponownie");
		RetryLevelPressed?.Invoke();
	}
}
