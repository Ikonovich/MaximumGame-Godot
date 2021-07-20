using System;
using Godot;

namespace MaxGame {


	public class Projectile : RigidBody { 


		[Export]
		public float ProjectileImpulse = 10.1f;

		// The total amount of accelerated that will be applied when the object is fired, 
		// multiplied by the mass and applied over the launch time.
		[Export]
		public float ProjectileAcceleration = 50.0f;

		[Export]
		public float ProjectileDamage = 15.0f;

		[Export]
		public float FlightTimeMax = 2000.0f;


		// Determines how long the projectile force is applied.
		protected float LaunchTime = 0.1f;
		

		protected bool IsLaunched = false;


		private float FlightTimeCurrent = 0.0f;

		private bool CollisionDetected = false;

		private Area HitBox;


		public override void _Ready() {

			HitBox = GetNode<Area>("HitBox");
			HitBox.Connect("body_entered", this, "Collided");

		}

		public override void _PhysicsProcess(float delta) {

			if (IsLaunched == false) {

			 	ApplyCentralImpulse(ProjectileAcceleration * Mass * -Transform.basis.z);
			 	Console.WriteLine("Applying projectile force");

				IsLaunched = true;
			}


		}

		public void Collided(Godot.Object body) {

			Console.WriteLine("Projectile hit something");


			if (CollisionDetected == false) {

				if (body.HasMethod("ProjectileHit")) {

					PlayableUnit bodyActual = (PlayableUnit)body;

					bodyActual.ProjectileHit(ProjectileDamage, GlobalTransform);
				}

			}
			CollisionDetected = true;
			QueueFree();

		}

	}
}
