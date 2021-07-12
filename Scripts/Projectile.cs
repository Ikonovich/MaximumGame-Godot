using System;
using Godot;

namespace MaxGame {


	public class Projectile : RigidBody { 


		[Export]
		public float ProjectileImpulse = 10.1f;

		[Export]
		public float ProjectileDamage = 15.0f;

		[Export]
		public float FlightTimeMax = 2000.0f;


		// Stores whether or not the launching impulse has been applied.
		private bool Launched = false;

		private float FlightTimeCurrent = 0.0f;

		private bool CollisionDetected = false;

		private Area HitBox;


		public override void _Ready() {

			HitBox = GetNode<Area>("HitBox");
			HitBox.Connect("body_entered", this, "Collided");

		}

		public override void _PhysicsProcess(float delta) {

			
			FlightTimeMax += delta;


			Vector3 launchImpulse = new Vector3(Transform.basis.z * ProjectileImpulse);


			AddCentralForce(launchImpulse);

			if (FlightTimeCurrent >= FlightTimeMax) {

				QueueFree();
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
