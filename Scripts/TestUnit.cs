using System;
using System.Collections.Generic;
using MagicSmoke;
using Godot;

namespace MaxGame {

	// <summary>
	// This class is used to test new Unit behaviors without modifying the Unit or PlayableUnit
	// classes. It inherits PlaybleUnit and only overrides methods to test new behaviors.

	public class TestUnit : PlayableUnit {
		

		// Tracks whether or not this unit is moving.
		public bool IsMoving = false;
		
		// Experimental Movement controller
		MovementHandler MovementHandler;


		public override void _Ready() {

			MovementHandler = GetNode<MovementHandler>("UnitPackage/MovementHandler");

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
	
		public override void _ProcessMovement(float delta) {

			// Stores a temporary target point for testing.

			Vector3 targetPoint = GlobalTransform.origin + new Vector3(20.0f, 0.0f, 15.0f);


			MovementHandler.MoveToPoint(targetPoint);
		}

	}
}

