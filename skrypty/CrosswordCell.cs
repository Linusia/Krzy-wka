using Godot;

public partial class CrosswordCell : Button
{
	private Label letterLabel;
	
	public int WordIndex { get; set; } = -1;
	public int LetterIndex { get; set; } = -1;
	public string CorrectLetter { get; set; } = "";
	public string CurrentLetter { get; set; } = "";
	
	public delegate void CellClickHandler(CrosswordCell cell);
	public event CellClickHandler CellClicked;
	
	public override void _Ready()
	{
		letterLabel = GetNodeOrNull<Label>("LetterLabel");
		if (letterLabel == null)
		{
			letterLabel = new Label();
			letterLabel.HorizontalAlignment = HorizontalAlignment.Center;
			letterLabel.VerticalAlignment = VerticalAlignment.Center;
			AddChild(letterLabel);
		}
		
		Pressed += OnPressed;
		
		UpdateDisplay();
	}
	
	private void OnPressed()
	{
		CellClicked?.Invoke(this);
	}
	
	public void Initialize(int x, int y)
	{
		Name = $"Cell_{x}_{y}";
	}
	
	public void UpdateDisplay()
	{
		if (letterLabel != null)
		{
			letterLabel.Text = CurrentLetter;
		}
		
		var style = new StyleBoxFlat();
		
		if (WordIndex == -1)
		{
			style.BgColor = new Color(0.8f, 0.8f, 0.8f);
		}
		else if (!string.IsNullOrEmpty(CurrentLetter))
		{
			if (CurrentLetter == CorrectLetter)
			{
				style.BgColor = new Color(0.6f, 0.9f, 0.6f);
			}
			else
			{
				style.BgColor = new Color(0.9f, 0.6f, 0.6f);
			}
		}
		else
		{
			style.BgColor = new Color(1, 1, 1);
		}
		
		style.BorderColor = new Color(0.3f, 0.3f, 0.3f);
		style.BorderWidthLeft = 1;
		style.BorderWidthRight = 1;
		style.BorderWidthTop = 1;
		style.BorderWidthBottom = 1;
		
		AddThemeStyleboxOverride("normal", style);
	}
	
	public bool PlaceLetter(string letter)
	{
		if (WordIndex == -1) return false;
		
		CurrentLetter = letter.ToUpper();
		UpdateDisplay();
		return true;
	}
	
	public bool IsCorrect()
	{
		return CurrentLetter == CorrectLetter;
	}
	
	public void SetEmpty()
	{
		WordIndex = -1;
		CurrentLetter = "";
		UpdateDisplay();
	}
	
	public void SetCorrect()
	{
		var style = new StyleBoxFlat();
		style.BgColor = new Color(0.6f, 0.9f, 0.6f);
		style.BorderColor = new Color(0.3f, 0.3f, 0.3f);
		style.BorderWidthLeft = 2;
		style.BorderWidthRight = 2;
		style.BorderWidthTop = 2;
		style.BorderWidthBottom = 2;
		AddThemeStyleboxOverride("normal", style);
	}
	
	public void ClearLetter()
	{
		CurrentLetter = "";
		UpdateDisplay();
	}
	
	public void ShowWrongLetterTemporarily(string letter)
	{
		string originalLetter = CurrentLetter;
		
		string tempLetter = letter;
		
		var style = new StyleBoxFlat();
		style.BgColor = new Color(0.9f, 0.6f, 0.6f);
		style.BorderColor = new Color(0.3f, 0.3f, 0.3f);
		style.BorderWidthLeft = 1;
		style.BorderWidthRight = 1;
		style.BorderWidthTop = 1;
		style.BorderWidthBottom = 1;
		
		AddThemeStyleboxOverride("normal", style);
		
		if (letterLabel != null)
		{
			letterLabel.Text = tempLetter;
		}
		
		GetTree().CreateTimer(1.0).Timeout += () =>
		{
			if (letterLabel != null)
			{
				letterLabel.Text = originalLetter;
			}
			
			UpdateDisplay();
		};
	}
}
