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
	private float _maxSpeed = 100;

	[Export]
	private float _frictionScale;

	[Export]
	private float _moveMultiplierMidair = 1.0f;

	[Export]
	private float _gravityModifierJumping = 1.0f;

	[Export]
	private float _maxSpeedModifierSliding = 1.0f;

	[Export]
	private float _frictionModifierSliding = 1.0f;

	private CollisionShape2D _tallHitbox;

	private CollisionShape2D _shortHitbox;

	private ShapeCast2D _feet;

	private AnimatedSprite _sprite;

	private bool _areFeetColliding;

	private bool _isSliding;
	public bool IsGrounded => _areFeetColliding;
	public bool IsSliding {
		get => _isSliding;
		set {
			_isSliding = value;
			Friction = _frictionScale * (value ? _frictionModifierSliding : 1f);
			_tallHitbox.Disabled = value;
			_shortHitbox.Disabled = !value;
		}
	}

	public int JumpCount { get; private set; }

	[Export]
	public float JumpDuration { get; private set; } = 1.0f;

	public float TimeInState { get; private set; }

	public PlayerState State { get; private set; }

	private bool _isAnimationFinished;

	public override void _Ready() {
		_tallHitbox = GetNode<CollisionShape2D>("TallHitbox");
		_shortHitbox = GetNode<CollisionShape2D>("ShortHitbox");
		_feet = GetNode<ShapeCast2D>("Feet");
		_sprite = GetNode<AnimatedSprite>("AnimatedSprite");
		EnterState(PlayerState.Idle);
		IsSliding = false;
	}

	public override void _PhysicsProcess(float delta) {
		_feet.ForceShapecastUpdate();
		_areFeetColliding = _feet.IsColliding();
		if( _areFeetColliding )
			JumpCount = 0;
	}

	public override void _Process(float delta) {
		PlayAnimation(GetDesiredAnimation());
	}

	public void EnterState(PlayerState value) {
		//GD.Print($"EnterState {value}");
		State = value;
		TimeInState = 0f;
		GravityScale = _gravityScale * (value == PlayerState.Jumping ? _gravityModifierJumping : 1f);
	}

	public void UpdateState(float delta) {
		TimeInState += delta;
	}

	public void Face(float value) {
		_sprite.FlipH = value < 0;
	}

	public void Jump(float delta) {
		++JumpCount;
		var impulse = _jumpScale * Vector2.Up;
		ApplyImpulse(Vector2.Zero, impulse);
		IsSliding = false;
	}

	public void MoveX(float value, float delta) {
		float speedLimit = _maxSpeed * (IsGrounded && IsSliding ? _maxSpeedModifierSliding : 1f);
		if( Mathf.Abs(LinearVelocity.x) > speedLimit )
			return;
		if( IsSliding )
			return;
		float moveRate = _moveScale * (IsGrounded ? 1f :_moveMultiplierMidair);
		var impulse = value * moveRate * delta * Vector2.Right;
		ApplyImpulse(Vector2.Zero, impulse);
	}

	public void PlayAnimation(string animation) {
		string currentAnimation = _sprite.Animation;
		string nextAnimation = HasTransition(currentAnimation, animation)
			? currentAnimation + "-" + animation
			: animation;
		if( nextAnimation == currentAnimation )
			return;
		if( (!_isAnimationFinished && ArePartOfTransition(currentAnimation, nextAnimation)) )
			return;
		_sprite.Play(nextAnimation);
		_isAnimationFinished = false;
	}

	private static bool HasTransition(string animation1, string animation2) {
		switch( animation1 ) {
		case "idle": return animation2 == "jump" || animation2 == "slide";
		case "jump": return animation2 == "idle";
		case "slide": return animation2 == "idle";
		default: return false;
		}
	}

	private static bool ArePartOfTransition(string animation1, string animation2) {
		switch( animation1 ) {
		case "idle": return animation2 == "idle-jump" || animation2 == "idle-slide";
		case "idle-jump": return animation2 == "jump";
		case "idle-slide": return animation2 == "slide";
		case "jump": return animation2 == "jump-idle";
		case "jump-idle": return animation2 == "idle";
		case "slide-idle": return animation2 == "idle";
		case "slide": return animation2 == "slide-idle";
		default: return false;
		}
	}

	private string GetDesiredAnimation() {
		switch( State ) {
		case PlayerState.Idle:
		case PlayerState.Moving:
			return IsSliding ? "slide" : "idle";
		case PlayerState.Freefall:
		case PlayerState.Jumping:
			return "jump";
		default: return "idle";
		}
	}

	private void AnimatedSprite_AnimationFinished() {
		_isAnimationFinished = true;
	}
}