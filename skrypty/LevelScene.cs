using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

public partial class LevelScene : Control
{
	[Export] public Label levelLabel;
	[Export] public Label movesLabel;
	[Export] public GridContainer scatteredLettersGrid;
	[Export] public GridContainer crosswordGrid;
	[Export] public VBoxContainer cluesContainer;
	[Export] public Button resetBtn;
	[Export] public Button checkBtn;
	[Export] public Button hintBtn;
	
	[Export] public PackedScene letterButtonPrefab;
	[Export] public PackedScene crosswordCellPrefab;
	[Export] public PackedScene winPanelPrefab;
	[Export] public PackedScene losePanelPrefab;
	
	private List<WordData> currentWords = new();
	private List<CrosswordCell> crosswordCells = new();
	private List<LetterButton> letterButtons = new();
	private List<ClueSlot> clueSlots = new();
	
	private int currentLevel = 1;
	private int movesLeft;
	private int totalMoves;
	private bool isGameActive = true;
	
	private LetterButton selectedLetter = null;
	private readonly Color normalLetterColor = new Color("#8c52ff");
	private readonly Color selectedLetterColor = new Color("#ff9900");
	
	private readonly string saveFilePath = "user://game_save.json";
	
	private class GameSaveData
	{
		public int HighestUnlockedLevel { get; set; } = 1;
	}
	
	private GameSaveData saveData = new GameSaveData();
	
