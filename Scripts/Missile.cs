using System;
using Godot;
using MagicSmoke;

namespace MaxGame {

	public class Missile : RigidBody {


		// Determines how long the missile will continue to apply an impulse to it's rear section.
		// Basically simulates running out of fuel.

		[Export]
		public float MaxFlightTime = 20.0f;

		[Export]
		public float ProjectileDamage = 15.0f;

		[Export]
		public float ProjectileImpulse = 15.0f;


		private float FlightCountdown;

		
		// Governs the amount of damage done by the missile.
		[Export]
		float Damage = 15.0f;

		// Keeps track of the missile's target. 
		protected Spatial Target;

		// Used to look at the target and determine the missile orientation
		protected RayCast TargetRay;

		// Keeps track of whether or not the initial launching impulse has already been applied to the missile.
		private bool Launched = false;

		private bool CollisionDetected = false;

		private Area HitBox;



		public override void _Ready() {


			HitBox = GetNode<Area>("HitBox");
			HitBox.Connect("body_entered", this, "Collided");

			FlightCountdown = MaxFlightTime;

			TargetRay = GetNode<RayCast>("TargetRay");

			Target = GetTree().Root.GetNode<Spatial>("Node/Structures/RocketPad");


		}

		public override void _PhysicsProcess(float delta) {

			TargetRay.LookAt(Target.GlobalTransform.origin, Vector3.Up);

			Console.WriteLine("Missile exists");

			FlightCountdown -= delta;

			// Handles the initial launch

			Vector3 launchImpulse = new Vector3(Transform.basis.z * ProjectileImpulse);


			AddCentralForce(launchImpulse);
			
			if (FlightCountdown <= 18.5 && FlightCountdown >= 17.8) {

				Vector3 torque = TargetRay.Rotation.Normalized();
				AddTorque(torque);
			}
			else if (FlightCountdown <= 17.8) {


			}
			
			if (FlightCountdown <= 0) {
				QueueFree();
			}



		}

		public void Collided(Godot.Object body) {

			if (CollisionDetected = false) {

				PlayableUnit bodyActual = (PlayableUnit)body;

				bodyActual.ProjectileHit(Damage, GlobalTransform);

				CollisionDetected = true;
				QueueFree();
			}

		}
	}
}
