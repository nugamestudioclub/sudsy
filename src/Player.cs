using Godot;
using System;

public class Player : RigidBody2D {
	[Export]
	private float _moveScale = 100.0f;

	[Export]
	private float _jumpScale = 100.0f;

	[Export]
	private float _gravityScale = 1.0f;

	[Export]
	private float _moveMultiplierMidair = 1.0f;

	[Export]
	private float _gravityModifierJumping = 1.0f;

	[Export]
	private float _maxSpeed = 100;

	private ShapeCast2D _feet;

	private bool _areFeetColliding;

	public bool IsGrounded => _areFeetColliding;

	public int JumpCount { get; private set; }

	[Export]
	public float JumpDuration { get; private set; } = 1.0f;

	public float TimeInState { get; private set; }

	public PlayerState State { get; private set; }
	
	public override void _Ready() {
		_feet = GetNode<ShapeCast2D>("Feet");
		EnterState(PlayerState.Idle);
	}

	public override void _PhysicsProcess(float delta) {
		_feet.ForceShapecastUpdate();
		_areFeetColliding = _feet.IsColliding();
		if( _areFeetColliding )
			JumpCount = 0;
	}

	public void EnterState(PlayerState value) {
		State = value;
		TimeInState = 0f;
		GravityScale = _gravityScale * (value == PlayerState.Jumping ? _gravityModifierJumping : 1f);
	}

	public void UpdateState(float delta) {
		TimeInState += delta;
	}

	public void Jump(float delta) {
		++JumpCount;
		var impulse = _jumpScale * Vector2.Up;
		ApplyImpulse(Vector2.Zero, impulse);
	}

	public void MoveX(float value, float delta) {
		if( Mathf.Abs(LinearVelocity.x) > _maxSpeed )
			return;
		float moveRate = _moveScale * (IsGrounded ? 1f : _moveMultiplierMidair);
		var impulse = value * moveRate * delta * Vector2.Right;
		ApplyImpulse(Vector2.Zero, impulse);
	}
}