	private readonly Dictionary<int, GameLevelData> levelsData = new()
	{
		{ 
			1, new GameLevelData { 
				MaxMoves = 10, 
				Words = new List<WordWithClue> { 
					new WordWithClue { Word = "DOM", Clue = "Miejsce gdzie mieszkasz" },
					new WordWithClue { Word = "KOT", Clue = "Mały futrzasty drapieżnik" },
					new WordWithClue { Word = "LAS", Clue = "Duży obszar z drzewami" }
				}
			} 
		},
		{ 
			2, new GameLevelData { 
				MaxMoves = 15, 
				Words = new List<WordWithClue> { 
					new WordWithClue { Word = "OKNO", Clue = "Przez nie widać na zewnątrz" },
					new WordWithClue { Word = "STÓŁ", Clue = "Mebel do jedzenia posiłków" },
					new WordWithClue { Word = "WODA", Clue = "Płyn niezbędny do życia" }
				}
			} 
		},
		{ 
			3, new GameLevelData { 
				MaxMoves = 18, 
				Words = new List<WordWithClue> { 
					new WordWithClue { Word = "AUTO", Clue = "Środek transportu z czterema kołami" },
					new WordWithClue { Word = "OGIEŃ", Clue = "Płomień dający ciepło i światło" },
					new WordWithClue { Word = "KWIAT", Clue = "Roślina ozdobna z płatkami" }
				}
			} 
		},
		{ 
			4, new GameLevelData { 
				MaxMoves = 26, 
				Words = new List<WordWithClue> { 
					new WordWithClue { Word = "KSIĄŻKA", Clue = "Zbiór kartek z drukowanym tekstem" },
					new WordWithClue { Word = "TELEWIZOR", Clue = "Urządzenie do oglądania programów" },
					new WordWithClue { Word = "KOMPUTER", Clue = "Maszyna do przetwarzania danych" }
				}
			} 
		},
		{ 
			5, new GameLevelData { 
				MaxMoves = 25, 
				Words = new List<WordWithClue> { 
					new WordWithClue { Word = "SZKOŁA", Clue = "Miejsce gdzie zdobywa się wiedzę" },
					new WordWithClue { Word = "NAUCZYCIEL", Clue = "Osoba która uczy uczniów" },
					new WordWithClue { Word = "ŁAWKA", Clue = "Mebel na którym siadają uczniowie" }
				}
			} 
		},
		{ 
			6, new GameLevelData { 
				MaxMoves = 27, 
				Words = new List<WordWithClue> { 
					new WordWithClue { Word = "HOSPITAL", Clue = "Miejsce gdzie leczy się chorych (po angielsku)" },
					new WordWithClue { Word = "DOCTOR", Clue = "Lekarz (po angielsku)" },
					new WordWithClue { Word = "PATIENT", Clue = "Pacjent (po angielsku)" }
				}
			} 
		},
		{ 
			7, new GameLevelData { 
				MaxMoves = 25, 
				Words = new List<WordWithClue> { 
					new WordWithClue { Word = "RIVER", Clue = "Rzeka (po angielsku)" },
					new WordWithClue { Word = "MOUNTAIN", Clue = "Góra (po angielsku)" },
					new WordWithClue { Word = "FOREST", Clue = "Las (po angielsku)" }
				}
			} 
		},
		{ 
			8, new GameLevelData { 
				MaxMoves = 20, 
				Words = new List<WordWithClue> { 
					new WordWithClue { Word = "ELEPHANT", Clue = "Słoń (po angielsku)" },
					new WordWithClue { Word = "LION", Clue = "Lew (po angielsku)" },
					new WordWithClue { Word = "TIGER", Clue = "Tygrys (po angielsku)" }
				}
			} 
		},
		{ 
			9, new GameLevelData { 
				MaxMoves = 29, 
				Words = new List<WordWithClue> { 
					new WordWithClue { Word = "CHOCOLATE", Clue = "Słodki przysmak z kakao" },
					new WordWithClue { Word = "BANANA", Clue = "Żółty owoc w kształcie księżyca" },
					new WordWithClue { Word = "STRAWBERRY", Clue = "Czerwony owoc z małymi pestkami" }
				}
			} 
		},
		{ 
			10, new GameLevelData { 
				MaxMoves = 30, 
				Words = new List<WordWithClue> { 
					new WordWithClue { Word = "BASKETBALL", Clue = "Sport z pomarańczową piłką i koszem" },
					new WordWithClue { Word = "FOOTBALL", Clue = "Najpopularniejszy sport na świecie" },
					new WordWithClue { Word = "TENNIS", Clue = "Sport z rakietami i siatką" }
			}
		} 
	},
		{ 
			11, new GameLevelData { 
				MaxMoves = 30, 
				Words = new List<WordWithClue> { 
					new WordWithClue { Word = "CHEMISTRY", Clue = "Nauka o substancjach i reakcjach" },
					new WordWithClue { Word = "BIOLOGY", Clue = "Nauka o żywych organizmach" },
					new WordWithClue { Word = "PHYSICS", Clue = "Nauka o energii i materii" }
				}
			} 
		},
		{ 
			12, new GameLevelData { 
				MaxMoves = 30, 
				Words = new List<WordWithClue> { 
					new WordWithClue { Word = "PYTHON", Clue = "Język programowania nazwany od węża" },
					new WordWithClue { Word = "JAVASCRIPT", Clue = "Język do tworzenia stron internetowych" },
					new WordWithClue { Word = "GODOT", Clue = "Silnik do tworzenia gier" }
				}
			} 
		},
		{ 
			13, new GameLevelData { 
				MaxMoves = 30, 
				Words = new List<WordWithClue> { 
					new WordWithClue { Word = "SATELLITE", Clue = "Urządzenie krążące wokół Ziemi" },
					new WordWithClue { Word = "ROCKET", Clue = "Pojazd kosmiczny z silnikiem" },
					new WordWithClue { Word = "ASTRONAUT", Clue = "Człowiek podróżujący w kosmos" }
				}
			} 
		},
		{ 
			14, new GameLevelData { 
				MaxMoves = 30, 
				Words = new List<WordWithClue> { 
					new WordWithClue { Word = "MICROSCOPE", Clue = "Urządzenie do oglądania małych obiektów" },
					new WordWithClue { Word = "TELESCOPE", Clue = "Urządzenie do obserwacji gwiazd" },
					new WordWithClue { Word = "THERMOMETER", Clue = "Przyrząd do mierzenia temperatury" }
				}
			} 
		},
		{ 
			15, new GameLevelData { 
				MaxMoves = 30, 
				Words = new List<WordWithClue> { 
					new WordWithClue { Word = "VICTORY", Clue = "Zwycięstwo po angielsku" },
					new WordWithClue { Word = "CHAMPION", Clue = "Mistrz po angielsku" },
					new WordWithClue { Word = "SUCCESS", Clue = "Sukces po angielsku" }
				}
			} 
		},
	};
	
