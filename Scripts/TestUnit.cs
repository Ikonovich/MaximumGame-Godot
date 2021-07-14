using System;
using System.Collections.Generic;
using MagicSmoke;
using Godot;
using MaxGame.Units.Control;

namespace MaxGame {

	// <summary>
	// This class is used to test new Unit behaviors without modifying the Unit or PlayableUnit
	// classes. It inherits PlaybleUnit and only overrides methods to test new behaviors.

	public class TestUnit : PlayableUnit {
		

		// Tracks whether or not this unit is moving.
		public bool IsMoving = false;
		
		// Experimental Movement controller
		MovementController MovementController;

		// Target point
		Vector3 TargetPoint;


		public override void _Ready() {

			MovementController = GetNode<MovementController>("UnitPackage/MovementController");
			MovementController.Parent = this;

			Navigation = GetTree().Root.GetNode<Navigation>("Node/Terrain/Spatial/Navigation");

			
			SignalGenerator = GetTree().Root.GetNode<SignalGenerator>("Node/SignalGenerator");
			//SignalGenerator.Connect("CloseMenu", this, nameof(CloseMenu));
			SignalGenerator.Connect("MenuOpened", this, nameof(OpenMenu));


			Camera = GetNode<FirstPersonCamera>(CameraPath);
			TargetRay = GetNode<RayCast>(TargetRayPath);
			Selector = GetNode<RaySelector>(SelectorPath);
			Head = GetNode<Spatial>(HeadPath);
			WeaponMount = GetNode<Spatial>(WeaponMountPath);
			TargetAnimation = GetNode<Tween>("TargetAnimation");

			
			HoverButton = GetNode<HoverButton>("HoverButton");
			HoverButton.Hide();



			Selector.Owner = this;

			//Initialize velocity vector
			Velocity = new Vector3();

		}		

		public override void _PhysicsProcess(float delta) {
			if ((IsPreviewScene == false) && (IsReady == false)) {

				Setup();

			}

			if (IsPreviewScene == false) {
				InputVector = new Vector3();

				if (IsPlayer) {

					_ProcessInput(delta);
					Selector.CursorCheck();

				}
				else {

					//_ProcessPathfinding(delta);
					//_ProcessTargeting(delta);

				}

				
				_ProcessMovement(delta);
				_ProcessCamera();


				// Handles disapeparing unit on death

				if (IsDead == true) {

					DeathCountdown -= delta;

					if (DeathCountdown <= 0) {

						QueueFree();
					}

				}
			}
		}
	
		public override void _ProcessMovement(float delta) {

			if (TargetPoint != Vector3.Zero) {
				MovementController.MoveToPoint(TargetPoint);
			}

		}

		public void SetTarget(Vector3 targetPoint) {

			TargetPoint = targetPoint;

		}


	}
}

