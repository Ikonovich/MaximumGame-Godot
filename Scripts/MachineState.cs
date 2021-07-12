using System;
using Godot;

namespace MaxGame {

    //<summary>
    //  This is a generic state that permits generic interfacing with a StateController node.
    //
    //
    //</summary>

    public abstract class MachineState : Node {


        [Signal]
        public delegate void ChangeState(string stateName);

        // <remarks>
        // Stores the name of this state.
        // Used to access the state in the Controller's state dictionary.
        // </remarks>
        public abstract string Name;


        // <remarks>
        // Called when moving from this state to a new state.
        // </remarks>
        public abstract void TransitionState(MachineState newState);


        // <remarks>
        // Called when running this state during a physics frame.
        // </remarks>
        public abstract void RunState();


        // <remarks>
        // Called when entering this state to setup any necessary parameters.
        // </remarks>
        public abstract void InitializeState();

        // <remarks>
        // Called when moving into this state.
        // </remarks>
        public abstract void BeginState();

        // <remarks>
        // Called when leaving this state to deinitialize, if necessary.
        // </remarks>
        public abstract void EndState();
    }
}