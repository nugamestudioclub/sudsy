using Godot;
using System;
using System.Collections.Generic;

public class PlayScene : Node2D {
	private readonly InputFrame _input = new InputFrame();

	public Player Player { get; private set; }

	public Soap Soap { get; private set; }

	public SFX SFX { get; private set; }


	private float _timeUntilMoveEmission = 0;

	[Export]
	private float _moveEmissionInterval = 0.15f;
	[Export]
	private float _slideEmissionInterval = 0.075f;

	private Vector2 _uiOffset;

	public Camera2D Camera { get; private set; }

	public UI UI { get; private set; }

	[Export]
	private float _minCleanSpeed = 5f;

	[Export]
	public float BaseCleanRate { get; private set; } = .05f;

	[Export]
	private float _moveCleanMod = .5f;

	public float MoveCleanRate => BaseCleanRate * _moveCleanMod;

	[Export]
	private float _slideCleanMod = 1;

	public float SlideCleanRate => BaseCleanRate * _slideCleanMod;

	[Export]
	private float _midairJumpCleanMod = 100;

	public float MidairJumpCleanRate => BaseCleanRate * _midairJumpCleanMod;


	private List<Dirt> _dirts = new List<Dirt>();

	public float TotalDirtAmount { get; private set; }
	public float CurrentDirtAmount { get; private set; }

	private List<Door> _doors = new List<Door>();

	private List<SoapDispenser> _soapDispensers = new List<SoapDispenser>();


	[Export]
	private NodePath _startingDoor;

	[Export]
	public float TotalTime { get; private set; }

	public float TimeToLive { get; private set; }

	public override void _Ready() {
		Player = GetNode<Player>("Player");
		Soap = GetNode<Soap>("Soap");
		Camera = GetNode<Camera2D>("Camera2D");
		UI = GetNode<UI>("Camera2D/UI");
		SFX = GetNode<SFX>("SFX");

		TimeToLive = TotalTime;

		foreach( var node in GetTree().GetNodesInGroup("Dirt") )
			if( node is Dirt dirt ) {
				TotalDirtAmount += dirt.Amount;
				_dirts.Add(dirt);
			}
		CurrentDirtAmount = TotalDirtAmount;

		GD.Print($"Starting Dirt {TotalDirtAmount}");
		foreach( var node in GetTree().GetNodesInGroup("Door") )
			if( node is Door door )
				_doors.Add(door);
		Enter(GetNode<Door>(_startingDoor));

		foreach( var node in GetTree().GetNodesInGroup("SoapDispenser") )
			if( node is SoapDispenser sd )
				_soapDispensers.Add(sd);

		DrawSoapUI(Player);
		DrawCleanUI();
		UI.DrawTime(TimeToLive);
	}


	public override void _PhysicsProcess(float delta) {
		ProcessPlayerState(Player, delta);
		ProcessPlayerAnimation(Player, delta);
		foreach( var dirt in _dirts )
			ProcessDirtInteraction(Player, dirt, delta);

		MovePlayer(Player, delta);
		ProcessSoap(Player, delta);

		foreach( var door in _doors )
			ProcessDoorInteraction(Player, door, delta);
		TimeToLive = Mathf.Max(TimeToLive - delta, 0);
		UI.DrawTime(TimeToLive);

		if( TimeToLive <= 0f || CurrentDirtAmount <= 0.1f ) {
			GameManager.Instance.ChangeScene("Credits");
		}
	}

	public override void _Process(float delta) {
		ProcessInput(delta);
	}

