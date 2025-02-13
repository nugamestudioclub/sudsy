﻿using Godot;

public class Particle : Sprite {
	[Export]
	public float Lifespan { get; set; }

	public float TimeToLive { get; private set; }

	public bool IsAlive => TimeToLive > 0;

	public Vector2 Velocity { get; set; }

	[Export]
	public float GravityScale { get; set; } = 1f;

	public float AngularVelocity { get; set; }

	public override void _Ready() {
		TimeToLive = Lifespan;
	}

	public override void _PhysicsProcess(float delta) {
		Rotation += AngularVelocity * delta;
		Velocity += GravityScale * GameLogic.Gravity;
        Position += Velocity * delta;
		
		TimeToLive -= delta;
		if( !IsAlive )
			QueueFree();
	}
}