using Godot;
using System;

public class Player : RigidBody2D {
	[Export]
	private float _moveScale = 100.0f;

	[Export]
	private float _jumpScale = 100.0f;

	[Export]
	private float _maxSpeed = 100;

	[Export]
	private float _gravityScale = 1.0f;

	[Export]
	private float _jumpGravityScale = 1.0f;

	private int _groundCount;

	public bool IsGrounded => JumpCount == 0 && _groundCount > 0;

	public int JumpCount { get; private set; }

	[Export]
	public float JumpDuration { get; private set; } = 1.0f;

	public float TimeInState { get; private set; }

	public PlayerState State { get; private set; }

	public override void _IntegrateForces(Physics2DDirectBodyState state) {
		state.Transform = new Transform2D(0.0f, Position);
	}

	public override void _Ready() {
		GravityScale = _gravityScale;
	}

	public void EnterState(PlayerState value) {
		State = value;
		TimeInState = 0f;
	}

	public void UpdateState(float delta) {
		TimeInState += delta;
	}

	public void Fall(float delta) {
		GravityScale = _gravityScale;
	}

	public void Jump(float delta) {
		++JumpCount;
		var impulse = _jumpScale * Vector2.Up;
		ApplyImpulse(Vector2.Zero, impulse);
		GravityScale = _jumpGravityScale;
	}

	public void MoveX(float value, float delta) {
		if( Mathf.Abs(LinearVelocity.x) > _maxSpeed )
			return;
		var impulse = value * _moveScale * delta * Vector2.Right;
		ApplyImpulse(Vector2.Zero, impulse);
	}

	private void Feet_BodyEntered(Node body) {
		++_groundCount;
		JumpCount = 0;
	}

	private void Feet_BodyExited(Node body) {
		--_groundCount;
	}
}