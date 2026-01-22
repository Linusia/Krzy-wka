using Godot;

public partial class WinPanel : CanvasLayer
{
	[Signal]
	public delegate void NextLevelEventHandler();
	[Signal]
	public delegate void GoToMenuEventHandler();
	
	private ButtonNext buttonNext;
	private ButtonMenu buttonMenu;
	private Label scoreLabel;
	
	private int _score = 0;
	public int Score 
	{ 
		get => _score;
		set
		{
			_score = value;
			UpdateScoreLabel();
		}
	}
	
	public override void _Ready()
	{
		GD.Print("=== WinPanel._Ready() ===");
		
		var textureRect = GetNode<TextureRect>("TextureRect");
		
		buttonNext = textureRect.GetNode<ButtonNext>("ButtonNext");
		buttonMenu = textureRect.GetNode<ButtonMenu>("ButtonMenu");
		scoreLabel = textureRect.GetNode<Label>("ScoreLabel");
		
		GD.Print($"WinPanel - ButtonNext: {buttonNext?.Name ?? "NULL"}");
		GD.Print($"WinPanel - ButtonMenu: {buttonMenu?.Name ?? "NULL"}");
		GD.Print($"WinPanel - ScoreLabel: {scoreLabel?.Name ?? "NULL"}");
		
		if (buttonNext != null)
		{
			buttonNext.NextLevelPressed += OnNextLevel;
			GD.Print("Podłączono event NextLevelPressed z ButtonNext");
		}
		else
		{
			GD.PrintErr("WinPanel: Nie znaleziono ButtonNext!");
		}
		
		if (buttonMenu != null)
		{
			GD.Print("ButtonMenu znaleziony i gotowy do użycia");
		}
		else
		{
			GD.PrintErr("WinPanel: Nie znaleziono ButtonMenu!");
		}
		
		UpdateScoreLabel();
	}
	
	private void OnNextLevel()
	{
		GD.Print("=== WinPanel.OnNextLevel() WYWOŁANY ===");
		GD.Print("Emituję sygnał NextLevel");
		EmitSignal(SignalName.NextLevel);
		QueueFree();
	}
	
	private void UpdateScoreLabel()
	{
		if (scoreLabel != null && IsInsideTree())
		{
			scoreLabel.Text = $"Wynik: {_score}";
			GD.Print($"WinPanel: Ustawiono wynik na {_score}");
		}
		else if (scoreLabel == null)
		{
			GD.PrintErr("WinPanel: ScoreLabel jest null!");
		}
	}
	
	public void SetScore(int score)
	{
		Score = score;
	}
	
	public void ShowWithAnimation()
	{
		GD.Print("WinPanel.ShowWithAnimation() wywołany");
		
		var mainContainer = GetNodeOrNull<Control>("TextureRect");
		
		if (mainContainer != null)
		{
			GD.Print("Animuję TextureRect w WinPanel");
			mainContainer.Modulate = new Color(1, 1, 1, 0);
			
			var tween = CreateTween();
			tween.TweenProperty(mainContainer, "modulate", new Color(1, 1, 1, 1), 0.5f);
		}
		else
		{
			GD.PrintErr("WinPanel: Nie znaleziono głównego kontenera do animacji!");
		}
	}
}
