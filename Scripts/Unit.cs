using System;
using System.Collections.Generic;
using Godot;
using MagicSmoke;
using MaxGame.Units.Control;


namespace MaxGame {
// This class functions as the root of all mobile units in the game.

	public class Unit : KinematicBody, IBuildable, IDestructible, ISensor {

		//Stores the name of this object
		[Export]
		public string Name { get; set; } = "NO NAME";

		// Stores the cost in resources of this object.
		[Export]
		public int EnergyCost { get; set; } = 0;
		[Export]
		public int MetalCost { get; set; } = 0;
		[Export]
		public int CrystalCost { get ; set; } = 0;

		// <remarks>
        // Stores the maximum allowed magnitude of the velocity vector for this unit.
        // Used by the movement controller.
        // </remarks>
		[Export]
        public float MaxSpeed = 10.0f;



		[Export]
		public NodePath TargetRayPath;

		[Export]
		public NodePath HeadPath;

		
		[Export]
		public NodePath WeaponMountPath;

		// Used to determine how far the turret of a ground unit can traverse up and down.
		// Defaults to 75 degrees in both directions.
		[Export]
		public float TurretUpLimit = 75.0f;


		[Export]
		public float TurretDownLimit = -75.0f;


		// Used to determine the Yoffset when building this item.
		// Accessed and used directly by RaySelector.
		[Export]
		public float TerrainYoffset { get; set; } = 0.0f;

		// Used to determine the camera offset when showing this item in the inventory.
		// Accessed and used directly by BuildMenuButton.
		[Export]
		public float PreviewXoffset { get; set; } = 0.0f;
		[Export]
		public float PreviewYoffset { get; set; } = 0.0f;
		[Export]
		public float PreviewZoffset { get; set; } = 0.0f;

		[Export]
		public int MaxHealth { get; set; } = 100;

		[Export]
		public int Health { get; set; } = 100;

		[Export]
		public float DamageMultiplier { get; set; } = 1.0f;

		// Determines how far away from the unit this object will rotate it's guns towards it.

		[Export]
		public float Range = 20;

		[Export]
		public int TeamID { get; set; } = 1;


		[Export]
		public float Accel = 0.1f;

		
		public float DeaccelAir = 0.3f;

		public float DeaccelGround = 0.7f;


		
		// <remarks>
		//	Stores the unit's stance, which effects behavior when the unit control structure is in the
		// "Idle" state.
		// Options are "Passive" and "Aggressive".
		// An aggressive unit will follow enemy units a certain distance in order to attack them.
		// A passive unit will remain in position and only fire on units that come within range.
		// </remarks>

		protected string Stance = "Aggressive";
		
		// Navigation structure
		protected Navigation Navigation;

		
		// Stores the part of the object that rotates independently from the body when aiming 
		// the weapons, if there is one.
		protected Spatial Head;

		// Stores the raycast node that is in the head, for targeting purposes.
		protected RayCast TargetRay;


			
		// Stores the primary non-selector weapon
		[Export]
		public PackedScene WeaponOneScene { get; set; }
		// Stores the secondary non-selector weapon

		[Export]
		public PackedScene WeaponTwoScene { get; set; }


		// Stores the weapons after they are converted to Spatial objects.
		protected Weapon WeaponOne;
		
		protected Weapon WeaponTwo;


		// This is the maximum of amount of deceleration that is applied in any given 
		// direction for an air unit.


		// Determines whether or not gravity is applied to the unit. 
		[Export]
		public bool IsGroundUnit = false;
		
		// Stores the mass of the unit, affecting force modulated movement behaviors.
		[Export]
		float Mass = 1.0f;


		// Stores the selection halo for each object.
		protected Spatial SelectionHalo;

		// Keeps track of whether or not the unit is selected.
		public bool IsSelected = false;

		// A bool used to determine whether or not this is a preview unit.
		// If it is, the HUD will not be instanced.
		// This is necessary to prevent infinite recursive viewports in the build menus.

		[Export]
		public bool IsPreviewScene { get; set; } = true;

		
		public Spatial CurrentTarget;
		public Vector3 TargetPoint;


		protected HealthBar HealthBar;


		// This menu serves as the interaction menu, and shows up on mouseover.
		
		protected Sprite3D RadialMenu;

		protected GameController GameController;

		protected UnitController UnitController;

		protected SignalGenerator SignalGenerator;


		// Sensor area 
		protected Area DetectionArea; 


		protected Vector3 InputVector;

		public Vector3 Velocity;

		protected float ReloadTime;

		protected float ReloadCounter;

		protected Tween TargetAnimation;

		protected HoverButton HoverButton;

		// Stores the position vectors for the unit's path
		protected Queue<Vector3> Path;

		// Stores whether or not path following is active for this unit
		protected bool Following = false;

		// Stores the currently active weapon
		protected Weapon CurrentWeapon;

		// Stores the spatial node that weapons go under
		protected Spatial WeaponMount;


