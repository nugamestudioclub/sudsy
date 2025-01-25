using Godot;

public class PlayScene : Node2D {
	private readonly InputFrame _input = new InputFrame();

	public Player Player { get; private set; }

	public override void _Ready() {
		Player = GetNode<Player>("Player");
	}

	public override void _PhysicsProcess(float delta) {
		ProcessPlayerState(Player, delta);
		MovePlayer(Player, delta);
		foreach( var node in GetTree().GetNodesInGroup("Door") )
			if( node is Door door )
				ProcessDoorInteraction(Player, door, delta);
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

		if( Input.IsActionJustPressed("interact") )
			_input.Set(ButtonKind.Interact);
		else if( !Input.IsActionPressed("interact") )
			_input.Reset(ButtonKind.Interact);
	}

	private void MovePlayer(Player player, float delta) {
		float inputX = _input.X;
		if( !Mathf.IsZeroApprox(inputX) ) {
			player.MoveX(inputX, delta);
			player.Face(inputX);
		}
		if( IsStartingJump(player) ) {
			player.Jump(delta);
			//holding jump
		}
		else if( IsStartingMidairJump(player) ) {
			player.MidAirJump(delta);
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
		return Input.IsActionJustPressed("jump") && player.IsGrounded && player.JumpCount == 0;
	}

	private bool IsStartingMidairJump(Player player)
		=> Input.IsActionJustPressed("jump") && !player.IsGrounded
		&& player.JumpCount < 2 && player.Soap >= player.MidairJumpSoapCost;

	private void ProcessPlayerState(Player player, float delta) {
		player.IsSliding = _input.Get(ButtonKind.Slide);
		var previousState = player.State;
		if( IsMoving(player) && player.State != PlayerState.Moving ) {
			player.EnterState(PlayerState.Moving);
		}
		else if( IsIdle(player) && player.State != PlayerState.Idle ) {
			player.EnterState(PlayerState.Idle);
		}
		else if( IsFalling(player) && player.State != PlayerState.Freefall ) {
			player.EnterState(PlayerState.Freefall);
		}
		else if( IsJumping(player) && player.State != PlayerState.Jumping ) {
			player.EnterState(PlayerState.Jumping);
		}
		if( player.State == previousState ) {
			player.UpdateState(delta);
		}
		else {
			if( previousState == PlayerState.Jumping )
				_input.Reset(ButtonKind.Jump);
		}
	}

	private void ProcessDoorInteraction(Player player, Door door, float delta) {
		if( _input.Get(ButtonKind.Interact) && door.OverlapsBody(player) ) {
			_input.Reset(ButtonKind.Interact);
			var destination = door.Destination;
			if( destination != null ) {
				var destinationDoor = door.GetNode<Door>(destination);
				if( destinationDoor != null )
					player.Position = new Vector2(
						destinationDoor.Position.x,
						destinationDoor.Position.y + (player.Position.y - door.Position.y)
					);
			}
		}
	}
}