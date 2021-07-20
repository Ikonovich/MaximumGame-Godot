using System;
using System.Collections.Generic;
using Godot;
using MagicSmoke;

namespace MaxGame {

	public class RaySelector : Weapon {


		// Keeps track of whether or not the selector has any selected items currently.
		public bool HasSelected = false;
		
		// The universal GameController
		private GameController GameController;

		// The signal generator
		private SignalGenerator SignalGenerator;

		// Used to create a visible selection area, maybe.
		private SurfaceTool Tool;

		// Stores a mesh instance that can be used as a visible selection shape
		private MeshInstance MeshInstance;

		private CubeMesh mesh;

		// Stores the collision area for the MeshInstance
		private Area CollisionArea;

		// Stores the collision shape for the area
		private CollisionShape CollisionShape;

		// Keeps track of whether or not there is a drag and drop selection underway
		private bool IsDragging = false;

		// Keeps track of whether or not there is a building process underway.
		private bool IsBuilding = false;

		// If Building == true, this should contain the current structure to be placed.
		private PhysicsBody BuildItem;

		// Stores the original IBuildable item the BuildItem is derived from.
		private IBuildable Item;

		// Used to keep finalize from setting off from the same click that activates build mode.
		private float BuildTimer = 0.1f;
		private float BuildCountdown = 0.0f;

		// Stores the BuildItem's original translation
		private Vector3 OriginalTranslation;

		// Stores the origin point of the currently active selection area, if there is one.
		private Vector3 SelectionOrigin;

		// Stores the current updated origin point of the active selection area, if there is one
		private Vector3 CurrentOrigin;
		
		private RayCast Ray;

		// Stores the selected bodies and is used to send them to the GameController.
		private Godot.Collections.Array Selected;



		public override void _Ready() {


			GameController = GetTree().Root.GetNode<GameController>("Node/GameController");
			SignalGenerator = GetTree().Root.GetNode<SignalGenerator>("Node/SignalGenerator");

			MeshInstance = GetTree().Root.GetNode<MeshInstance>("Node/Effects/SelectionMeshInstance");
			CollisionArea = GetTree().Root.GetNode<Area>("Node/Effects/SelectionMeshInstance/Area");
			CollisionShape = GetTree().Root.GetNode<CollisionShape>("Node/Effects/SelectionMeshInstance/Area/CollisionShape");

			Selected = new Godot.Collections.Array();

			Ray = GetNode<RayCast>("RayCast");
			

			DebugRenderer = GetNode<DebugRenderer>(DebugRenderer.ComponentPath);

		
		}

		public override void _PhysicsProcess(float delta) {

			if (BuildCountdown > 0) {
				BuildCountdown -= delta;
			}
		}

		public void CursorCheck() {

			

			Ray.ForceRaycastUpdate();

			///// Debug Renderer

			Vector3 start = Ray.GlobalTransform.origin;
			Vector3 end = Ray.GlobalTransform.Xform(Ray.CastTo + Ray.GlobalTransform.origin);

			//DebugRenderer.QueueLineSegment(start, end, Colors.Red, Colors.Red, duration: 1000);


			////// End Debug Renderer

			if (Ray.IsColliding()) {

				if (IsBuilding == true) {

					BuildItem.Translation = OriginalTranslation + Ray.GetCollisionPoint() + new Vector3(0.0f, Item.TerrainYoffset, 0.0f);

				}

				Godot.Object body = Ray.GetCollider();


				if (body is IInteractable) {  
	
					IInteractable bodyActual = (IInteractable)body;

					bodyActual.CursorInteract();

				}
			}

		}

		
		// Functions equivalently to a "Select" function for the RaySelector
		public override void Shoot() {


			Ray.ForceRaycastUpdate();

			///// Debug Renderer

			Vector3 start = Ray.GlobalTransform.origin;
			Vector3 end = Ray.GlobalTransform.Xform(Ray.CastTo);

			DebugRenderer.QueueLineSegment(start, end, Colors.Red, Colors.Red, duration: 1000);


			////// End Debug Renderer


			if (Ray.IsColliding()) {


				// Store the collision point
				Vector3 collisionPoint = Ray.GetCollisionPoint();
				// Handles unit selection

				Godot.Object body = Ray.GetCollider();


				// Used primarily for interacting with in-world buttons.
				if (body is IInteractable) {  
	
					IInteractable bodyActual = (IInteractable)body;

					Selected.Add(bodyActual);
					GameController.Selected(Selected);

				}

				else {


					if (IsDragging == false) {

						Console.WriteLine("Area selection begin");

						
						mesh = new CubeMesh();

						mesh.Size = new Vector3(0, 20, 0);


						Material material = ResourceLoader.Load("res://Game Assets/Materiels/SelectionAreaMaterial.material") as Material;

					
						mesh.SurfaceSetMaterial(0, material);
					

						SelectionOrigin = collisionPoint;
						CurrentOrigin = SelectionOrigin;
						MeshInstance.Translation = CurrentOrigin;

						IsDragging = true;
					}
					else {

						
						CurrentOrigin = SelectionOrigin - ((SelectionOrigin - collisionPoint) / 2);


						// Set the mesh size, including the default height value.

						mesh.Size = ((SelectionOrigin - collisionPoint) + new Vector3(0, 20, 0));

					}

					MeshInstance.Translation = CurrentOrigin;

					//Generates a collision shape
					Shape shape = mesh.CreateConvexShape();
					CollisionShape.Shape = shape;

						
					MeshInstance.Mesh = mesh;
					CollisionShape.GlobalTransform = MeshInstance.GlobalTransform;

					// Prints any bodies this shape is overlapping

					Console.WriteLine(CollisionArea.GetOverlappingBodies());



					Selected = CollisionArea.GetOverlappingBodies();

					GameController.Selected(Selected);
				}
			}
			else {

				Deselect();
			}


		}