		// Determines the distance between the current actor location and the
		// NextPoint that is considered "Close enough". Once this distance is reached,
		// NextPoint will be transitioned to the next position vector in the Path queue.
		protected float MaxDistance = 2.0f;

		// Stores the effect that plays when this unit is destroyed.

		protected PackedScene DeathEffectScene;

		// one-shot death countdown timer. Used to call QueueFree after the death effect has gotten going.
		protected float DeathCountdown = 1.0f;

		protected bool IsDead = false;
		

		// HUD, if used
		protected HUD HUD;

		// Used to determine whether or not this scene's various objects have been readied.
		// Used to transition from preview scene to a regular world scene.

		protected bool IsReady = false;

		

		// Debug Renderer object
		protected DebugRenderer DebugRenderer;

		public override void _Ready() {

			
			DebugRenderer = GetNode<DebugRenderer>(DebugRenderer.ComponentPath);
			Navigation = GetTree().Root.GetNode<Navigation>("Node/Terrain/Spatial/Navigation");

		
			UnitController = GetNode<UnitController>("UnitPackage/UnitController");
			GameController = GetTree().Root.GetNode<GameController>("Node/GameController");

			SelectionHalo = GetNode<Spatial>("SelectionHalo");



			// Setting up the sensor system.

			
			DetectionArea = GetNode<Area>("DetectionArea");
			DetectionArea.Connect("area_entered", this, "TargetDetected");
			DetectionArea.Connect("area_exited", this, "TargetGone");

			// Initialize health to full value and update the displays.
			MaxHealth = 100;
			Health = MaxHealth;

			HealthBar = GetNode<HealthBar>("StatusBar");
			HealthBar.UpdateHealth(Health, MaxHealth);




			// Initialize movement vectors.

			Velocity = new Vector3();

		}

		public override void _PhysicsProcess(float delta) {
			_ProcessInput(delta);
			_ProcessTargeting(delta);

			// Handles disapeparing unit on death

			if (IsDead == true) {

				DeathCountdown -= delta;

				if (DeathCountdown <= 0) {

					QueueFree();
				}

			}
		}


		public virtual void _ProcessInput(float delta) {}

		public virtual void _ProcessPathfinding(float delta) {



			// Checks to see if following is true


			if (Following) {

				//Console.WriteLine("Begin pathfinding, Unit 277");

				Vector3 toPoint = new Vector3();
				Vector3 nextPoint = new Vector3();

				// Used to store the new air path if this is an air unit.
				Queue<Vector3> airPath = new Queue<Vector3>();

				
				if (Path.Count != 0) { 

					// If this is an air unit, sets path's Y values to the level the air unit 
					// is already on.


					if (!IsGroundUnit) {
						foreach (Vector3 point in Path) {
							Vector3 airPoint = point;

							airPoint.y = GlobalTransform.origin.y;
							airPath.Enqueue(airPoint);
						}
						Path = airPath;
					}
					


					// Use the debugger to draw the various points along the path
					//DebugDrawPath();

					//Console.WriteLine("Next point");

					nextPoint = Path.Peek();

					// Distance to the next point
					toPoint = nextPoint - GlobalTransform.origin;
					float distance = toPoint.Length();



					if (!IsGroundUnit) {
						toPoint.y = GlobalTransform.origin.y;
					}
					
					//Console.WriteLine("Navigating to: " + nextPoint);
					//Console.WriteLine("From: " + GlobalTransform.origin);
					//Console.WriteLine("Distance: " + distance);

					if (distance <= MaxDistance) {



						Path.Dequeue();
						Console.WriteLine("Point " + nextPoint + " removed at " + GlobalTransform.origin);
					
					}

					else {

						InputVector = toPoint;

						// Sets Y vector to zero for air units.

						if (!IsGroundUnit) {
							InputVector.y = 0;
						}
					}
				}
				else  {

					Console.WriteLine("Path complete");
					Following = false;

				}
			}
		}

		// Called by the game controller when this unit is assigned to a target.

		public virtual void NewTargetPoint(Vector3 targetPoint) {

			TargetPoint = targetPoint;
			UnitController.TransitionState("MoveToPoint");
		}


		public virtual void NewTarget(IDestructible target) {

			CurrentTarget = (Spatial)target;

			if (target.TeamID == TeamID) {

				UnitController.TransitionState("FollowTarget");

			}
			else {
				UnitController.TransitionState("AttackTarget");
			}
		}
		
