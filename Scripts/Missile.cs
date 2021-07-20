using System;
using Godot;
using MagicSmoke;

namespace MaxGame {

	public class Missile : RigidBody {


		// Determines how long the missile will continue to apply an impulse to it's rear section.
		// Basically simulates running out of fuel.

		[Export]
		public float MaxFlightTime = 50.0f;

		[Export]
		public float ProjectileDamage = 15.0f;

		[Export]
		public float ProjectileImpulse = 20.0f;

		// <remarks>
		// Determines how far the missile is away from it's target point when it enters
		// the terminal descent phase.
		// </remarks> 

		protected float TerminalDistance = 30.0f;


		private float FlightCountdown;

		
		// Governs the amount of damage done by the missile.
		[Export]
		float Damage = 15.0f;

		// Keeps track of the missile's target item, if such cxists.
		protected Spatial TargetItem;
		
		// Keeps track of the missile's target point, if such cxists.
		protected Vector3 TargetPoint;

		// Used to look at the target and determine the missile orientation
		protected RayCast TargetRay;

		// Keeps track of whether or not the initial launching impulse has already been applied to the missile.
		private bool Launched = false;
		
		// Keeps track of whether or not this has already had the Y axis locked.
		private bool HasLocked = false;

		private bool CollisionDetected = false;

		private Area HitBox;
		
		// Determines how long the larger launch impulse lasts before it switches to the flight impulse.

		protected float LaunchBurn = 2.0f;
		


		public override void _Ready() {


			HitBox = GetNode<Area>("HitBox");
			HitBox.Connect("body_entered", this, "Collided");

			FlightCountdown = MaxFlightTime;

			TargetRay = GetNode<RayCast>("TargetRay");
		}

		public override void _PhysicsProcess(float delta) {



			AxisLockAngularY = true;

			if (TargetItem != null) {

				TargetPoint = TargetItem.GlobalTransform.origin;
			}
			if (TargetPoint != null) {

				Vector3 launchImpulse = new Vector3(Transform.basis.z * ProjectileImpulse);


					
				if (LaunchBurn > 0) {
					Console.WriteLine("First target point " + TargetPoint.ToString());

					AddCentralForce(launchImpulse);

					LaunchBurn -= delta;
			
				}
				
				float distance = (TargetPoint - GlobalTransform.origin).Length();

				Vector3 targetPoint = Vector3.Zero;

				if (distance > 60) {
					targetPoint = TargetPoint + new Vector3(0.0f, 30.0f, 0.0f);
				}
				else {
					AxisLockLinearY = false;
					targetPoint = TargetPoint;
					AddCentralForce(launchImpulse);
				}
				
				Vector3 target = targetPoint.Normalized();

				FlightCountdown -= delta;


				
			
				TargetRay.LookAt(targetPoint, Vector3.Up);
				Vector3 forward = -TargetRay.GlobalTransform.basis.z;


				Vector3 rotation = Rotation - TargetRay.Rotation;

				// Locks the rotation into place once it reaches an appropriate value.

				
				Console.WriteLine(target.ToString());

				
				// Handles the initial launch rotation

				if ((Translation.y > 35) && (HasLocked == false)) {

					AxisLockLinearY = true;
					HasLocked = true;
				}


				if ((FlightCountdown < 48) && (FlightCountdown > 47)) {
					Console.WriteLine(TargetPoint.ToString());

					Vector3 torque = Vector3.Zero;
					
					torque = new Vector3(target.z, target.y, -target.x) * 1f;
					
					AddTorque(torque);
					AddCentralForce(forward * 5.0f);



				}
				else if (FlightCountdown < 47) {

					Vector3 torque = new Vector3(rotation.x, rotation.y, rotation.z) * 0.1f;
					AddTorque(torque);
					AddCentralForce(forward * 5.0f);


				}

			
				if (FlightCountdown <= 0) {
					QueueFree();
				}
			}

		}

		public void SetTarget(IInteractable targetItem) {

			TargetItem = (Spatial)targetItem;
		}
		
		public void SetTarget(Vector3 targetPoint) {

			Console.WriteLine("Target point set " + targetPoint.ToString());
			
			TargetPoint = targetPoint;
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