		public void RightSelect() {
			
			if (Ray.IsColliding()) {


				// Store the collision point
				Vector3 collisionPoint = Ray.GetCollisionPoint();
				// Handles unit selection

				Godot.Object body = Ray.GetCollider();

				if (body is IInteractable) {  
	
					IInteractable target = (IInteractable)body;
					GameController.RightSelect(target, TeamID);
				}
				else {
					GameController.RightSelect(collisionPoint);
				}
			}
		}

		// Called to end an area selection
		public void Release() {

			Console.WriteLine("Released");

			IsDragging = false;
			// Creates an invisibly sized cubemesh

			CubeMesh cubeMesh = new CubeMesh();
			cubeMesh.Size = new Vector3(0, 0, 0);
			MeshInstance.Mesh = cubeMesh;

			Selected = new Godot.Collections.Array();


		}

		public void Deselect() {

			GameController.Deselected();
			HasSelected = false;

		}

		// This method handles initiating the building process.

		public void Build(IBuildable item) {


			Item = item;
			Item.IsPreviewScene = false;
			Item.TeamID = Owner.TeamID;
			BuildItem = (PhysicsBody)Item;

			SetMeshesTransparent(BuildItem, 0.7f);

			// Disabling ray picking on the building item so it doesn't interfere with placement.
			OriginalTranslation = BuildItem.Translation;

			if (Ray.IsColliding()) {

				Ray.ForceRaycastUpdate();


				// Store the collision point
				Vector3 collisionPoint = Ray.GetCollisionPoint();



				GetTree().Root.AddChild(BuildItem);
				BuildItem.Translation = BuildItem.Translation + collisionPoint + new Vector3(0.0f, Item.TerrainYoffset, 0.0f);


				Console.WriteLine("Item should be added: " + BuildItem.Name);

				
				IsBuilding = true;
				BuildCountdown = BuildTimer;
				// Signals the player that a build has begun.
				SignalGenerator.EmitEnterBuildMode();


			}
		}
		public void FinalizeBuild() {


			if (BuildCountdown <= 0) {

				// Shakes the camera
				Owner.ShakeCamera(0.1, 0.1);


				// Makes the object fully opaque.
				SetMeshesTransparent(BuildItem, 1.0f);

				// Set item collision masks.
				BuildItem.SetCollisionMaskBit(0, true);
				BuildItem.SetCollisionMaskBit(1, true);
				BuildItem.SetCollisionMaskBit(2, true);

				BuildItem.SetCollisionLayerBit(2, true);

				Console.WriteLine("Finalizing");	
				
				Dictionary<ResourceType, int> resourceDict = new Dictionary<ResourceType, int>();

				resourceDict.Add(ResourceType.Energy, -Item.EnergyCost);
				resourceDict.Add(ResourceType.Metal, -Item.MetalCost);
				resourceDict.Add(ResourceType.Crystal, -Item.CrystalCost);


				GameController.UpdateResources(resourceDict, TeamID);

				SignalGenerator.EmitExitBuildMode();
				IsBuilding = false;


			}	
		}


		public void SetMeshesTransparent(Node node, float alpha) {

			int count = node.GetChildCount();
			for (int i = 0; i < count; i++) {

				Node child = node.GetChild(i);

				if (child is MeshInstance meshChild) {

					int surfaceCount = meshChild.Mesh.GetSurfaceCount();

					for (int j = 0; j < surfaceCount; j++) {

						SpatialMaterial material = (SpatialMaterial)meshChild.Mesh.SurfaceGetMaterial(j);


						if (material != null) {

							
							SpatialMaterial newMaterial = new SpatialMaterial();

							newMaterial.AlbedoTexture = material.AlbedoTexture;
							newMaterial.AlbedoColor = new Color(material.AlbedoColor.r, material.AlbedoColor.g, material.AlbedoColor.b, alpha);
			
							if (alpha == 1.0f) {
								newMaterial.FlagsTransparent = false;
							}
							else {

								newMaterial.FlagsTransparent = true;

							}

							meshChild.Mesh.SurfaceSetMaterial(j, newMaterial);
						}

					}
				}
				SetMeshesTransparent(child, alpha);
			}
		}

	}
}
