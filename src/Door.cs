using Godot;

public class Door : Area2D {
	[Export]
	public NodePath Destination { get; private set; }

	public Node2D CameraHook { get; private set; }

	public override void _Ready() {
		CameraHook = GetNode<Node2D>("CameraHook");
	}
}