using Godot;

public partial class ButtonMenuWL : TextureButton
{
	// Użyj [Export] dla ścieżki - to jest natywny typ Godota
	[Export] public string MenuScenePath = "res://sceny/lvl_menu.tscn";
	
	public override void _Ready()
	{
		Pressed += OnPressed;
	}
	
	private void OnPressed()
	{
		GD.Print("ButtonMenu: Przechodzę do menu");
		
		// Sprawdź czy ścieżka jest ustawiona
		if (string.IsNullOrEmpty(MenuScenePath))
		{
			GD.PrintErr("ButtonMenu: MenuScenePath nie jest ustawiony!");
			return;
		}
		
		var menuScene = GD.Load<PackedScene>(MenuScenePath);
		if (menuScene != null)
		{
			GetTree().ChangeSceneToPacked(menuScene);
		}
		else
		{
			GD.PrintErr($"ButtonMenu: Nie znaleziono sceny: {MenuScenePath}");
			
			// Fallback: spróbuj domyślnej ścieżki
			var fallbackScene = GD.Load<PackedScene>("res://scenes/Menu.tscn");
			if (fallbackScene != null)
			{
				GetTree().ChangeSceneToPacked(fallbackScene);
			}
		}
	}
}
