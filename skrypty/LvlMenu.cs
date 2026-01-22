using Godot;
using System;
using System.Collections.Generic;
using System.Text.Json;

public partial class LvlMenu : Node2D
{
	[Export] private Button powrot_do_menu;
	[Export] private Button kolejna_strona_lv;
	[Export] private GridContainer gridContainer;
	
	private readonly string saveFilePath = "user://game_save.json";
	
	private class GameSaveData
	{
		public int HighestUnlockedLevel { get; set; } = 1;
		public List<int> CompletedLevels { get; set; } = new List<int>();
		public int TotalScore { get; set; } = 0;
	}
	
	private GameSaveData saveData = new GameSaveData();
	private List<Button> levelButtons = new List<Button>();
	
	public override void _Ready()
	{
		GD.Print("=== ŁADOWANIE MENU POZIOMÓW ===");
		
		LoadGameSave();
		
		if (powrot_do_menu != null)
		{
			powrot_do_menu.Pressed += OnBackToMenuPressed;
		}
		
		if (kolejna_strona_lv != null)
		{
			kolejna_strona_lv.Pressed += OnNextPagePressed;
		}
		
		if (gridContainer == null)
		{
			gridContainer = GetNodeOrNull<GridContainer>("MarginContainer/GridContainer");
			if (gridContainer == null)
			{
				gridContainer = GetNodeOrNull<GridContainer>("GridContainer");
			}
		}
		
		if (gridContainer != null)
		{
			SetupLevelButtons();
		}
		else
		{
			GD.PrintErr("Nie znaleziono GridContainer!");
			CreateTestButtons();
		}
		
		GD.Print($"Najwyższy odblokowany poziom: {saveData.HighestUnlockedLevel}");
		GD.Print($"Ukończone poziomy: {string.Join(", ", saveData.CompletedLevels)}");
	}
	
	public override void _EnterTree()
	{
		base._EnterTree();
		GD.Print("=== LvlMenu: _EnterTree() ===");
		
		LoadGameSave();
		
		if (gridContainer != null)
		{
			SetupLevelButtons();
		}
	}
	
	private void LoadGameSave()
	{
		GD.Print("Ładowanie zapisu gry z menu...");
		
		using var file = FileAccess.Open(saveFilePath, FileAccess.ModeFlags.Read);
		if (file != null)
		{
			try
			{
				string json = file.GetAsText();
				saveData = JsonSerializer.Deserialize<GameSaveData>(json);
				GD.Print($"Odczytano zapis: odblokowany poziom {saveData.HighestUnlockedLevel}, wynik: {saveData.TotalScore}");
			}
			catch (Exception e)
			{
				GD.PrintErr($"Błąd odczytu zapisu: {e.Message}");
				saveData = new GameSaveData();
			}
		}
		else
		{
			GD.Print("Brak zapisu gry, ustawiam domyślny poziom 1");
			saveData = new GameSaveData();
		}
	}
	
	private void SetupLevelButtons()
	{
		GD.Print($"Ustawiam {gridContainer.GetChildCount()} przycisków poziomów");
		
		levelButtons.Clear();
		
		foreach (Node child in gridContainer.GetChildren())
		{
			if (child is Button button)
			{
				levelButtons.Add(button);
			}
		}
		
		levelButtons.Sort((a, b) => 
		{
			int aNum = ExtractLevelNumber(a.Name);
			int bNum = ExtractLevelNumber(b.Name);
			return aNum.CompareTo(bNum);
		});
		
		for (int i = 0; i < levelButtons.Count; i++)
		{
			int levelNumber = i + 1;
			Button button = levelButtons[i];
			
			button.Text = $"Poziom {levelNumber}";
			
			if (levelNumber <= saveData.HighestUnlockedLevel)
			{
				button.Disabled = false;
				button.Modulate = new Color(1, 1, 1, 1);
				
				bool isCompleted = saveData.CompletedLevels.Contains(levelNumber);
				
				if (isCompleted)
				{
					button.Text += "\n✓";
					button.TooltipText = $"Poziom {levelNumber} - Ukończony!\nKliknij, aby zagrać ponownie";
					
					var styleBox = new StyleBoxFlat();
					styleBox.BgColor = new Color(0.2f, 0.8f, 0.2f);
					styleBox.CornerRadiusTopLeft = 10;
					styleBox.CornerRadiusTopRight = 10;
					styleBox.CornerRadiusBottomLeft = 10;
					styleBox.CornerRadiusBottomRight = 10;
					button.AddThemeStyleboxOverride("normal", styleBox);
				}
				else
				{
					button.TooltipText = $"Poziom {levelNumber} - Odblokowany\nKliknij, aby zagrać";
					
					var styleBox = new StyleBoxFlat();
					styleBox.BgColor = new Color(0.2f, 0.5f, 0.8f);
					styleBox.CornerRadiusTopLeft = 10;
					styleBox.CornerRadiusTopRight = 10;
					styleBox.CornerRadiusBottomLeft = 10;
					styleBox.CornerRadiusBottomRight = 10;
					button.AddThemeStyleboxOverride("normal", styleBox);
				}
			}
			else
			{
				button.Disabled = true;
				button.Modulate = new Color(0.7f, 0.7f, 0.7f, 0.8f); // Szary
				button.Text += "\n🔒";
				
				var styleBox = new StyleBoxFlat();
				styleBox.BgColor = new Color(0.5f, 0.5f, 0.5f); // Szary
				styleBox.CornerRadiusTopLeft = 10;
				styleBox.CornerRadiusTopRight = 10;
				styleBox.CornerRadiusBottomLeft = 10;
				styleBox.CornerRadiusBottomRight = 10;
				button.AddThemeStyleboxOverride("normal", styleBox);
				button.AddThemeStyleboxOverride("disabled", styleBox);
				
				button.TooltipText = $"Poziom {levelNumber} - Zablokowany\nUkończ poziom {saveData.HighestUnlockedLevel}, aby odblokować";
			}
			
			int levelToLoad = levelNumber;
			button.Pressed += () => {
				GD.Print($"Kliknięto przycisk poziomu {levelToLoad}");
				StartLevel(levelToLoad);
			};
			
			GD.Print($"Przycisk poziomu {levelNumber}: {(levelNumber <= saveData.HighestUnlockedLevel ? "ODBLOKOWANY" : "ZABLOKOWANY")}");
		}
	}
	
