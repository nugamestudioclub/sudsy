using Godot;
using System;
using System.Numerics;

public class PlayScene : Node2D {
	private readonly InputFrame _input = new InputFrame();

	public Player Player { get; private set; }

	public override void _Ready() {
		Player = GetNode<Player>("Player");
	}

	public override void _PhysicsProcess(float delta) {
		ProcessPlayerState(Player, delta);
		MovePlayer(Player, delta);
	}

	public override void _Process(float delta) {
		ProcessInput(delta);
	}

	private void ProcessInput(float delta) {
		if( Input.IsActionJustPressed("left") )
			_input.Set(ButtonKind.Left);
		else if( !Input.IsActionPressed("left") )
			_input.Reset(ButtonKind.Left);

		if( Input.IsActionJustPressed("right") )
			_input.Set(ButtonKind.Right);
		else if( !Input.IsActionPressed("right") )
			_input.Reset(ButtonKind.Right);

		if( Input.IsActionJustPressed("up") )
			_input.Set(ButtonKind.Up);
		else if( !Input.IsActionPressed("up") )
			_input.Reset(ButtonKind.Up);

		if( Input.IsActionJustPressed("down") )
			_input.Set(ButtonKind.Down);
		else if( !Input.IsActionPressed("down") )
			_input.Reset(ButtonKind.Down);

		if( Input.IsActionJustPressed("jump") )
			_input.Set(ButtonKind.Jump);
		else if( !Input.IsActionPressed("jump") )
			_input.Reset(ButtonKind.Jump);

		if( Input.IsActionJustPressed("slide") )
			_input.Set(ButtonKind.Slide);
		else if( !Input.IsActionPressed("slide") )
			_input.Reset(ButtonKind.Slide);
	}

	private void MovePlayer(Player player, float delta) {
		float inputX = _input.X;
		if( !Mathf.IsZeroApprox(inputX) ) {
			player.MoveX(inputX, delta);
		}
		if( IsStartingJump(player) ) {
			player.Jump(delta);
		}
		else if( !IsJumping(player) ) {
			_input.Reset(ButtonKind.Jump);
		}
	}

	private bool IsFalling(Player player) {
		return !player.IsGrounded
			&& (
				!_input.Get(ButtonKind.Jump)
				|| player.TimeInState > player.JumpDuration
				|| player.LinearVelocity.y > 0
			);
	}

	private bool IsJumping(Player player) {
		return !player.IsGrounded
			&& _input.Get(ButtonKind.Jump)
			&& player.TimeInState <= player.JumpDuration && player.LinearVelocity.y < 0;
	}
	private bool IsMoving(Player player) {
		return player.IsGrounded && !Mathf.IsZeroApprox(player.LinearVelocity.x);
	}

	private bool IsIdle(Player player) {
		return player.IsGrounded && Mathf.IsZeroApprox(player.LinearVelocity.x);
	}

	private bool IsStartingJump(Player player) {
		return _input.Get(ButtonKind.Jump) && player.IsGrounded && player.JumpCount == 0 && !player.IsSliding;
	}

	private void ProcessPlayerState(Player player, float delta) {
		player.IsSliding = _input.Get(ButtonKind.Slide);
		if( IsMoving(player) && player.State != PlayerState.Moving ) {
			player.EnterState(PlayerState.Moving);
		}
		else if( IsIdle(player) && player.State != PlayerState.Idle ) {
			player.EnterState(PlayerState.Idle);
		}
		else if( IsFalling(player) && player.State != PlayerState.Freefall ) {
			player.EnterState(PlayerState.Freefall);
			_input.Reset(ButtonKind.Jump);
		}
		else if( IsJumping(player) && player.State != PlayerState.Jumping ) {
			player.EnterState(PlayerState.Jumping);
		}
		player.UpdateState(delta);
	}
}