	public override void _Ready()
	{
		GD.Print("=== ROZPOCZĘCIE GRY ===");
		
		LoadGameSave();
		
		resetBtn.Pressed += OnResetButtonPressed;
		checkBtn.Pressed += OnCheckButtonPressed;
		hintBtn.Pressed += OnHintButtonPressed;
		
		currentLevel = saveData.HighestUnlockedLevel;
		LoadLevel(currentLevel);
		
		GD.Print("=== GOTOWE ===");
	}

	private void LoadGameSave()
	{
		GD.Print("Ładowanie zapisu gry...");
		
		using var file = FileAccess.Open(saveFilePath, FileAccess.ModeFlags.Read);
		if (file != null)
		{
			try
			{
				string json = file.GetAsText();
				saveData = JsonSerializer.Deserialize<GameSaveData>(json);
				GD.Print($"Odczytano zapis: odblokowany poziom {saveData.HighestUnlockedLevel}");
			}
			catch (Exception e)
			{
				GD.PrintErr($"Błąd odczytu zapisu: {e.Message}");
				saveData = new GameSaveData();
				SaveGameSave();
			}
		}
		else
		{
			GD.Print("Brak zapisu gry, tworzenie nowego...");
			saveData = new GameSaveData();
			SaveGameSave();
		}
	}
	
	private void SaveGameSave()
	{
		try
		{
			string json = JsonSerializer.Serialize(saveData);
			using var file = FileAccess.Open(saveFilePath, FileAccess.ModeFlags.Write);
			file.StoreString(json);
			GD.Print($"Zapisano: odblokowany poziom {saveData.HighestUnlockedLevel}");
		}
		catch (Exception e)
		{
			GD.PrintErr($"Błąd zapisu: {e.Message}");
		}
	}
	
	private void UnlockNextLevel()
	{
		int nextLevel = currentLevel + 1;
		
		if (levelsData.ContainsKey(nextLevel))
		{
			if (nextLevel > saveData.HighestUnlockedLevel)
			{
				saveData.HighestUnlockedLevel = nextLevel;
				SaveGameSave();
				GD.Print($"Odblokowano poziom {nextLevel}! Najwyższy odblokowany: {saveData.HighestUnlockedLevel}");
			}
			else
			{
				GD.Print($"Poziom {nextLevel} był już wcześniej odblokowany");
			}
		}
		else
		{
			GD.Print($"Poziom {currentLevel} to ostatni dostępny poziom w grze");
		}
	}
	
	private void LoadLevel(int level)
	{
		GD.Print($"\n=== ŁADOWANIE POZIOMU {level} ===");
		currentLevel = level;
		
		if (!levelsData.ContainsKey(level))
		{
			GD.PrintErr($"Brak danych dla poziomu {level}");
			return;
		}
		
		var levelData = levelsData[level];
		movesLeft = levelData.MaxMoves;
		totalMoves = levelData.MaxMoves;
		isGameActive = true;
		
		ClearLevel();
		
		UpdateUI();
		
		InitializeWords(levelData.Words);
		
		CreateCrosswordGrid();
		
		CreateScatteredLetters();
		
		CreateClueSlots();
		
		GD.Print($"Poziom {level} załadowany.");
	}
	
	private void ClearLevel()
	{
		foreach (Node child in scatteredLettersGrid.GetChildren())
			child.QueueFree();
		letterButtons.Clear();
		
		foreach (Node child in crosswordGrid.GetChildren())
			child.QueueFree();
		crosswordCells.Clear();
		
		foreach (Node child in cluesContainer.GetChildren())
			child.QueueFree();
		clueSlots.Clear();
		
		currentWords.Clear();
		selectedLetter = null;
		
		foreach (Node child in GetChildren().ToArray())
		{
			if (child is LosePanel || child is WinPanel)
			{
				GD.Print($"Usuwam istniejący panel: {child.Name}");
				child.QueueFree();
			}
		}
	}
	
