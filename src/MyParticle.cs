using Godot;

public class MyParticle : Sprite {
	[Export]
	public float Lifespan { get; set; }

	public float TimeToLive { get; private set; }

	public bool IsAlive => TimeToLive > 0;

	[Export]
	public Vector2 Velocity { get; set; }

	[Export]
	public float AngularVelocity { get; set; }

	public override void _Ready() {
		TimeToLive = Lifespan;
	}

	public override void _PhysicsProcess(float delta) {
		Rotation += AngularVelocity;
		Position += Velocity;
		TimeToLive -= delta;
		if( !IsAlive )
			QueueFree();
	}
}