	private int ExtractLevelNumber(string nodeName)
	{
		if (string.IsNullOrEmpty(nodeName))
			return 0;
		
		string numStr = nodeName.Replace("lvl", "");
		
		if (int.TryParse(numStr, out int result))
		{
			return result;
		}
		
		foreach (char c in nodeName)
		{
			if (char.IsDigit(c))
			{
				string digitStr = c.ToString();
				if (int.TryParse(digitStr, out int digit))
				{
					return digit;
				}
			}
		}
		
		return 0;
	}
	
	private void StartLevel(int levelNumber)
	{
		GD.Print($"======= ROZPOCZYNAM POZIOM {levelNumber} =======");
		GD.Print($"Odblokowane poziomy do: {saveData.HighestUnlockedLevel}");
		
		if (levelNumber > saveData.HighestUnlockedLevel)
		{
			GD.PrintErr($"Poziom {levelNumber} nie jest odblokowany!");
			ShowLevelLockedMessage(levelNumber);
			return;
		}
		
		GameManager.LevelToLoad = levelNumber;
		GD.Print($"USTAWIONO: GameManager.LevelToLoad = {GameManager.LevelToLoad}");

		string scenePath = "res://sceny/levele/LevelScene.tscn";
		GD.Print($"Przechodzę do: {scenePath}");
		GetTree().ChangeSceneToFile(scenePath);
	}
	
	private void ShowLevelLockedMessage(int levelNumber)
	{
		var viewportRect = GetViewport().GetVisibleRect();
		var viewportSize = viewportRect.Size;
		
		var messagePanel = new PanelContainer();
		messagePanel.Name = "LevelLockedMessage";
		messagePanel.CustomMinimumSize = new Vector2(400, 200);
		messagePanel.Position = new Vector2(
			viewportRect.Position.X + viewportSize.X / 2 - 200,
			viewportRect.Position.Y + viewportSize.Y / 2 - 100
		);
		messagePanel.Visible = true;
		
		var vbox = new VBoxContainer();
		vbox.CustomMinimumSize = new Vector2(400, 200);
		vbox.Alignment = BoxContainer.AlignmentMode.Center;
		messagePanel.AddChild(vbox);
		
		var label = new Label();
		label.Text = $"Poziom {levelNumber} jest zablokowany!\n\nNajpierw ukończ poziom {saveData.HighestUnlockedLevel}.";
		label.HorizontalAlignment = HorizontalAlignment.Center;
		label.AutowrapMode = TextServer.AutowrapMode.Word;
		label.CustomMinimumSize = new Vector2(380, 100);
		vbox.AddChild(label);
		
		var okButton = new Button();
		okButton.Text = "OK";
		okButton.CustomMinimumSize = new Vector2(100, 40);
		okButton.Pressed += () => messagePanel.QueueFree();
		vbox.AddChild(okButton);
		
		AddChild(messagePanel);
	}
	
	private void CreateTestButtons()
	{
		GD.Print("Tworzę testowe przyciski...");
		
		foreach (Node child in GetChildren())
		{
			if (child is Button && child.Name != "powrot_do_menu" && child.Name != "kolejna_strona_lv")
			{
				child.QueueFree();
			}
		}
		
		var testGrid = new GridContainer();
		testGrid.Columns = 5;
		testGrid.CustomMinimumSize = new Vector2(800, 400);
		testGrid.Position = new Vector2(100, 100);
		AddChild(testGrid);
		
		for (int i = 1; i <= 15; i++)
		{
			var button = new Button();
			button.Text = $"Poziom {i}";
			button.CustomMinimumSize = new Vector2(150, 60);
			
			if (i <= saveData.HighestUnlockedLevel)
			{
				button.Disabled = false;
				button.Modulate = new Color(1, 1, 1, 1);
				button.TooltipText = $"Kliknij, aby zagrać w poziom {i}";
				
				int level = i;
				button.Pressed += () => StartLevel(level);
			}
			else
			{
				button.Disabled = true;
				button.Modulate = new Color(0.5f, 0.5f, 0.5f, 0.7f);
				button.Text += "\n🔒";
				button.TooltipText = $"Poziom {i} jest zablokowany!";
			}
			
			testGrid.AddChild(button);
		}
	}
	
	private void OnBackToMenuPressed()
	{
		GD.Print("Powrót do menu głównego");
		GetTree().ChangeSceneToFile("res://sceny/menu.tscn");
	}
	
	private void OnNextPagePressed()
	{
		GD.Print("Przejście do następnej strony poziomów (jeśli są)");
	}

	public void RefreshLevelButtons()
	{
		LoadGameSave();
		SetupLevelButtons();
		GD.Print("Odświeżono przyciski poziomów");
	}
}
