using Godot;
using System.Collections.Generic;

public class ParticleEmitter : Node2D {
	[Export]
	public int Quantity { get; set; } = 10;

	[Export]
	public float ScaleMin { get; set; } = 1f;

	[Export]
	public float ScaleMax { get; set; } = 1f;

	[Export]
	public float ParticleMinSpeed { get; set; } = 0f;

    [Export]
    public float ParticleMaxSpeed { get; set; } = 30f;

    [Export]
    public float ParticleMinLifespan { get; set; } = .5f;

    [Export]
    public float ParticleMaxLifespan { get; set; } = 2f;

	[Export]
	public Vector2 Extents { get; set; }


    //[Export]
    //public float DecayRate { get; set; } = 0.1f;

    //[Export]
    //public Vector2 RateMin { get; set; } = new Vector2(-1f, -1f);

    //[Export]
    //public Vector2 RateMax { get; set; } = new Vector2(1f, 1f);

    [Export]
	private List<PackedScene> _particles = new List<PackedScene>();
	
	public void Spawn(Vector2 position, Vector2 velocity = default) {
		if( _particles.Count == 0 )
			return;
		for( int i = 0; i < Quantity; i++ ) {
			int index = GameLogic.Random.Next(_particles.Count);
			var particle = (Particle)_particles[index].Instance();
			particle.GlobalPosition = GameLogic.Random.NextPointInside(Extents, position);

            float randomAngleRadians = Mathf.Deg2Rad(GameLogic.Random.Next(360));
			Vector2 randomDirection = new Vector2(Mathf.Cos(randomAngleRadians), Mathf.Sin(randomAngleRadians));
			float randomSpeed = (float) GameLogic.Random.NextBetween(ParticleMaxSpeed, ParticleMinSpeed);
			particle.Velocity = (velocity.Normalized() + randomDirection).Normalized() * randomSpeed;
			
			float randomLifespan = (float)GameLogic.Random.NextBetween(ParticleMinLifespan, ParticleMaxLifespan);
			particle.Lifespan = randomLifespan;
            particle.SetAsToplevel(true);
			AddChild(particle);
		}
	}

}