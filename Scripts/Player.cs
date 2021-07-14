using System;
using System.Collections.Generic;
using Godot;
using MagicSmoke;


namespace MaxGame {


	public class Player : PlayableUnit {

		public override void _Ready() {




			SignalGenerator = GetTree().Root.GetNode<SignalGenerator>("Node/SignalGenerator");

			GameController = GetTree().Root.GetNode<GameController>("Node/GameController");
			DebugRenderer = GetNode<DebugRenderer>(DebugRenderer.ComponentPath);



			SelectionHalo = GetNode<Spatial>("SelectionHalo");



			Input.SetMouseMode(Input.MouseMode.Captured);
			Camera = GetNode<FirstPersonCamera>("Pivot/Camera");
			TargetRay = GetNode<RayCast>("Pivot/Camera/TargetRay");
			CrossHair = GetNode<TextureRect>("HUD/CrossHair");
			HUD = GetNode<HUD>("HUD");


			WeaponMount = GetNode<Spatial>("Pivot/WeaponMount");
			Selector = GetNode<RaySelector>("Pivot/Selector");
			Selector.Owner = this;


			Selector.TeamID = TeamID;
			HUD.TeamID = TeamID;

			// Sets the selector as the default weapon and sets the selector's team ID.
			CurrentWeapon = Selector;
			HUD.WeaponSwap(CurrentWeapon.GetIcon());

			// Update the interaction healthbar and the HUD healthbar.

			HealthBar = GetNode<HealthBar>("StatusBar");
			HealthBar.UpdateHealth(Health, MaxHealth);
			HealthBar.Hide();

			// Update HUD health
			HUD.UpdateHealth(Health, MaxHealth);




			// HUDHealthBar = GetNode<TextureProgress>("HUD/HealthBar");
			// HUDHealthBar.UpdateHealth(Health, MaxHealth);
			


			// Set up the hover button
			HoverButton = GetNode<HoverButton>("HoverButton");
			HoverButton.Hide();
			
			// Checks to see if weapon scenes have been set and, if so, casts them to spatial
			// sets them as children of WeaponMount node.


			if (WeaponOneScene != null) {

				WeaponOne = (Weapon)WeaponOneScene.Instance();

				WeaponMount.AddChild(WeaponOne);

				WeaponOne.GlobalTransform = WeaponMount.GlobalTransform;

				WeaponOne.TeamID = TeamID;
				WeaponOne.Owner = this;

			}

			if (WeaponTwoScene != null) {


				WeaponTwo = (Weapon)WeaponTwoScene.Instance();

				WeaponMount.AddChild(WeaponTwo);

				WeaponTwo.GlobalTransform = WeaponMount.GlobalTransform;

				WeaponTwo.TeamID = TeamID;
				WeaponTwo.Owner = this;

			}

			// Start this off as the default player
			SetAsPlayer();

			IsReady = true;
			IsPreviewScene = false;
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
					_ProcessMovement(delta);

				}
				else {

					_ProcessPathfinding(delta);
					_ProcessTargeting(delta);

				}

				
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

	}
}
