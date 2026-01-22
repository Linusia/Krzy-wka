using Godot;

public partial class LosePanel : CanvasLayer
{
	[Signal]
	public delegate void RetryLevelEventHandler();
	[Signal]
	public delegate void GoToMenuEventHandler();
	
	[Export] private TextureButton buttonRetry;
	[Export] private TextureButton buttonMenu;
	[Export] private Label scoreLabel;
	
	private int _score = 0;
	
	public override void _Ready()
	{
		GD.Print("=== LosePanel._Ready() ===");
		
		if (buttonRetry == null)
		{
			buttonRetry = GetNodeOrNull<TextureButton>("TextureRect/ButtonRetry");
		}
		
		if (buttonMenu == null)
		{
			buttonMenu = GetNodeOrNull<TextureButton>("TextureRect/ButtonMenu");
		}
		
		if (scoreLabel == null)
		{
			scoreLabel = GetNodeOrNull<Label>("TextureRect/ScoreLabel");
		}
		
		GD.Print($"ButtonRetry: {(buttonRetry != null ? "Znaleziono" : "NULL")}");
		GD.Print($"ButtonMenu: {(buttonMenu != null ? "Znaleziono" : "NULL")}");
		GD.Print($"ScoreLabel: {(scoreLabel != null ? "Znaleziono" : "NULL")}");
		
		if (buttonRetry != null)
		{
			buttonRetry.Pressed += OnRetryButtonPressed;
			GD.Print("Podłączono buttonRetry.Pressed");
		}
		
		UpdateScoreLabel();
		
		GD.Print("LosePanel gotowy");
	}
	
	private void OnRetryButtonPressed()
	{
		GD.Print("=== OnRetryButtonPressed() ===");
		GD.Print("Emituję sygnał RetryLevel");
		EmitSignal(SignalName.RetryLevel);
		
		QueueFree();
	}
	
	private void UpdateScoreLabel()
	{
		if (scoreLabel != null)
		{
			scoreLabel.Text = $"Wynik: {_score}";
		}
	}
	
	public void SetScore(int score)
	{
		_score = score;
		UpdateScoreLabel();
	}
	
	public void ShowWithAnimation()
	{
		var mainContainer = GetNodeOrNull<Control>("TextureRect");
		
		if (mainContainer != null)
		{
			mainContainer.Modulate = new Color(1, 1, 1, 0);
			
			var tween = CreateTween();
			tween.TweenProperty(mainContainer, "modulate", new Color(1, 1, 1, 1), 0.5f);
		}
	}
}
