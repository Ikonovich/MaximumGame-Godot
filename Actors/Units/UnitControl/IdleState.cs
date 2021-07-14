using System;
using Godot;

namespace MaxGame.Units.Control {

	public class IdleState : MachineState {


		public override string StateName { get; }= "Idle";

		
		// <remarks>
		// Stores the Unit that this state is responsible for the behavior of.
		// Set from the unit controller.
		// </remarks>

		public override Unit Parent { get; set; }



		// <remarks>
		// This parameter determines how far away the unit will travel from the position
		// where it became idle, in order to attack an enemy. 
		// </remarks>
		protected float AggressiveDistance = 5.0f;

		// <remarks>
		// This stores the position where the unit entered the idle state.
		// After pursuing an enemy in the aggressive stance a certain distance or until it is destroyed,
		// the unit will return to this location.
		//</remarks>

		protected Vector3 IdlePosition;


		// <remarks>
		// Stores the movement controller..
		// </remarks>
		protected MovementController MovementController;


		public override void _Ready() {

			MovementController = GetNode<MovementController>("../MovementController");

		}


		public override void RunState() {

			

			// Checks to see if the unit is in the aggressive stance and if it has detected a target.

			if (Parent.GetStance() == "Aggressive") {

				Spatial target = Parent.CurrentTarget;

				if (target != null) {
				// Gets the distance from IdlePosition and distance to the target.

					float distanceFromIdle = (IdlePosition - Parent.GlobalTransform.origin).Length();

					float targetDistance = (Parent.GlobalTransform.origin - target.GlobalTransform.origin).Length();

					if ((distanceFromIdle < AggressiveDistance) && (targetDistance > Parent.Range)) {

						MovementController.MoveToAttack(target);
						
					}
				}

			}
		}

		public override void BeginState() {

			InitializeState();
		}

		public override void EndState() {
			
		}


		public override void InitializeState() {

			IdlePosition = Parent.GlobalTransform.origin;
		}

		public override void TransitionState(MachineState newState) {

			EndState();
			newState.BeginState();
		}

		 public override void EmitChangeState(string stateName) {

			 EmitSignal(nameof(ChangeState), stateName);
		 }
	}
}