	private void UpdateUI()
	{
		levelLabel.Text = $"Poziom: {currentLevel}/15";
		movesLabel.Text = $"Ruchy: {movesLeft}/{totalMoves}";
		
		if (movesLeft <= 0 && isGameActive)
		{
			ShowLosePanel();
		}
	}
	
	private void InitializeWords(List<WordWithClue> wordsWithClues)
	{
		for (int i = 0; i < wordsWithClues.Count; i++)
		{
			currentWords.Add(new WordData
			{
				Index = i,
				Answer = wordsWithClues[i].Word.ToUpper(),
				Clue = wordsWithClues[i].Clue,
				IsComplete = false,
				PlacedLetters = new string('_', wordsWithClues[i].Word.Length)
			});
		}
	}
	
	private void CreateCrosswordGrid()
	{
		if (crosswordCellPrefab == null)
		{
			GD.PrintErr("BRAK crosswordCellPrefab! Tworzę fallback...");
		}
		
		int maxLength = currentWords.Max(w => w.Answer.Length);
		crosswordGrid.Columns = maxLength;
		
		int totalCells = currentWords.Count * maxLength;
		
		for (int i = 0; i < totalCells; i++)
		{
			CrosswordCell cell;
			
			if (crosswordCellPrefab != null)
			{
				cell = crosswordCellPrefab.Instantiate<CrosswordCell>();
			}
			else
			{
				cell = CreateFallbackCell();
			}
			
			crosswordGrid.AddChild(cell);
			crosswordCells.Add(cell);
			
			int row = i / maxLength;
			int col = i % maxLength;
			cell.Initialize(col, row);
			
			cell.CellClicked += OnCrosswordCellClicked;
			
			if (row < currentWords.Count && col < currentWords[row].Answer.Length)
			{
				cell.WordIndex = row;
				cell.LetterIndex = col;
				cell.CorrectLetter = currentWords[row].Answer[col].ToString();
			}
			else
			{
				cell.WordIndex = -1;
				cell.SetEmpty();
			}
			
			cell.UpdateDisplay();
		}
	}
	
	private void CreateScatteredLetters()
	{
		if (letterButtonPrefab == null)
		{
			GD.PrintErr("BRAK letterButtonPrefab! Tworzę fallback...");
		}
		
		string allLetters = "";
		foreach (var word in currentWords)
		{
			allLetters += word.Answer;
		}
		
		char[] letters = allLetters.ToCharArray();
		RandomizeArray(letters);
		
		foreach (char letter in letters)
		{
			LetterButton letterBtn;
			
			if (letterButtonPrefab != null)
			{
				letterBtn = letterButtonPrefab.Instantiate<LetterButton>();
			}
			else
			{
				letterBtn = CreateFallbackLetterButton();
			}
			
			scatteredLettersGrid.AddChild(letterBtn);
			
			letterBtn.Initialize(letter.ToString(), normalLetterColor);
			letterBtn.LetterClicked += OnLetterButtonClicked;
			letterButtons.Add(letterBtn);
		}
		
		scatteredLettersGrid.Columns = 6;
		
		GD.Print($"Stworzono {letterButtons.Count} przycisków z literami");
	}
	
	private void CreateClueSlots()
	{
		for (int i = 0; i < currentWords.Count; i++)
		{
			var word = currentWords[i];
			
			var slotContainer = new HBoxContainer();
			cluesContainer.AddChild(slotContainer);
			
			var numberLabel = new Label();
			numberLabel.Text = $"{i + 1}. ";
			numberLabel.CustomMinimumSize = new Vector2(30, 30);
			slotContainer.AddChild(numberLabel);
			
			var clueLabel = new Label();
			clueLabel.Text = word.Clue;
			clueLabel.SizeFlagsHorizontal = SizeFlags.ExpandFill;
			clueLabel.AutowrapMode = TextServer.AutowrapMode.Word;
			clueLabel.CustomMinimumSize = new Vector2(200, 30);
			slotContainer.AddChild(clueLabel);
			
			var progressLabel = new Label();
			progressLabel.Text = word.PlacedLetters;
			progressLabel.CustomMinimumSize = new Vector2(150, 30);
			progressLabel.HorizontalAlignment = HorizontalAlignment.Center;
			slotContainer.AddChild(progressLabel);
			
			clueSlots.Add(new ClueSlot
			{
				WordIndex = i,
				ProgressLabel = progressLabel
			});
		}
	}
	
