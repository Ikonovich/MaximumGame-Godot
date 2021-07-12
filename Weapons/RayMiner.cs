using System;
using System.Collections.Generic;
using Godot;
using MagicSmoke;
using MaxGame;

namespace MaxGame {

	public class RayMiner : Weapon {


		// Stores the scene for the visual effect of the ray.
		private PackedScene RayMeshScene;

		// Determines how long the firing texture displays for. 
		private float DisplayTime = 0.15f;

		// Keeps track of the display timer.
		private float DisplayCountdown = 0.0f;

		// Stores the instantiated visual effect of the ray.
		private MeshInstance RayMesh;

		private CylinderMesh Mesh;


		// Stores the efficiency of the mining laser
		float Efficiency = 1.0f;



		
		public override void _Ready() {



			
			// Setting up the raymesh scene
			RayMeshScene = (PackedScene)ResourceLoader.Load("res://Weapons/Effects/RayMinerMesh.tscn");

			RayMesh = (MeshInstance)RayMeshScene.Instance();

			AddChild(RayMesh);


			FireDelay = 0.13f;

			Damage = 10; 
			
			GameController = GetTree().GetRoot().GetNode<GameController>("Node/GameController");
			
			Ray = GetNode<RayCast>("RayCast");



			DebugRenderer = GetNode<DebugRenderer>(DebugRenderer.ComponentPath);



		}

		public override void _PhysicsProcess(float delta) {

			if (DelayCountdown > 0) {

				DelayCountdown -= delta;
			}

			if (DisplayCountdown > 0) {
				DisplayCountdown -= delta;

				if (DisplayCountdown <= 0) {

					Mesh.Height = 0;
					RayMesh.Mesh = Mesh;
					//RemoveChild(RayMesh);
				}
			}
		}

		// Handles shooting the weapon and causing damage

		public override void Shoot() {

			if (DelayCountdown <= 0) {

				Ray = GetNode<RayCast>("RayCast");

				// Creating a modifiable mesh instance by pulling it from the mesh scene.
				
				Mesh = (CylinderMesh)RayMesh.Mesh;

				// Used to track the calculated length of the ray
				float RayLength;



				Ray.ForceRaycastUpdate();

				///// Debug Renderer

				Vector3 start = Ray.GlobalTransform.origin;
				Vector3 end = Ray.GlobalTransform.Xform(Ray.CastTo);


				DebugRenderer.QueueLineSegment(start, end, Colors.Red, Colors.Red, duration: 1000);


				////// End Debug Renderer


				if (Ray.IsColliding()) {

					
					// Handles unit selection

					Godot.Object body = Ray.GetCollider();
					Vector3 TargetPoint = Ray.GetCollisionPoint();
					
					// Setting the length of the ray effect.
					// Also setting the origin of the ray effect to the halfway point between the 
					// shooter and the target.

					RayLength = (GlobalTransform.origin - TargetPoint).Length();
					RayMesh.Translation = Translation + new Vector3(0, 0, -RayLength / 2);



					if (body is ResourceDeposit) {  
		
						ResourceDeposit deposit = (ResourceDeposit)body;

						int collected = deposit.Harvest(Efficiency);

						GameController.UpdateResource(deposit.ResourceType, collected, TeamID);

					}


				}
				else {
					
					RayLength = 100;
					RayMesh.Translation = Translation + new Vector3(0, 0, -RayLength / 2);

				}


				Mesh.Height = RayLength;
				RayMesh.Mesh = Mesh;

				DelayCountdown = FireDelay;
				DisplayCountdown = DisplayTime;	
			}
		}

		public void GetTargetPoint() {

			Ray.ForceRaycastUpdate();

			Vector3 TargetPoint = Ray.GetCollisionPoint();


		}

		public void Deselect() {

			if (!(Target is null)) {

				Target.Deselected();

			}
		}
	}
}
