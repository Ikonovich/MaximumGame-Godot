using System;
using System.Collections.Generic;
using Godot;
using MaxGame;

namespace MaxGame.Units.Control {

	//<summary>
	// This controller class handles changing states for individual units.
	// When another state reaches a change-of-state point, it emits a signal that 
	// instructs this Controller to direct a state change.
	// </summary>


	public class UnitController : Node {


		// <remarks>
		// Used to store a list of all states available to this controller.
		// For the UnitController, this means a list of all child nodes.
		// </remarks>
		protected Dictionary<string, MachineState> StateDict;

		// <remarks>
		// Used to store the current state of the object.
		// If at any point during runtime this becomes null, something terrible has happened.
		// </remarks>
		protected string ActiveState;

		//<remarks> 
		// Used to store the default object state name, which is set when the object is created.
		// Typically, this will be idle.
		//</remarks>
		protected string DefaultState = "Idle";

		//<remarks>
		// Stores the parent unit of this control structure.
		// </remarks>
		public Unit Parent;

		public override void _Ready() {

			StateDict = new Dictionary<string, MachineState>();

			ActiveState = DefaultState;

			for (int i = 0; i < GetChildCount(); i++) {

				
				Parent = (Unit)GetParent().GetParent();
				var child = GetChild(i);
				if (child is MachineState state) {

					Console.WriteLine("Unit Controller child " + i.ToString() + " " + state.StateName);
					state.Connect("ChangeState", this,"TransitionState");
					state.Parent = Parent;
					StateDict.Add(state.StateName, state);

				}


			}
		}

		// <remarks>
		// Calls the currently active state to run during every physics cycle.
		//
		// </remarks>

		public override void _Process(float delta) {


			MachineState activeState = StateDict[ActiveState];
			activeState.RunState();

		}

		// <remarks>
		// This method calls the active state to conduct a state transition.
		// 

		public void TransitionState(string stateName) {

			Console.WriteLine("Controller is transitioning the state");

			MachineState newState = StateDict[stateName];
			MachineState activeState = StateDict[ActiveState];

			activeState.TransitionState(newState);

			ActiveState = stateName;
		}


		public void TargetAssigned(IDestructible target) {

			if (target.TeamID == Parent.TeamID) {

				TransitionState("FollowTarget");

			}
			else {
				TransitionState("AttackTarget");
			}
		}

		public string GetStance() {

			return "Aggessive";
			//return Parent.Stance;
		}





	}

}
