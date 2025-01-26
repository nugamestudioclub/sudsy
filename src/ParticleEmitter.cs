using Godot;
using System.Collections.Generic;

public class ParticleEmitter : Node2D {
	[Export]
	public int Quantity { get; set; } = 10;

	//[Export]
	//public float ScaleMin { get; set; } = 1f;

	//[Export]
	//public float ScaleMax { get; set; } = 1f;

	//[Export]
	//public float DecayRate { get; set; } = 0.1f;

	//[Export]
	//public Vector2 RateMin { get; set; } = new Vector2(-1f, -1f);

	//[Export]
	//public Vector2 RateMax { get; set; } = new Vector2(1f, 1f);

	[Export]
	private List<PackedScene> _particles = new List<PackedScene>();
	
	public void Spawn(Vector2 position = default, Vector2 velocity = default) {
		if( _particles.Count == 0 )
			return;
		for( int i = 0; i < Quantity; i++ ) {
			int index = GameLogic.Random.Next(_particles.Count);
			var particle = (Particle)_particles[index].Instance();
			particle.Position = position - Position;
			AddChild(particle);
		}
	}
}