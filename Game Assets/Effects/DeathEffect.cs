using System;
using Godot;



public class DeathEffect : Spatial {


	// Determines how long the effect plays.   
	float Countdown = 2.0f;

	// Used to access the particles node
	Particles Effect;
	
	public override void _Ready() {

		Effect = GetNode<Particles>("Effect");

		Effect.Emitting = true;

	}

	public override void _PhysicsProcess(float delta) {

		Countdown -= delta;

		if (Countdown <= 0) {

			QueueFree();
		}
	}

}
