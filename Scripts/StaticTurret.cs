using System;
using Godot;
using MagicSmoke;

namespace MaxGame {

    public class StaticTurret : StaticBody, IInteractable, IDestructible, ISensor, IWeaponized {

        [Export]
        public int MaxHealth { get; set; } = 100;

        [Export]
        public int Health { get; set; } = 100;

        // Used to determine the maximum fire rate. 
        [Export]
        public float ReloadTime;

        protected float ReloadCounter;

        // Determines what percentage of the damage the item receives. Defaults to 1 for 100%.

        [Export]
        public float DamageMultiplier { get; set; } = 1;

        [Export]
        public Spatial CurrentTarget { get; set; }


        // Determines which team the object is on. Defaults to 0.
        [Export]
        public int TeamID { get; set; } = 0;

        protected Area TargetPoint;

        protected HealthBar HealthBar;

        protected GameController GameController;

        // Sensor area 
        protected Area DetectionArea; 

        // Debug Renderer object
        protected DebugRenderer DebugRenderer;

        


        public override void _Ready() {

            DebugRenderer = GetNode<DebugRenderer>(DebugRenderer.ComponentPath);
            DetectionArea = GetNode<Area>("DetectionArea");

            DetectionArea.Connect("body_entered", this, "TargetDetected");
            DetectionArea.Connect("body_exited", this, "TargetGone");

            HealthBar = GetNode<HealthBar>("StatusBar");
            HealthBar.UpdateHealth(Health, MaxHealth);


        }

        public override void _PhysicsProcess(float delta) { 
            
        }

        public virtual void Shoot() {

        }

        // The next two methods handle cursor interaction.
        public virtual void Selected() {}

        public virtual void Deselected() {}

        public virtual void CursorInteract() {

        }

        // The next two methods handle target management.
        public virtual void TargetDetected(Area target) {

            if ((CurrentTarget == null) && (target is KinematicBody)) {


                Area targetArea = target;
            }

        }

        public virtual void TargetGone(Area target) {

            if ((CurrentTarget != null) && (CurrentTarget == target)) {
                CurrentTarget = null; 

            }
        }


        public void ProjectileHit(float damage, Transform transform) {

            Health -= (int)(damage * DamageMultiplier);
            HealthBar.UpdateHealth(Health, MaxHealth);
        }
    }
}