	private void OnLetterButtonClicked(LetterButton letterBtn)
	{
		if (!isGameActive || movesLeft <= 0) return;
		
		GD.Print($"Kliknięto literę: {letterBtn.GetLetter()}");
		
		if (selectedLetter != null)
		{
			selectedLetter.SetColor(normalLetterColor);
		}
		
		if (selectedLetter == letterBtn)
		{
			selectedLetter = null;
			GD.Print("Odznaczono literę");
		}
		else
		{
			selectedLetter = letterBtn;
			selectedLetter.SetColor(selectedLetterColor);
			GD.Print($"Wybrano literę: {letterBtn.GetLetter()}");
		}
	}
	
	private void OnCrosswordCellClicked(CrosswordCell cell)
	{
		if (!isGameActive || selectedLetter == null || cell.WordIndex == -1 || movesLeft <= 0) 
		{
			GD.Print("Nie można umieścić litery");
			return;
		}
		
		GD.Print($"Próba umieszczenia litery {selectedLetter.GetLetter()} w komórce");
		
		if (!string.IsNullOrEmpty(cell.CurrentLetter))
		{
			GD.Print("Komórka już zajęta!");
			return;
		}
		
		string letter = selectedLetter.GetLetter();
		
		bool isCorrect = letter == cell.CorrectLetter;
		
		if (isCorrect)
		{
			bool placed = cell.PlaceLetter(letter);
			
			if (placed)
			{
				GD.Print($"Umieszczono POPRAWNĄ literę {letter}");
				
				movesLeft--;
				UpdateUI();
				
				selectedLetter.SetColor(normalLetterColor);
				selectedLetter.QueueFree(); 
				letterButtons.Remove(selectedLetter);
				selectedLetter = null;
				
				UpdateClueDisplay(cell.WordIndex);
				
				CheckAllWords();
			}
		}
		else
		{
			GD.Print($"BŁĄD: Litera {letter} nie pasuje! Oczekiwano: {cell.CorrectLetter}");
			
			cell.ShowWrongLetterTemporarily(letter);
			
			movesLeft--;
			UpdateUI();
			
			if (selectedLetter != null)
			{
				selectedLetter.SetColor(normalLetterColor);
				selectedLetter = null;
			}
		}
	}
	
	private void CheckWordCompletion(int wordIndex)
	{
		if (wordIndex < 0 || wordIndex >= currentWords.Count) return;
		
		var word = currentWords[wordIndex];
		bool complete = true;
		
		for (int i = 0; i < word.Answer.Length; i++)
		{
			var cell = crosswordCells.FirstOrDefault(c => 
				c.WordIndex == wordIndex && c.LetterIndex == i);
			
			if (cell == null || string.IsNullOrEmpty(cell.CurrentLetter))
			{
				complete = false;
				break;
			}
		}
		
		if (complete)
		{
			GD.Print($"Słowo {wordIndex + 1} ({word.Answer}) jest kompletne!");
			word.IsComplete = true;
		}
	}
	
	private void CheckAllWords()
	{
		bool allComplete = true;
		bool allCorrect = true;
		
		for (int i = 0; i < currentWords.Count; i++)
		{
			var word = currentWords[i];
			bool wordComplete = true;
			bool wordCorrect = true;
			
			for (int j = 0; j < word.Answer.Length; j++)
			{
				var cell = crosswordCells.FirstOrDefault(c => 
					c.WordIndex == i && c.LetterIndex == j);
				
				if (cell == null || string.IsNullOrEmpty(cell.CurrentLetter))
				{
					wordComplete = false;
					wordCorrect = false;
					break;
				}
				
				if (!cell.IsCorrect())
				{
					wordCorrect = false;
				}
			}
			
			if (!wordComplete) allComplete = false;
			if (!wordCorrect) allCorrect = false;
		}
		
		if (allCorrect && allComplete)
		{
			GD.Print("WSZYSTKIE SŁOWA POPRAWNE! POZIOM UKOŃCZONY!");
			isGameActive = false;
			ShowWinPanel();
		}
	}
	
