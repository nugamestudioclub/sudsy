﻿using Godot;

public class Soap : Node2D {
	private ParticleEmitter _MoveEmitter;

	private ParticleEmitter _JumpEmitter;

	private ParticleEmitter _MidAirJumpEmitter;

	private ParticleEmitter _SlideEmitter;

	public override void _Ready() {
		_MoveEmitter = GetNode<ParticleEmitter>("MoveEmitter");
		_JumpEmitter = GetNode<ParticleEmitter>("JumpEmitter");
		_MidAirJumpEmitter = GetNode<ParticleEmitter>("MidairJumpEmitter");
		_SlideEmitter = GetNode<ParticleEmitter>("SlideEmitter");
	}

	public void Jump(Vector2 position) {
		_JumpEmitter.Spawn(position);
	}

	public void MidairJump(Vector2 position) {
		_MidAirJumpEmitter.Spawn(position);
	}

	public void Move(Vector2 position, Vector2 offset, Vector2 velocity) {
		float dx = _MoveEmitter.Extents.x - offset.x;
		float spawnPosX = velocity.x < 0 ? position.x - dx : position.x + dx;
        _MoveEmitter.Spawn(new Vector2(spawnPosX, position.y), velocity);
	}

	public void Slide(Vector2 position, Vector2 velocity) {
		_SlideEmitter.Spawn(position, velocity);
	}
}