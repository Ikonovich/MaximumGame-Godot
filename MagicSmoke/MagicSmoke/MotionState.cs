using Godot;

namespace MagicSmoke {

    /// <summary>
    /// A utility class for simulating velocity and acceleration in three dimensions. Mainly intended for use on kinematic bodies.
    /// </summary>
    public class MotionState {

        protected float maxVelocity, maxAcceleration;
        protected float maxVelocitySquared, maxAccelerationSquared;

        public Vector3 Velocity { get; set; } = Vector3.Zero;
        public Vector3 Acceleration { get; set; } = Vector3.Zero;
        public float MaxVelocity { get => maxVelocity; set { maxVelocity = value; maxVelocitySquared = value * value; } }
        public float MaxAcceleration { get => maxAcceleration; set { maxAcceleration = value; maxAccelerationSquared = value * value; } }
        public float VelocityDecayRate { get; set; } = 1f;
        public float AccelerationDecayRate { get; set; } = 1f;
        public bool UseVelocityDecay {get; set;} = true;
        public bool UseAccelerationDecay {get; set;} = true;
        public bool UseGravity {get; set;} = false;
        public Vector3 Gravity {get; set;} = Vector3.Zero;
        public void ApplyAcceleration(Vector3 accel) {
            Acceleration += accel;
        }

        public MotionState() {
            
        }
        
        public void Update(float dTime) {

            if (Velocity.LengthSquared() <= 0.01f) Velocity = Vector3.Zero;
            else if (Velocity.LengthSquared() > maxVelocitySquared) Velocity = Velocity.LinearInterpolate(Vector3.Zero, 0.01f);

            if (Acceleration.LengthSquared() <= 0.01f) Acceleration = Vector3.Zero;
            else if (Acceleration.LengthSquared() > maxAccelerationSquared) Acceleration = Acceleration.LinearInterpolate(Vector3.Zero, 0.01f);
        

            var gravityFactor = UseGravity ? Gravity : Vector3.Zero;
            
            if(Velocity.LengthSquared() < 0.01f) Velocity = Vector3.Zero;
            if(Acceleration.LengthSquared() < 0.01f) Acceleration = Vector3.Zero;

            if(Acceleration != Vector3.Zero || gravityFactor != Vector3.Zero) {
                Velocity += (Acceleration + gravityFactor) * dTime;
                if(UseAccelerationDecay) Acceleration = Acceleration.LinearInterpolate(Vector3.Zero, AccelerationDecayRate * dTime);
            }
            if (Velocity != Vector3.Zero && UseVelocityDecay) {
                Velocity = Velocity.LinearInterpolate(Vector3.Zero, VelocityDecayRate * dTime);
            }
        }

        public void Reset() {
            Velocity = Vector3.Zero;
            Acceleration = Vector3.Zero;
        }

    }
}