		public virtual void _ProcessMovement(float delta) {


			// Normalizing the input vector.
			
			if (InputVector.Length() > 0.01) {

				


			// 	//Console.WriteLine("Input vector is non-zero");
				
			InputVector = InputVector.Normalized();

				
			// 	// 	Multiplying the direction vector times acceleration and adding it to the velocity vector.

			Velocity += (InputVector * Accel);
			
			}
			
			// Code handling deceleration

			// First, we check to see if the unti is a ground unit.
			// A different Deaccel factor applies to non ground units.

			// Next we normalize the current velocity vector, subtracts the normalized input
			// vector, and applies deceleration to the result.
			// Effectively, only applies movement resistance in diirections
			// that the player is not moving.

			
			if (IsGroundUnit == true) {  

				Velocity -= ((Velocity.Normalized() - InputVector.Normalized()) * DeaccelGround);
				
			// 	// Checks to see if velocity is low enough to force the unit to a standstill.

				if (IsOnFloor() == false) {

					Velocity.y += (-3.0f);
				}

				if (Velocity.Length() > 0.1f) {

					Velocity = MoveAndSlideWithSnap(Velocity, GetFloorNormal(), Vector3.Up, true, 1, 0.5f, false);
				}

				
			}
			else {

				Velocity -= ((Velocity.Normalized() - InputVector.Normalized()) * DeaccelAir);

				if ((Velocity.Length() < 0.3f) && (InputVector == Vector3.Zero)) {

					Velocity = Vector3.Zero;

				}

				Console.WriteLine("Moving player");
				Velocity = MoveAndSlideWithSnap(Velocity, GetFloorNormal(), Vector3.Up, true, 1, 1, true);
				
			}





			// This is collision testing code, if necessary.

			int collisionCount = GetSlideCount();

			if (collisionCount > 1) {

					
				KinematicCollision test = GetSlideCollision(0);


				Godot.Object collider = test.GetCollider();

				Console.WriteLine(collider.GetClass());
			}

		}

		public virtual void _ProcessTargeting(float delta) {

			if (CurrentTarget != null) {


				IDestructible target = (IDestructible)CurrentTarget;

				// Gets the range to the current target


				Vector3 targetVector = GlobalTransform.origin - CurrentTarget.GlobalTransform.origin;

				float range = targetVector.Length();

				if ((Range > targetVector.Length()) && (target.TeamID != TeamID)) {

					Console.WriteLine("Should be rotating turret");


					TargetRay.LookAt(CurrentTarget.GlobalTransform.origin, Vector3.Up);

					// Used to calculate how long the turret should take to interpolate.
					Vector3 rotateDiff = Head.Rotation - TargetRay.Rotation;
					float rotateTime = rotateDiff.Length() * 4;

					TargetAnimation.InterpolateProperty(Head, "rotation_degrees", Head.RotationDegrees, TargetRay.RotationDegrees + new Vector3(0.0f, 180.0f, 0.0f), 0.5f, Tween.TransitionType.Linear, Tween.EaseType.InOut, 0);
					TargetAnimation.Start();
				}
			}
		}

		public virtual void Selected() {

			Console.WriteLine("Unit is selected");

			if (IsPreviewScene == false) {
				SelectionHalo.Show();
				HealthBar.ShowItem();

			
				IsSelected = true;
			}
		}
		

		public virtual void Deselected() {

			if (IsPreviewScene == false) {
				SelectionHalo.Hide();
				HealthBar.HideItem();
				HoverButton.HideItem();
				IsSelected = false;
			}

		}

		public virtual void CursorInteract() {

			if (IsPreviewScene == false) {
				HealthBar.ShowItem();
				HoverButton.ShowItem();
			}
			

		}



		public void ObjectDetected(Spatial newTarget) {


			
		 	if ((newTarget is IDestructible) && (CurrentTarget == null)) {
				IDestructible target = (IDestructible)newTarget;
			
				CurrentTarget = newTarget;
		 	}



		}

		// Deprecated
		public void TargetDetected(Area target) {}


		public virtual void TargetGone(Area target) {



		}

		// This method is used by called by the various weapons upon impact to implement the damage effect.
		public virtual void ProjectileHit(float damage, Transform inputTransform) {

			int damageActual = (int)(damage * DamageMultiplier);
			Health -= damageActual;

			HealthBar.UpdateHealth(Health, MaxHealth);

			// Updates the HUD healthbar, if a HUD exists (I.E. this is a PlayableUnit type)

			if (HUD != null) {

				HUD.UpdateHealth(Health, MaxHealth);
			}


			if (Health <= 0) {

				Destroyed();
		
			}



		}

		public virtual void Destroyed() {

			// Set this unit to dead so it will start the DeathCountdown and call QueueFree after it finishes.
			IsDead = true;

			// Get death effect scene.

			DeathEffectScene = (PackedScene)ResourceLoader.Load("res://Game Assets/Effects/BlueSplosion.tscn");

			DeathEffect deathEffect = (DeathEffect)DeathEffectScene.Instance();

			Node root = GetTree().Root;
			root.AddChild(deathEffect);

			deathEffect.GlobalTransform = GlobalTransform;
		}


		// 
		public void DebugDrawPath() {
			var lastPoint = GlobalTransform.origin;
			foreach(var point in Path) {


				DebugRenderer.QueueWireframePyramid(lastPoint, (point - lastPoint).Normalized(), 0.8f, 0.2f, Colors.Purple);
				DebugRenderer.QueueLineSegment(lastPoint, point, Colors.DarkRed);
				lastPoint = point;
			}
		}

		public string GetStance() {

			return Stance;
		}

	}
}