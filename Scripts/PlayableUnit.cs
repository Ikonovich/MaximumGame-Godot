using System;
using System.Collections.Generic;
using MagicSmoke;
using Godot;
using MaxGame.Units.Control;

namespace MaxGame {

	public class PlayableUnit : Unit, IConstructor {
		
		[Export]
		public float mouseSensitivity { get; set; } = 0.002f;

		// Keeps track of whether or not the player is currently in this unit.
		// This bool is used to enable player input for this unit.
		[Export]
		public bool IsPlayer {get; set; } = false;

		[Export]
		public NodePath CameraPath;

		[Export]
		public NodePath SelectorPath;

		// Stores a list of the units that this structure can build.
		public Dictionary<int, PackedScene> BuildableItems { get; set; }


		// Used to avoid clicking too often. 

		protected float SelectionTimer = 0.1f;
		protected float SelectionCountdown = 0.0f;

		
		bool MenuOpen = false;

		protected BuildMenuButton BuildMenuButton;
		protected bool IsBuilding = false;
		
		protected TextureRect CrossHair;
		protected FirstPersonCamera Camera;


		protected Vector2 Mouse;
		
		// The selector is used to determine what if any object the crosshairs are hovering over,
		// followed by activating tool tips and allowing selection.

		protected RaySelector Selector;


		public override void _Ready() {

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



			Selector.Owner = this;

			//Initialize velocity vector
			Velocity = new Vector3();

			UnitController = GetNode<UnitController>("UnitPackage/UnitController");

		}		

