using System;
using Godot;

namespace MaxGame.Unit.Control {

    public class IdleState : MachineState {

        public string Name = "Idle";


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
        // Stores the Unit that this state is responsible for the behavior of.
        // Set from the unit controller.
        // </remarks>
        public Unit Parent;

        // <remarks>
        // Stores the movement controller.
        // </remarks>
        protected MovementController MovementController;


        public override void _Ready() {

            

        }


        public override void RunState() {

            // Checks to see if the unit is in the aggressive stance and if it has detected a target.

            if ((Parent.GetStance() == "Aggressive") {

                Spatial target = Parent.CurrentTarget;
                // Gets the distance from IdlePosition and distance to the target.

                Vector3 distanceFromIdle = (IdlePosition - Parent.GlobalTransform.origin).Length();

                Vector3 targetDistance = (Parent.GlobalTransform.origin - target.GlobalTransform.origin).Length();

                if (distance < AggressiveDistance) {

                    
                }

            }
        }

        public override void BeginState() {

            Initialize();
        }

        public override void Initialize() {

            IdlePosition = Parent.GlobalTransform.origin;
        }
    }
}