	private void Enter(Door door) {
		//Camera.GlobalPosition = door.CameraHook.GlobalPosition;
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

	private bool IsSliding(Player player) {
		return player.IsSliding;

	}

	private bool IsFailingMidairJump(Player player) {
		return Input.IsActionJustPressed("jump")
			&& !player.IsGrounded
			&& player.JumpCount < 1
			&& player.Soap < player.MidairJumpSoapCost;
	}

	private bool IsFailingSlide(Player player) {
		return Input.IsActionJustPressed("slide") && player.Soap < player.SlideSoapCost;
	}

	private bool IsIdle(Player player) {
		return player.IsGrounded && Mathf.IsZeroApprox(player.LinearVelocity.x);
	}

	private bool IsStartingJump(Player player) {
		return Input.IsActionJustPressed("jump") && player.IsGrounded && player.JumpCount == 0;
	}

	private bool IsStartingMidairJump(Player player) {
		return Input.IsActionJustPressed("jump")
			&& !player.IsGrounded
			&& player.JumpCount < 1
			&& player.Soap >= player.MidairJumpSoapCost;
	}

	private bool IsStartingSlide(Player player) {
		return Input.IsActionJustPressed("slide") && player.Soap >= player.SlideSoapCost;
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
				Enter(destinationDoor);
			}
		}
	}

	private void ProcessClean(Dirt dirt, float amount) {
		float currentAmount = dirt.Amount;
		float nextAmount = Math.Max(0, currentAmount - amount);
		float delta = currentAmount - nextAmount;
		if( delta > 0 ) {
			CurrentDirtAmount -= delta;
			dirt.Amount -= delta;
			DrawCleanUI();

		}
	}

	private void ProcessDirtInteraction(Player player, Dirt dirt, float delta) {
		if( !dirt.OverlapsArea(player.Brush) ) return;
		if( player.IsJumpingInMidair ) {
			ProcessClean(dirt, MidairJumpCleanRate);
		}
		else if( player.IsGrounded && Math.Abs(Player.LinearVelocity.x) >= _minCleanSpeed ) {
			ProcessClean(dirt, IsSliding(player) ? SlideCleanRate : MoveCleanRate);
		}


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
			Soap.Jump(player.Feet.GlobalPosition);
			SFX.Play("Jump");
		}
		else if( IsStartingMidairJump(player) ) {
			player.MidairJump(delta);
			player.Soap = Math.Max(player.Soap - player.MidairJumpSoapCost, 0);
			DrawSoapUI(player);
			Soap.MidairJump(player.Feet.GlobalPosition);
			SFX.Play("MidairJump");
		}
		else if( !IsJumping(player) ) {
			_input.Reset(ButtonKind.Jump);
		}
		if( IsStartingSlide(player) ) {
			SFX.Play("Slide");
		}
		else if( IsFailingSlide(player) ) {
			SFX.Play("SoaplessSlide");

		}
		else if( IsFailingMidairJump(player) ) {
			SFX.Play("SoaplessMidairJump");
		}
	}

	private void ShowMoveParticles(Player player) {
		float xOffset = player.Feet.Shape is RectangleShape2D rect ? rect.Extents.x : 0;
		Soap.Move(player.Feet.GlobalPosition, new Vector2(xOffset, 0), player.LinearVelocity);
	}

	private void ShowSlideParticles(Player player) {
		Soap.Slide(player.Feet.GlobalPosition, player.LinearVelocity);
	}

	private void RegenerateSoap(Player player) {
		player.Soap = Math.Min(player.MaxSoap, player.Soap + player.SoapRegenRate);
		DrawSoapUI(player);
	}

	private void ProcessSoap(Player player, float delta) {
		if( _timeUntilMoveEmission <= 0 ) {
			if( IsSliding(player) ) {
				if( player.Soap > 0 ) {
					ShowSlideParticles(player);
					player.Soap = Math.Max(0, player.Soap - player.SlideSoapCost);
					DrawSoapUI(player);
					_timeUntilMoveEmission = _slideEmissionInterval;
				}
				else {
					ShowMoveParticles(player);
					_timeUntilMoveEmission = _moveEmissionInterval;
				}
			}
			else {
				if( player.IsGrounded ) {
					ShowMoveParticles(player);
					_timeUntilMoveEmission = _moveEmissionInterval;

				}
			}

		}
		else {
			_timeUntilMoveEmission = Math.Max(_timeUntilMoveEmission - delta, 0);
		}
		if( !IsSliding(player) ) {
			RegenerateSoap(player);
		}

		float currentSoap = player.Soap;
		float maxSoap = player.MaxSoap;
		if( currentSoap < maxSoap ) {
			foreach( var sd in _soapDispensers ) {
				if( sd.OverlapsBody(Player) ) {
					player.Soap = player.MaxSoap;
					DrawSoapUI(player);
					SFX.Play("CollectSoap");
				}
			}
		}
	}

	private void ProcessPlayerAnimation(Player player, float delta) {
		player.PlayAnimation(player.GetDesiredAnimation());
	}

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

	private void DrawCleanUI() {
		UI.DrawClean(1f - (CurrentDirtAmount / TotalDirtAmount));
	}

	private void DrawSoapUI(Player player) {
		UI.DrawSoap(player.Soap / player.MaxSoap);
	}
}