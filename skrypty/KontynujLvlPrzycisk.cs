using Godot;
using System;
using System.Text.Json;

public partial class KontynujLvlPrzycisk : TextureButton
{
	private readonly string saveFilePath = "user://game_save.json";
	private PackedScene levelScenePrefab;

	public override void _Ready()
	{
		levelScenePrefab = GD.Load<PackedScene>("res://sceny/levele/LevelScene.tscn");
		CheckSaveFileExists();
		Pressed += OnContinuePressed;
	}

	private void CheckSaveFileExists()
	{
		if (Godot.FileAccess.FileExists(saveFilePath))
		{
			Disabled = false;
		}
		else
		{
			Disabled = true;
		}
	}

	private void OnContinuePressed()
	{
		if (!Godot.FileAccess.FileExists(saveFilePath))
		{
			GD.Print("Brak pliku zapisu");
			return;
		}

		using var file = Godot.FileAccess.Open(saveFilePath, Godot.FileAccess.ModeFlags.Read);
		if (file == null) return;

		try
		{
			string json = file.GetAsText();
			var saveData = JsonSerializer.Deserialize<GameSaveData>(json);
			
			GetTree().ChangeSceneToPacked(levelScenePrefab);
			
			CallDeferred("LoadLevelAfterSceneChange", saveData.HighestUnlockedLevel);
		}
		catch (Exception e)
		{
			GD.PrintErr($"Błąd: {e.Message}");
		}
	}

	private void LoadLevelAfterSceneChange(int level)
	{
		var levelScene = GetTree().CurrentScene as LevelScene;
		if (levelScene != null)
		{
			levelScene.LoadSpecificLevel(level);
		}
	}

	private class GameSaveData
	{
		public int HighestUnlockedLevel { get; set; } = 1;
	}
}