	private void OnResetButtonPressed()
	{
		GD.Print("Resetowanie poziomu...");
		LoadLevel(currentLevel);
	}
	
	private void OnCheckButtonPressed()
	{
		if (!isGameActive) return;
		
		GD.Print("Sprawdzanie wszystkich odpowiedzi...");
		CheckAllWords();
	}
	
	private void OnHintButtonPressed()
	{
		if (!isGameActive || movesLeft <= 0) return;
		
		GD.Print("Użyto podpowiedzi");
		
		for (int i = 0; i < currentWords.Count; i++)
		{
			var word = currentWords[i];
			if (!word.IsComplete)
			{
				for (int j = 0; j < word.Answer.Length; j++)
				{
					var cell = crosswordCells.FirstOrDefault(c => 
						c.WordIndex == i && c.LetterIndex == j);
					
					if (cell != null && string.IsNullOrEmpty(cell.CurrentLetter))
					{
						var letterBtn = letterButtons.FirstOrDefault(l => 
							l.GetLetter() == word.Answer[j].ToString());
						
						if (letterBtn != null)
						{
							cell.PlaceLetter(word.Answer[j].ToString());
							
							if (selectedLetter != null)
							{
								selectedLetter.SetColor(normalLetterColor);
								selectedLetter = null;
							}
							
							letterBtn.QueueFree();
							letterButtons.Remove(letterBtn);

							movesLeft--;
							UpdateUI();
							
							GD.Print($"Podpowiedź: umieszczono literę {word.Answer[j]} w słowie {i + 1}");
							
							UpdateClueDisplay(i);
							
							CheckWordCompletion(i);
							CheckAllWords();
							return;
						}
					}
				}
			}
		}
	}
	
	private void UpdateClueDisplay(int wordIndex)
	{
		if (wordIndex >= 0 && wordIndex < clueSlots.Count)
		{
			var word = currentWords[wordIndex];
			var slot = clueSlots[wordIndex];
			
			char[] display = new char[word.Answer.Length];
			for (int i = 0; i < word.Answer.Length; i++)
			{
				var cell = crosswordCells.FirstOrDefault(c => 
					c.WordIndex == wordIndex && c.LetterIndex == i);
				
				display[i] = cell != null && !string.IsNullOrEmpty(cell.CurrentLetter) 
					? cell.CurrentLetter[0] 
					: '_';
			}
			
			word.PlacedLetters = new string(display);
			slot.ProgressLabel.Text = word.PlacedLetters;
		}
	}
	
	private void RandomizeArray<T>(T[] array)
	{
		Random rng = new Random();
		int n = array.Length;
		while (n > 1)
		{
			n--;
			int k = rng.Next(n + 1);
			T value = array[k];
			array[k] = array[n];
			array[n] = value;
		}
	}
	
	private CrosswordCell CreateFallbackCell()
	{
		var cell = new CrosswordCell();
		cell.CustomMinimumSize = new Vector2(40, 40);
		return cell;
	}
	
	private LetterButton CreateFallbackLetterButton()
	{
		var btn = new LetterButton();
		btn.CustomMinimumSize = new Vector2(60, 60);
		return btn;
	}

	private void ShowWinPanel()
	{
		GD.Print("=== ShowWinPanel() ===");
		isGameActive = false;
		
		UnlockNextLevel();
		
		if (winPanelPrefab != null)
		{
			var winPanel = winPanelPrefab.Instantiate<WinPanel>();
			AddChild(winPanel);
			
			int score = CalculateScore();
			winPanel.SetScore(score);
			
			winPanel.ShowWithAnimation();
			
			winPanel.NextLevel += OnNextLevel;
			winPanel.GoToMenu += OnGoToMenu;
			
			GD.Print("WinPanel wyświetlony z wynikiem: " + score);
		}
		else
		{
			GD.PrintErr("Brak prefabu WinPanel!");
			CreateFallbackWinMessage();
		}
	}
	
