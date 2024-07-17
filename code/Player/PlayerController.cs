
using Sandbox.Diagnostics;
using SpriteTools;
using System;

namespace Rose;

public sealed class PlayerController : Component
{
	[Property] public float MoveSpeed { get; set; } = 5000f;
	[Property] public float MaxSpeed { get; set; } = 500f;
	[Property] public float JumpForce { get; set; } = 350;
	[Property] public SpriteComponent Sprite { get; private set; }
	[RequireComponent] private Rigidbody _rb { get; set; }

	private FixedUpdateInputBuffer _fixedInput = new();
	private Vector3 _velocity;
	private float _lastDir;
	private bool _horizontalFlip = false;
	private bool _isAttacking = false;
	private bool _isGrounded;

	protected override void OnStart()
	{
		Sprite.OnBroadcastEvent += OnSpriteAnimationMessage;
		Sprite.OnAnimationComplete += OnSpriteAnimationCompleted;
	}

	private void OnSpriteAnimationCompleted( string animation )
	{
		if ( animation == "attack" )
		{
			_isAttacking = false;
		}
	}

	private void OnSpriteAnimationMessage( string message )
	{
		if ( message == "attack" )
			DoAttack();
	}

	protected override void OnUpdate()
	{
		_fixedInput.Update();

		_velocity = 0;
		var wishMove = new Vector3( 0, Input.AnalogMove.y, 0 ).Normal;

		if ( !_isAttacking )
		{
			if ( !_rb.Velocity.y.AlmostEqual( 0, 1f ) )
				Sprite.PlayAnimation( "walk" );

			if ( wishMove != 0 )
			{
				_lastDir = 0;
			}
			else
				Sprite.PlayAnimation( "idle" );
		}

		_lastDir += Input.AnalogMove.y;
		_horizontalFlip = _lastDir > 0;

		Sprite.SpriteFlags = _horizontalFlip ? SpriteFlags.HorizontalFlip : SpriteFlags.None;

		var addSpeed = _isGrounded ? MoveSpeed : MoveSpeed / 3f;
		var addVelocity = wishMove * addSpeed;
		_velocity += addVelocity;

		var currentvelocity = _rb.Velocity;
		if ( MathF.Abs( currentvelocity.y ) > MaxSpeed )
			_velocity.y = MathF.Sin( currentvelocity.y ) * MaxSpeed;

		_rb.ApplyForce( _velocity );

		// IsGrounded trace
		{
			var radius = 8f;
			var pos = Transform.Position + Vector3.Down * 2.5f;
			var trace = Scene.Trace.Sphere( radius, Transform.Position, pos ).IgnoreGameObjectHierarchy( GameObject ).Run();
			DebugOverlay.Sphere( new Sphere( pos, radius ), color: Color.Red );
			_isGrounded = trace.Hit;
		}

		if ( Input.Pressed( "attack1" ) )
		{
			_isAttacking = true;
			Sprite.PlayAnimation( "attack" );
		}

		if ( Input.Pressed( "jump" ) && _isGrounded )
			_rb.ApplyImpulse( Vector3.Up * JumpForce );

		if ( !_isGrounded && !_isAttacking )
			Sprite.PlayAnimation( "jump" );
	}

	protected override void OnFixedUpdate()
	{
		_fixedInput.FixedClear();
	}

	private void DoAttack()
	{
		var radius = 26f;
		var offset = (_horizontalFlip ? Vector3.Left : Vector3.Right) * 39f;
		var pos = Transform.Position + Vector3.Up * 13.5f + offset;
		var hits = Scene.Trace.Sphere( radius, pos, pos ).IgnoreGameObjectHierarchy( GameObject ).RunAll();
		DebugOverlay.Sphere( new Sphere( pos, radius ), color: Color.Red, time: 5 );

		foreach ( var tr in hits )
		{
			if ( tr.Hit )
			{
				var other = tr.GameObject;
				if ( other.Components.TryGet<Rigidbody>( out var rb ) )
					rb.ApplyImpulse( Vector3.Up * 1300 + offset * 56 );

				Log.Info( tr.GameObject.Name );

			}
		}
	}
}
