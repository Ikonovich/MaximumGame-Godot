using System;
using Godot;

namespace MaxGame.Units.Control {

	//<summary>
	// This class handles all movement related behaviors for the unit that is set as it's parent.
	// It is called by various states as they seek to achieve their goals.
	// It does not effect what state the unit is in directly, but the unit controller may utilize
	// information from this class to decide what state to transition to.
	// The Unit-specific variables used by this class, such as Mass, are stored in the parent Unit.
	//</summary>


	public class MovementController : Node {


		public Unit Parent;


		public string StateName = "Test name";

		// <remarks>
		// Stores the distance from a point that is acceptable to consider it reached.
		//
		// </remarks>
		public float DistanceMargin = 5.0f;

		public float Accel = 1.0f;

		// <remarks>
		// A factor to control how powerful the steering behavior is. Lower values turn more slowly.
		// </remarks>
		public float SteeringFactor = 0.1f;

		
		// <remarks>
		// Stores the distance from a point that is acceptable to consider it reached.
		//
		// </remarks>

		public float FollowDistance = 20.0f;



		public override void _Ready() {

			Parent = (Unit)GetParent().GetParent().GetParent();


		}

		public override void _PhysicsProcess(float delta) {


		}

		// <remarks>
		// This returns a bool letting the state machine know that the requested point has been reached.
		// </remarks>

		public void ApplyMovement(Vector3 InputVector) {

			if (InputVector.Length() > 0.01) {

				
				//Console.WriteLine("Input vector is non-zero");
				
				InputVector = InputVector.Normalized();

				
				// 	Multiplying the direction vector times acceleration and adding it to the velocity vector.

				Parent.Velocity += (InputVector * Accel);
			
			}
			
			// Code handling deceleration

			// First, we check to see if the unti is a ground unit.
			// A different Deaccel factor applies to non ground units.

			// Next we normalize the current velocity vector, subtracts the normalized input
			// vector, and applies deceleration to the result.
			// Effectively, only applies movement resistance in diirections
			// that the player is not moving.

			
			if (Parent.IsGroundUnit == true) {  

				Parent.Velocity -= ((Parent.Velocity.Normalized() - InputVector.Normalized()) * 0.7f);
				
				// Checks to see if velocity is low enough to force the unit to a standstill.

				if (Parent.IsOnFloor() == false) {

					Parent.Velocity.y += (-3.0f);
				}

				if (Parent.Velocity.Length() < 0.1f) {

					Parent.Velocity = Vector3.Zero;
				}
			
			}
			else {

				Parent.Velocity -= ((Parent.Velocity.Normalized() - InputVector.Normalized()) * 0.3f);

				if ((Parent.Velocity.Length() < 0.3f) && (InputVector == Vector3.Zero)) {

					Parent.Velocity = Vector3.Zero;

				}

			}
			if (Parent.Velocity.Length() > Parent.MaxSpeed) {

			Parent.Velocity = Parent.Velocity.Normalized() * Parent.MaxSpeed;
			}
			
			Parent.Velocity = Parent.MoveAndSlideWithSnap(Parent.Velocity, Parent.GetFloorNormal(), Vector3.Up, true, 1, 1, true);
			
			
		}

		public void  MoveToPoint(Vector3 point) {


			//Console.WriteLine("Moving to " + point.ToString());

			Vector3 steeringVector = new Vector3(Vector3.Zero);
			Vector3 movementVector = new Vector3(Vector3.Zero);


			float distance = (point - Parent.GlobalTransform.origin).Length();

			
			if (distance > DistanceMargin) {


				Vector3 desiredVector = (point - Parent.Translation).Normalized() * Parent.MaxSpeed;

				if (Parent.Velocity.Length() <= 1) {
					
					movementVector = new Vector3(0,0, -10) * SteeringFactor;

				}
				else {
					steeringVector = (desiredVector - Parent.Velocity) * SteeringFactor;

					
					movementVector = Parent.Velocity + steeringVector;
				}
				

				//}
					// Using the previous velocity to establish a gradient between the current vector
					// and the new vector.


				if (CollisionDetected(movementVector) == false) {

					ApplyMovement(movementVector);
				}

			}
		}
				
		
		public void MoveToFollow(Spatial target) {

			 Vector3 desiredVector = (target.GlobalTransform.origin - Parent.GlobalTransform.origin).Normalized() * Parent.MaxSpeed;

			// Using the previous velocity to establish a gradient between the current vector
			// and the new vector.

			Vector3 movementVector = desiredVector - Parent.Velocity;

			float distance = (target.GlobalTransform.origin - Parent.GlobalTransform.origin).Length();

			if (distance > FollowDistance) {

				if (CollisionDetected(movementVector) == false) {

					ApplyMovement(movementVector);
				}
			}


		}

		public void MoveToAttack(Spatial target) {

			 Vector3 desiredVector = (target.GlobalTransform.origin - Parent.GlobalTransform.origin).Normalized() * Parent.MaxSpeed;

			// Using the previous velocity to establish a gradient between the current vector
			// and the new vector.

			Vector3 movementVector = desiredVector - Parent.Velocity;

			float distance = (target.GlobalTransform.origin - Parent.GlobalTransform.origin).Length();

			// <remarks>
			// Seeks to place the unit within 90% of the weapon range of the parent object.
			// This means that the target moving won't constantly take it in and out of weapoon
			// range.
			// </remarks>
			if (distance > Parent.Range * 0.9f) {

				if (CollisionDetected(movementVector) == false) {

					ApplyMovement(movementVector);
				}
			}


		}

		public void HandleCollision () {


		}

		public bool CollisionDetected(Vector3 input) {

			return false;
		}
	}
}
