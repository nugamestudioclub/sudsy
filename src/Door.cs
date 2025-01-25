using Godot;

public class Door : Area2D {
	[Export]
	public NodePath Destination { get; private set; }
}