	private void ShowLosePanel()
	{
		GD.Print("=== ShowLosePanel() ===");
		isGameActive = false;
		
		if (losePanelPrefab != null)
		{
			var losePanel = losePanelPrefab.Instantiate<LosePanel>();
			AddChild(losePanel);
			
			int score = CalculateScore();
			losePanel.SetScore(score);
			
			losePanel.ShowWithAnimation();
			
			losePanel.RetryLevel += OnRetryLevel;
			losePanel.GoToMenu += OnGoToMenu;
			
			GD.Print("LosePanel wyświetlony z wynikiem: " + score);
		}
		else
		{
			GD.PrintErr("Brak prefabu LosePanel!");
			CreateFallbackLoseMessage();
		}
	}
	
	private int CalculateScore()
	{
		int score = 0;
		
		foreach (var cell in crosswordCells)
		{
			if (cell.IsCorrect())
				score += 10;
		}
		
		score += movesLeft * 5;
		
		if (!isGameActive)
		{
			bool allWordsComplete = true;
			for (int i = 0; i < currentWords.Count; i++)
			{
				if (!currentWords[i].IsComplete)
				{
					allWordsComplete = false;
					break;
				}
			}
			
			if (allWordsComplete)
			{
				score += 50; 
				GD.Print("Dodano bonus 50 pkt za ukończenie poziomu");
			}
		}
		
		GD.Print($"Obliczony wynik: {score} (poprawne litery: {crosswordCells.Count(c => c.IsCorrect())}, ruchy pozostałe: {movesLeft})");
		return score;
	}
	
	private void CreateFallbackWinMessage()
	{
		var container = new VBoxContainer();
		container.Position = new Vector2(Size.X / 2 - 150, Size.Y / 2 - 100);
		container.CustomMinimumSize = new Vector2(300, 200);
		AddChild(container);
		
		string message = $"WYGRANA!\nPoziom {currentLevel} ukończony!\nWynik: {CalculateScore()}";
		
		int nextLevel = currentLevel + 1;
		if (nextLevel <= 15 && nextLevel > saveData.HighestUnlockedLevel)
		{
			message += $"\n\nOdblokowano poziom {nextLevel}!";
		}
		
		var label = new Label();
		label.Text = message;
		label.HorizontalAlignment = HorizontalAlignment.Center;
		label.AutowrapMode = TextServer.AutowrapMode.Word;
		container.AddChild(label);
		
		var nextButton = new Button();
		nextButton.Text = "Następny poziom";
		nextButton.Pressed += () => {
			container.QueueFree();
			OnNextLevel();
		};
		container.AddChild(nextButton);
		
		var menuButton = new Button();
		menuButton.Text = "Menu";
		menuButton.Pressed += () => {
			container.QueueFree();
			OnGoToMenu();
		};
		container.AddChild(menuButton);
	}
	
	private void CreateFallbackLoseMessage()
	{
		var container = new VBoxContainer();
		container.Position = new Vector2(Size.X / 2 - 150, Size.Y / 2 - 100);
		container.CustomMinimumSize = new Vector2(300, 200);
		AddChild(container);
		
		var label = new Label();
		label.Text = $"PRZEGRANA!\nSkończyły Ci się ruchy.\nWynik: {CalculateScore()}";
		label.HorizontalAlignment = HorizontalAlignment.Center;
		container.AddChild(label);
		
		var retryButton = new Button();
		retryButton.Text = "Spróbuj ponownie";
		retryButton.Pressed += () => {
			container.QueueFree();
			OnRetryLevel();
		};
		container.AddChild(retryButton);
		
		var menuButton = new Button();
		menuButton.Text = "Menu";
		menuButton.Pressed += () => {
			container.QueueFree();
			OnGoToMenu();
		};
		container.AddChild(menuButton);
	}
	
