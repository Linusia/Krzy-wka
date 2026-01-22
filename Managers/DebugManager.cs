using Godot;

public static class DebugTools
{
	public static void PrintSceneTree(Node node, int depth = 0)
	{
		string indent = new string(' ', depth * 2);
		GD.Print($"{indent}└─ {node.Name} ({node.GetType().Name})");
		
		foreach (Node child in node.GetChildren())
		{
			PrintSceneTree(child, depth + 1);
		}
	}
	
	public static void PrintButtonInfo(Button button)
	{
		GD.Print($"Button: {button.Name}");
		GD.Print($"  Text: {button.Text}");
		GD.Print($"  Disabled: {button.Disabled}");
		GD.Print($"  Visible: {button.Visible}");
		GD.Print($"  Position: {button.Position}");
		GD.Print($"  Size: {button.Size}");
	}
}
