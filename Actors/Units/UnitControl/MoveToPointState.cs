using System;
using Godot;

namespace MaxGame.Units.Control {


	public class MoveToPointState : MachineState {


		public override string StateName { get; } = "MoveToPoint";

		public override Unit Parent { get; set; }


		protected Vector3 TargetPoint;

		protected MovementController MovementController;

		public override void _Ready() {


			MovementController = GetNode<MovementController>("../MovementController");
		}

		  // <remarks>
		// Called when moving from this state to a new state.
		// </remarks>
		public override void TransitionState(MachineState newState) {

			newState.BeginState();
		}


		// <remarks>
		// Called when running this state during a physics frame.
		// </remarks>
		public override void RunState() {


			float distance = (Parent.GlobalTransform.origin - TargetPoint).Length();
			if (distance > MovementController.DistanceMargin) {

				MovementController.MoveToPoint(TargetPoint);
				Console.WriteLine("MoveToPoint state running");


			}
			else {
				Console.WriteLine("Moving to point complete, returning to idle");
				EmitChangeState("Idle");
			}

		}


		// <remarks>
		// Called when entering this state to setup any necessary parameters.
		// </remarks>
		public override void InitializeState() {

			TargetPoint = Parent.TargetPoint;
			Console.WriteLine("Target set to " + TargetPoint.ToString());
		}

		// <remarks>
		// Called when moving into this state.
		// </remarks>
		public override void BeginState() {

			Console.WriteLine("MoveToPoint state is beginning");


			InitializeState();

		}
		// <remarks>
		// Called when leaving this state to deinitialize, if necessary.
		// </remarks>
		public override void EndState() {


		}


		// <remarks>
		// Called when leaving this state reaches a state transition point. Intended to emit the
		// ChangeState signal
		// </remarks>

		public override void EmitChangeState(string stateName) {

			EmitSignal(nameof(ChangeState), stateName);

		}
	}
}
