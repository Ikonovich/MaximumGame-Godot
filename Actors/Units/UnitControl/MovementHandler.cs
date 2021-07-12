using System;
using Godot;

namespace MaxGame.Unit.Control {

    //<summary>
    // This class handles all movement related behaviors for the unit that is set as it's parent.
    // It is called by various states as they seek to achieve their goals.
    // It does not effect what state the unit is in directly, but the unit controller may utilize
    // information from this class to decide what state to transition to.
    // The Unit-specific variables used by this class, such as Mass, are stored in the parent Unit.
    //</summary>


    public class MovementHandler : Node {


        public Unit Parent;


        // <remarks>
        // Stores the distance from a point that is acceptable to consider it reached.
        //
        // </remarks>
        public float DistanceMargin = 1.0;

        
        // <remarks>
        // Stores the distance from a point that is acceptable to consider it reached.
        //
        // </remarks>

        public float FollowMargin = 5.0;



        public override void _Ready() {


        }

        public override void _PhysicsProcess(float delta) {


        }

        // <remarks>
        // This returns a bool letting the state machine know that the requested point has been reached.
        // </remarks>

        public void ApplyMovement(Vector3 movementVector) {

            Velocity = MoveAndSlideWithSnap(Velocity, GetFloorNormal(), Vector3.Up, true, 1, 0.5f, false);

        }

        public bool MoveToPoint(Vector3 point) {

            Vector3 movementVector = new Vector3(Vector3.Zero);

            if (Parent.Velocity = Vector3.Zero) {

                Vector3 direction = new Vector3(0, 0, -10);
                movementVector = desiredVector - Parent.Velocity;

            }
            else {

                Vector3 desiredVector = (point - Parent.GlobalTransform.origin).Normalized();

                movementVector = desiredVector - Parent.Velocity;

            }
                // Using the previous velocity to establish a gradient between the current vector
                // and the new vector.


            float distance = (point - Parent.GlobalTransform.origin).Length();

            if (distance > DistanceMargin) {

                if (CollisionDetected(movementVector) == false) {

                    ApplyMovement(movementVector);
                }

                return false;
            }
            else {

                return true;
            }

                
        }

        public void FollowTarget(Spatial target) {

             Vector3 desiredVector = target.GlobalTransform.origin - Parent.GlobalTransform.origin).Normalized();

            // Using the previous velocity to establish a gradient between the current vector
            // and the new vector.

            Vector3 movementVector = desiredVector - Parent.Velocity;

            float distance = (point - Parent.GlobalTransform.origin).Length();

            if (distance > FollowMargin) {

                if (CollisionDetected(movementVector) == false) {

                    ApplyMovement(movementVector);
                }

                return false;
            }
            else {
                return true;
            }


        }

        public void MoveToAttack(Spatial target) {


        }

        public void HandleCollision () {


        }

        CollisionDetected(Vector3 input) {

            return false;
        }

    }
}