		public void Setup() {

			
			
			RadialMenu = GetNode<RadialMenu>("UnitPackage/RadialMenu");
			RadialMenu.Parent = this;
			RadialMenu.TeamID = TeamID;

			DebugRenderer = GetNode<DebugRenderer>(DebugRenderer.ComponentPath);
			
			HealthBar = GetNode<HealthBar>("StatusBar");
			SelectionHalo = GetNode<Spatial>("SelectionHalo");

			// HUD objects

			PackedScene HUDscene = (PackedScene)ResourceLoader.Load("res:///GUI/Scenes/HUD.tscn");
			HUD = (HUD)HUDscene.Instance();
			AddChild(HUD);
			CrossHair = (TextureRect)HUD.GetChild(0);
			HUD.UpdateHealth(MaxHealth, Health);
			HUD.TeamID = TeamID;
			HUD.Hide();
			CrossHair.Hide();

			

			GameController = GetTree().Root.GetNode<GameController>("Node/GameController");

				
			CurrentWeapon = Selector;
			HUD.WeaponSwap(CurrentWeapon.GetIcon());

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

			IsReady = true;

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

		public override void _UnhandledInput(InputEvent @event) {

			GetTree().SetInputAsHandled();
		}

		public override void _ProcessInput(float delta) {


			// This method processes button inputs, but not mouse movement inputs, which are handled by
			// _Input.

			if (IsPlayer == true) {
			
			// Counts down the selection timer
				if (SelectionCountdown > 0) {
					SelectionCountdown -= delta;
				}


				if (Input.IsActionPressed("move_forward")) {
					InputVector -= GlobalTransform.basis.z * delta;

				}
				if (Input.IsActionPressed("move_back")) {
					InputVector += GlobalTransform.basis.z * delta;
					
				}
				if (Input.IsActionPressed("move_left")) {
					InputVector -= GlobalTransform.basis.x * delta;
					
				}
				if (Input.IsActionPressed("move_right")) {
					InputVector += GlobalTransform.basis.x * delta;
					
				}

				if (Input.IsActionPressed("NumOne")) {

					CurrentWeapon = Selector;
					HUD.WeaponSwap(Selector.GetIcon());

				}
				if (Input.IsActionPressed("NumTwo") && WeaponOne != null) {

					CurrentWeapon = WeaponOne;			
					HUD.WeaponSwap(WeaponOne.GetIcon());


				}
				if (Input.IsActionPressed("NumThree") && WeaponTwo != null) {

					CurrentWeapon = WeaponTwo;
					HUD.WeaponSwap(WeaponTwo.GetIcon());
				}


				
				if (!IsGroundUnit) { 

					if (Input.IsActionPressed("move_up")) {
						InputVector += GlobalTransform.basis.y * delta;
					
					}    

					if (Input.IsActionPressed("move_down")) {
						InputVector -= GlobalTransform.basis.y * delta;
					
					}       

					if (Input.IsActionPressed("rotate_left")) {
						RotateObjectLocal(Vector3.Back, delta);
					}

					if (Input.IsActionPressed("rotate_right")) {
						RotateObjectLocal(Vector3.Forward, delta);	
					}	
				
				}

				// End movement handling

				// Begin interactions handling


				if (Input.IsActionPressed("shoot") && (SelectionCountdown <= 0) && (IsBuilding == false)) {

					SelectionCountdown = SelectionTimer;
					CurrentWeapon.Shoot();

				}

				if (Input.IsActionJustReleased("shoot") && (CurrentWeapon == Selector) && (IsBuilding == true) && SelectionCountdown <= 0) {
					Console.WriteLine("Should be finalizing build from player input");
					Selector.FinalizeBuild();
					
				}

				if (Input.IsActionJustReleased("shoot") && (CurrentWeapon == Selector) && (IsBuilding == false)) {

					Selector.Release();
				}



				// This tells the selector to get the point it's raycast is colliding with the environment
				// and send it to the GameController so that each currently selected unit can be directed
				// to move towards it.

				if (Input.IsActionJustReleased("right_click")) {
					
					Selector.RightSelect();

				}
				

				// This function allows the mouse to be released from the UI.

				if (Input.IsActionJustPressed("ui_cancel") && (MenuOpen == false)) {


					HUD.OpenBuildMenu(GetBuildableItems(), this);
					OpenMenu(TeamID);
					MenuOpen = true;


				}
				else if (Input.IsActionJustPressed("ui_cancel") && (MenuOpen == true)) {

					SignalGenerator.EmitCloseMenu(TeamID);
					CloseMenu(TeamID);
					MenuOpen = false;
				}

				// This function recaptures the mouse.
				if (Input.IsActionPressed("ui_accept")) {

					Input.SetMouseMode(Input.MouseMode.Captured);
				}
				
				// This function swaps between the first and third person camera views.
				if (Input.IsActionJustPressed("swap_camera")) {

					GameController.SwapCamera();
				}

			}
				// End taking keyboard input

		}
		

		// This function keeps the HUD centered.
		public void _ProcessCamera() {

			if (IsPlayer) {

				Vector2 CameraCenter;
				
				

				CameraCenter.x = (Camera.GetViewport().GetVisibleRect().Size.x / 2) - (CrossHair.RectSize.x / 2);
				CameraCenter.y = (Camera.GetViewport().GetVisibleRect().Size.y / 2) - (CrossHair.RectSize.y / 2);

				CrossHair.SetPosition(CameraCenter, false);

					

				}
			}

		public virtual void Shoot() {

		}

		// This function sets this unit as the player unit in the Game Controller.
		public virtual void SetAsPlayer() {

			IsPlayer = true;

			// Hide healthbar and button

			HealthBar.Hide();
			// Show the crosshair
			CrossHair.Show();
			HUD.Show();



			//Set the selector's owner
			Selector.Owner = this;

			// Connect to the signal generator.
			
			SignalGenerator.Connect("MenuOpened", this, nameof(OpenMenu));
			SignalGenerator.Connect("MenuClosed", this, nameof(CloseMenu));
			SignalGenerator.Connect("EnterBuildMode", this, nameof(EnterBuildMode));
			SignalGenerator.Connect("ExitBuildMode", this, nameof(ExitBuildMode));

			SignalGenerator.EmitPlayerSet(this, TeamID);
			

		}

		// The Game Controller uses this function to unset this player as the current player unit.
		// Calling this is necessary to disable player input to a unit.
		public void UnsetAsPlayer() {

			IsPlayer = false;

			// Reset the signal generator to disconnect from all signals.
			SignalGenerator = new SignalGenerator();

			// Hide the CrossHair and the HUD

			CrossHair.Hide();
			HUD.Hide();

		

			// Disconnect from the build buttons by replacing the old button. 
			BuildMenuButton = new BuildMenuButton();

			
		}

		public void BuildMenu(Dictionary<int, PackedScene> menuItems, Node constructor) {

			Console.WriteLine("Open build menu signal detected");
			HUD.OpenBuildMenu(menuItems, constructor);

		}

		// Signalled whenever this 
		
		public void OpenMenu(int teamID) {

			if ((teamID == TeamID) && (IsPlayer == true)) {

				Input.SetMouseMode(Input.MouseMode.Visible);
				CrossHair.Hide();
				MenuOpen = true;
			}

		}


		
		public void CloseMenu(int teamID) {

			if ((teamID == TeamID) && (IsPlayer == true)) {

				Input.SetMouseMode(Input.MouseMode.Captured);
				CrossHair.Show();
				MenuOpen = false;
			}
		}
		

		public override void _Input(InputEvent @event) {
			
			// Handles mouse movement 
			

			if (IsPlayer && @event is InputEventMouseMotion && Input.GetMouseMode() == Input.MouseMode.Captured) {

				InputEventMouseMotion mouseEvent = @event as InputEventMouseMotion;
			

				RotateObjectLocal(Vector3.Up, -mouseEvent.Relative.x * mouseSensitivity);

				if (!IsGroundUnit) {
					
						// Used to stabilize the x axis during mouse rotation.

						
					float yRotation = Rotation.y;

					RotateObjectLocal(Vector3.Left, mouseEvent.Relative.y * mouseSensitivity);

					Rotation = new Vector3(Rotation.x, Rotation.y, 0.0f);


				}
				else {
		

					Head.RotateObjectLocal(Vector3.Right, mouseEvent.Relative.y * mouseSensitivity);
					Console.WriteLine(Head.Rotation.x.ToString());


					if (Head.Rotation.x > TurretUpLimit/360) {

						Head.Rotation = new Vector3(TurretUpLimit/360, Head.Rotation.y, Head.Rotation.z);


					}

					if (Head.Rotation.x < TurretDownLimit/360) {


						Head.Rotation = new Vector3(TurretDownLimit/360, Head.Rotation.y, Head.Rotation.z);
					}
				}


			}
			

		}
		

		// Returns the items the player can currently build.

		public Dictionary<int, PackedScene> GetBuildableItems() {

			TeamState state = GameController.GetTeamState(TeamID);

			Dictionary<int, PackedScene> buildableDict = JsonHandler.GetIBuildables();
			
			Dictionary<int, PackedScene> tempDict = new Dictionary<int, PackedScene>();

			foreach (KeyValuePair<int, PackedScene> kvp in buildableDict) {

				tempDict.Add(kvp.Key, kvp.Value);
			}

			return tempDict;
			
		}

		// Used to call the build function on the ray selector.
		public void Build(PackedScene item) {

			
			IBuildable buildItem = (IBuildable)item.Instance();
			buildItem.IsPreviewScene = false;

			if (IsPlayer == true) {
				Selector.Build(buildItem);
				HUD.CloseBuildMenu(TeamID);


				MenuOpen = false;
			}
		}
		

		// These two methods are required to implement the IConstructor interface, but are
		// not used by the standard playable unit.

		public void Add(IBuildable item) {}


		public void Remove(IBuildable item) {}


		public void EnterBuildMode() {

			Console.WriteLine("Entering build mode");
			IsBuilding = true;
		}

		public void ExitBuildMode() {

			IsBuilding = false;
			Console.WriteLine("Exiting build mode");

		}

		// This allows the Game Controller to get the unit camera when the player switches to this unit.

		public virtual Camera GetCamera() {

			return Camera;

		}

		public void ShakeCamera(double magnitude, double decay) {

			Camera.StartShake(magnitude, decay);
		}

	}
}
