using Godot;
using System.Collections.Generic;

public class LevelData
{
	public int Moves;
	public int GridWidth;
	public int GridHeight;
	public Dictionary<Vector2I, char> LettersInGrid;
	public List<char> AvailableLetters;
}

public static class Levels
{
	public static List<LevelData> All = new List<LevelData>()
	{
		new LevelData()
		{
			Moves = 20,
			GridWidth = 5,
			GridHeight = 5,
			LettersInGrid = new Dictionary<Vector2I, char>()
			{
				{ new Vector2I(1,1), 'K' },
				{ new Vector2I(2,1), 'O' },
				{ new Vector2I(3,1), 'T' },
			},
			AvailableLetters = new List<char>(){ 'K','O','T','A','L' }
		},

		new LevelData()
		{
			Moves = 18,
			GridWidth = 6,
			GridHeight = 6,
			LettersInGrid = new Dictionary<Vector2I, char>()
			{
				{ new Vector2I(2,2), 'D' },
				{ new Vector2I(3,2), 'O' },
				{ new Vector2I(4,2), 'M' },
			},
			AvailableLetters = new List<char>(){ 'D','O','M','A','K' }
		}
	};
}
