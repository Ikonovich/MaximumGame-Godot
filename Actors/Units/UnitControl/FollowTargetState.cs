using System;
using Godot;

namespace MaxGame.Units.Control {


	public class FollowTargetState : MachineState {


		public override string StateName { get; }= "FollowTarget";

		public override Unit Parent { get; set; }



		Spatial Target;

		protected MovementController MovementController;



		public override void _Ready() {

			MovementController = GetNode<MovementController>("../MovementController");
			
		}

		  // <remarks>
		// Called when moving from this state to a new state.
		// </remarks>
		public override void TransitionState(MachineState newState) {

			Console.WriteLine("Leaving FollowTarget state");

			newState.BeginState();

		}			



		// <remarks>
		// Called when running this state during a physics frame.
		// </remarks>
		public override void RunState() {

			if (Target == null) {

				EmitChangeState("Idle");

			}
			else {

				MovementController.MoveToFollow(Target);
			}

		}


		// <remarks>
		// Called when entering this state to setup any necessary parameters.
		// </remarks>
		public override void InitializeState() {
			Target = Parent.CurrentTarget;

		}

		// <remarks>
		// Called when moving into this state.
		// </remarks>
		public override void BeginState() {

			Console.WriteLine("FollowTarget state is beginning");

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
