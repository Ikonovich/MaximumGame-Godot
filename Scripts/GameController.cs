using System;
using System.Collections.Generic;
using Godot;
using MagicSmoke;

namespace MaxGame {


	public class GameController : Node {
		
		// Keeps track of whether or not the orbit camera is being used by the player.
		[Export]
		public bool IsThirdPerson { get; protected set; } = true;

		SignalGenerator SignalGenerator;

		private OrbitCamera OrbitCamera;

		private Camera TopDownCamera;

		private Camera FPSCamera;

		// Stores the unit that the player reverts to if it leaves another unit.
		private PlayableUnit DefaultPlayer;

		// Stores the unit the player is currently in if it's not in the Default.
		private PlayableUnit CurrentPlayer;

		// Determines whether or not the default player object is.
		private bool IsDefault = true;


		// Used to prevent a player from accidentally swapping back to the original unit
		// when two units are looking at each other.
		private float SwapDelay = 0.25f;
		private float SwapCountdown = 0.5f;

		// Stores the current player item selection.

		private List<Unit> SelectedList;

		// Stores any currently selected building.
		private Building SelectedBuilding;

		
		// Navigation structure
		private Navigation Navigation;

		// Stores the teams.

		private TeamState TeamOne;
		private TeamState TeamTwo;

		private Dictionary<int, TeamState> TeamDict;



		public override void _Ready() {


			SelectedList = new List<Unit>();

			SignalGenerator = GetTree().Root.GetNode<SignalGenerator>("Node/SignalGenerator");

			Navigation = GetTree().Root.GetNode<Navigation>("Node/Terrain/Spatial/Navigation");

			//OrbitCamera = GetNode<OrbitCamera>("OrbitCamera");

			DefaultPlayer = GetTree().Root.GetNode<PlayableUnit>("Node/Player");
			CurrentPlayer = DefaultPlayer;

			// Set the default first person camera.

			FPSCamera = CurrentPlayer.GetNode<Camera>("Pivot/Camera");

			FPSCamera.Current = true;

			// Set default orbital camera target.

			//OrbitCamera.CameraTargetPath = CurrentPlayer.GetPath();

			TeamOne = new TeamState();
			TeamTwo = new TeamState();

			TeamOne.CreateTeam(1);
			TeamTwo.CreateTeam(2);

			TeamDict = new Dictionary<int, TeamState>();
			TeamDict.Add(TeamOne.TeamID, TeamOne);
			TeamDict.Add(TeamTwo.TeamID, TeamTwo);


		}

		public override void _PhysicsProcess(float delta) {
			if (SwapCountdown > 0) {

				SwapCountdown -= delta;
			}
		}

		public PlayableUnit GetPlayer() {

			return CurrentPlayer;

		}

		public void SetPlayer(PlayableUnit unit) {




			if (SwapCountdown <= 0) {

				// Changing the active player unit.
				CurrentPlayer.UnsetAsPlayer();


				CurrentPlayer = unit;
			

				FPSCamera = CurrentPlayer.GetCamera();
				FPSCamera.Current = true;

				Console.WriteLine(CurrentPlayer.Name);


				// Sets whether or not the player assigned is currently the default player unit.
				if (unit != DefaultPlayer) {
					IsDefault = false;
				}
				else { 
					IsDefault = true;
				}

				SwapCountdown = SwapDelay;

				
				CurrentPlayer.SetAsPlayer();
			}




		}
		
		// Returns the player to the default unit when called. 
		public void UnsetPlayer() {

			SetPlayer(DefaultPlayer);



		}
		
		// Currently does nothing.

		public void SwapCamera() {

			// if (IsThirdPerson) {

			// 	FPSCamera.Current = true;
			// 	IsThirdPerson = false;
			// }

			// else {

			// 	IsThirdPerson = true;
			// 	OrbitCamera.Current = true;
			// }
		}

		public void Selected(Godot.Collections.Array selected) {

			Deselected();


			SelectedList = new List<Unit>();


			// Checks each item to see if it's a unit, if so adds it to the SelectedList.

			foreach (Node item in selected) {

				if (item is Unit) {
					SelectedList.Add((Unit)item);
				}
			}

			// Goes through the selected list and applies selected functionality to each unit.

			foreach (Unit unit in SelectedList) {

				unit.Selected();

			}


		}

		// Used for selecting buildings, which can only be done individually.

		public void BuildingSelected(Building building) {


			Console.WriteLine("Setting building in game controller");
			SelectedBuilding = building;
		}

				
		// Goes through the selected list and applies Deselected functionality to each unit.

		public void Deselected() {

			foreach (Unit unit in SelectedList) {

				unit.Deselected();

			}

			if (SelectedBuilding != null) {

				SelectedBuilding.Deselected();
				SelectedBuilding = null;
			}

			SelectedList = new List<Unit>();

		}

		// This method handles when an object is targeted by the selector on right click.
		// Can be used to assign a unit to follow/support a friendly unit or destroy an enemy unit.

		// This method handles when a point is targeted by the selector on right click,
		// typically for movement.


		public void ObjectTargeted(IDestructible target) {

			Console.WriteLine("Target object:");

			foreach (Unit item in SelectedList) {

				item.NewTarget(target);

			}

			if (SelectedBuilding != null) {


			}
		}


		public void PointTargeted(Vector3 targetPoint) {
			

			foreach (Unit item in SelectedList) {

				item.NewTargetPoint(targetPoint);
			}

			
			if (SelectedBuilding != null) {

				Console.WriteLine("BUilding path generated");

				SelectedBuilding.SetRallyPoint(targetPoint);
			}
		}

		// These methods are called to update the resources available to a team. The first
		// takes an individual resource type and amount and a team ID. The second takes three
		// int modifiers and a team ID.
		// The amounts passed are added to the totals stored in the TeamState, and are not
		// setting it directly. 
		// For this reason, build costs should be passed in as negative values and
		// resource acquisitions as positive values.

		public void UpdateResource(ResourceType resource, int amount, int teamID) {

			TeamState team = TeamDict[teamID];
			team.UpdateResource(resource, amount);
			SignalGenerator.EmitUpdateResources();
			
		}


		public void UpdateResources(Dictionary<ResourceType, int> modifierDict, int teamID) {

			TeamState team = TeamDict[teamID];
			team.UpdateResourceDict(modifierDict);

			SignalGenerator.EmitUpdateResources();
		}

		public TeamState GetTeamState(int teamID) {

			return TeamDict[teamID];
		}


	}
}
