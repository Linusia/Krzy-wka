using Godot;

public static class GameManager
{
	public static int LevelToLoad { get; set; } = -1;
	
	public static int TotalScore { get; set; } = 0;
	public static int CurrentStreak { get; set; } = 0;
	
	private static bool isInitialized = false;
	
	public static void Initialize()
	{
		if (!isInitialized)
		{
			GD.Print("GameManager zainicjalizowany");
			LevelToLoad = -1;
			TotalScore = 0;
			CurrentStreak = 0;
			isInitialized = true;
		}
	}
	
	public static void Reset()
	{
		LevelToLoad = -1;
		TotalScore = 0;
		CurrentStreak = 0;
		GD.Print("GameManager zresetowany");
	}
	
	public static void PrintStatus()
	{
		GD.Print($"GameManager Status: LevelToLoad={LevelToLoad}, TotalScore={TotalScore}, Streak={CurrentStreak}");
	}
}
