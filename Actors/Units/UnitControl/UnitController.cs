using System;
using System.Collections.Generic;
using Godot;
using MaxGame;

namespace MaxGame.Unit.Control {

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
        protected Dictionary<string, MachineState> StateDict = new Dictionary<string, GenericState>();

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
        protected Unit Parent;

        public override void _Ready() {

            Parent = GetParent();
            ActiveState = DefaultState;

            for (int i = 0; i < GetChildCount(), i++) {

                var child = GetChild(i);
                if (child is MachineState state) {

                    state.Connect("ChangeState". this, "ChangeState");
                    state.Parent = Parent;
                    StateDict.Add(state.Name, state);

                }
            }
        }

        // <remarks>
        // Calls the currently active state to run during every physics cycle.
        //
        // </remarks>

        public override void _Process(float delta) {

            ActiveState.RunState();
        }

        // <remarks>
        // This method calls the active state to conduct a state transition.
        // 

        public override void ChangeState(string stateName) {

            MachineState newState = StateDict[stateName];

            ActiveState.TransitionState(newState);
        }


        public string GetStance() {

            return Parent.Stance;
        }

    }

}