using Godot;

public partial class LetterButton : Button
{
	public delegate void LetterClickHandler(LetterButton button);
	public event LetterClickHandler LetterClicked;
	
	private string letter;
	
	public override void _Ready()
	{
		Pressed += OnPressed;
	}
	
	public void Initialize(string letterText, Color color)
	{
		letter = letterText.ToUpper();
		Text = letter;
		
		var style = new StyleBoxFlat();
		style.BgColor = color;
		style.CornerRadiusBottomLeft = 10;
		style.CornerRadiusBottomRight = 10;
		style.CornerRadiusTopLeft = 10;
		style.CornerRadiusTopRight = 10;
		AddThemeStyleboxOverride("normal", style);
	}
	
	private void OnPressed()
	{
		LetterClicked?.Invoke(this);
	}
	
	public void SetColor(Color color)
	{
		var style = GetThemeStylebox("normal") as StyleBoxFlat;
		if (style != null)
		{
			style.BgColor = color;
		}
	}
	
	public string GetLetter() => letter;
}