	public void OnNextLevel()
	{
		GD.Print("=== OnNextLevel() WYWOŁANY ===");
		GD.Print($"Przechodzenie z poziomu {currentLevel} do {currentLevel + 1}");
		
		currentLevel++;
		
		if (currentLevel > saveData.HighestUnlockedLevel)
		{
			GD.Print($"Poziom {currentLevel} nie jest jeszcze odblokowany! Wracam do najwyższego odblokowanego.");
			currentLevel = saveData.HighestUnlockedLevel;
		}
		
		if (!levelsData.ContainsKey(currentLevel))
		{
			GD.Print($"Brak poziomu {currentLevel}! To był ostatni poziom. Wracam do menu.");
			OnGoToMenu();
			return;
		}
		
		GD.Print($"Ładuję poziom {currentLevel}");
		
		foreach (Node child in GetChildren().ToArray())
		{
			if (child is LosePanel || child is WinPanel)
			{
				GD.Print($"Usuwam panel: {child.Name}");
				child.QueueFree();
			}
		}
		
		CallDeferred("LoadLevel", currentLevel);
	}
	
	public void OnRetryLevel()
	{
		GD.Print("=== OnRetryLevel() WYWOŁANY ===");
		GD.Print($"Ponowne ładowanie poziomu: {currentLevel}");
		
		foreach (Node child in GetChildren().ToArray())
		{
			if (child is LosePanel || child is WinPanel)
			{
				GD.Print($"Usuwam panel: {child.Name}");
				child.QueueFree();
			}
		}
		
		CallDeferred("LoadLevel", currentLevel);
	}
	
	public void OnGoToMenu()
	{
		GD.Print("=== OnGoToMenu() WYWOŁANY ===");
		GD.Print("Przechodzenie do menu...");
		
		foreach (Node child in GetChildren().ToArray())
		{
			if (child is LosePanel || child is WinPanel)
			{
				child.QueueFree();
			}
		}
		
		var menuScene = GD.Load<PackedScene>("res://sceny/menu.tscn");
		if (menuScene != null)
		{
			GetTree().ChangeSceneToPacked(menuScene);
		}
		else
		{
			GD.PrintErr("Nie znaleziono sceny menu.tscn! Sprawdź ścieżkę.");
			
			menuScene = GD.Load<PackedScene>("res://Menu.tscn");
			if (menuScene != null)
			{
				GetTree().ChangeSceneToPacked(menuScene);
			}
			else
			{
				GetTree().ChangeSceneToFile("res://MainMenu.tscn");
			}
		}
	}

	public void LoadSpecificLevel(int level)
	{
		GD.Print($"Próba załadowania poziomu {level}");
		
		if (!levelsData.ContainsKey(level))
		{
			GD.PrintErr($"Poziom {level} nie istnieje!");
			return;
		}
		
		if (level > saveData.HighestUnlockedLevel)
		{
			GD.PrintErr($"Poziom {level} nie jest odblokowany! Najwyższy odblokowany to {saveData.HighestUnlockedLevel}");
			return;
		}
		
		GD.Print($"Ładowanie poziomu {level} (odblokowany: {level <= saveData.HighestUnlockedLevel})");
		
		currentLevel = level;
		
		foreach (Node child in GetChildren().ToArray())
		{
			if (child is LosePanel || child is WinPanel)
			{
				GD.Print($"Usuwam panel: {child.Name}");
				child.QueueFree();
			}
		}
		
		LoadLevel(currentLevel);
	}
}

public class GameLevelData
{
	public int MaxMoves { get; set; }
	public List<WordWithClue> Words { get; set; } = new();
}

public class WordWithClue
{
	public string Word { get; set; } = "";
	public string Clue { get; set; } = "";
}

public class WordData
{
	public int Index { get; set; }
	public string Answer { get; set; } = "";
	public string Clue { get; set; } = "";
	public bool IsComplete { get; set; }
	public string PlacedLetters { get; set; } = "";
}

public class ClueSlot
{
	public int WordIndex { get; set; }
	public Label ProgressLabel { get; set; }
}
