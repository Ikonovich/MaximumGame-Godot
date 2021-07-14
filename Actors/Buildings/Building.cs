using System;
using System.Collections.Generic;
using Godot;

namespace MaxGame {

	// This class is a generic class for immobile structures. It stores a list of objects buildable from 
	// the menu, as well as opening the build menu on double click.
	// It also stores an offset that allows the structure to be placed directly on the ground, instead of
	// halfway inside of it. 


	public class Building : StaticBody, IInteractable, IBuildable, IDestructible, IConstructor {


		//Stores the name of this object
		[Export]
		public string Name { get; set; } = "NO NAME";

		// Stores the team ID of the building. Must be set when the building is placed.

		public int TeamID { get; set; } 

		// Stores the cost in resources of this object.
		[Export]
		public int EnergyCost { get; set; } = 0;
		[Export]
   		public int MetalCost { get; set; } = 0;
		[Export]
		public int CrystalCost { get ; set; } = 0;

		
		[Export]
		public bool CanBuild { get; set; } = false;

		// Stores the translation relative to the parent that constructed units are placed at.

		[Export]
		public Vector3 BuildTranslation { get; set; } = new Vector3(0.0f, 0.0f, 0.0f);

		// Takes a json file that will initialize this building's buildable items.
		[Export]
		string QueueMenuJson { get; set; }

		public float BuildTimer = 3.0f;

		public float BuildCountdown = 0.0f;

		// Keeps track of whether or not this building can build things.


		protected SignalGenerator SignalGenerator;

		protected QueueMenu QueueMenu;

		// Stores a list of IBuildables that this structure can construct.
		public Dictionary<int, PackedScene> BuildableItems { get; set; }

		// Used to add a height offset to this structure. Directly accessed by the player's RaySelector when 
		// placing the building.
		[Export]
		public float TerrainYoffset { get; set; }

		

		// Used to determine the camera offset when showing this item in the inventory.
		// Accessed and used directly by QueueMenuButton.
		[Export]
		public float PreviewXoffset { get; set; } = 0.0f;
		[Export]
		public float PreviewYoffset { get; set; } = 0.0f;
		[Export]
		public float PreviewZoffset { get; set; } = 0.0f;


		
		// Health stats. Defaults to 100.
		[Export]
		public int MaxHealth { get; set; } = 100;
		[Export]
		public int Health { get; set; } = 100;

		// This determines how much damage the particular instance takes.
		// 1.0 is full damage, 0.5 is 1/2 damage, 0.0 is no damage, etc. Defaults to 1.
		[Export]
		public float DamageMultiplier { get; set; } = 1;



		public bool IsPreviewScene { get; set; } = true;
		public bool IsReady = false;

		protected float SelectionCountdown = 0.0f;
		protected float SelectionTimer = 0.300f;
		protected Spatial SelectionEffect;


		// Stores the point new units first go to, if any.
		Vector3 RallyPoint;

		public override void _Ready() {


			SignalGenerator = GetTree().Root.GetNode<SignalGenerator>("Node/SignalGenerator");
			SignalGenerator.Connect("MenuTakeover", this, nameof(CloseMenu));

			
			SelectionEffect = GetNode<Spatial>("SelectionEffect");
			SelectionEffect.Hide();


		}

		public void Setup() {

			if (CanBuild == true) {

				PackedScene QueueMenuScene = (PackedScene)ResourceLoader.Load("res://GUI/Scenes/QueueMenu.tscn");
				QueueMenu = (QueueMenu)QueueMenuScene.Instance();
				QueueMenu.Parent = this;
				AddChild(QueueMenu);


				
				SetBuildableItems();
				QueueMenu.Close(TeamID);

			}
		

			IsReady = true;
		}




		public override void _Process(float delta) {

			if ((IsPreviewScene == false) && (IsReady == false)) {

				Setup();
			}
			if (SelectionCountdown > 0)  {

				SelectionCountdown -= delta;
			}

			// Keeps track of time between production items, for now.

			if (BuildCountdown > 0) {

				BuildCountdown -= delta;

				if (BuildCountdown <= 0) {

					Finalize();
				}
			}
		}
		
		public void CursorInteract() {



		}

		public void Selected() {



			if (SelectionCountdown > 0) {

				
				Console.WriteLine("Second selection");

				if ((CanBuild == true) && (IsPreviewScene == false)) {
					SetBuildableItems();
					QueueMenu.Open(BuildableItems, this);
					SignalGenerator.EmitMenuOpened(TeamID);
				}

			}
			else {

				SelectionEffect.Show();
				SelectionCountdown = SelectionTimer;

			}

		}

		public void Deselected() {
			
			SelectionEffect.Hide();
		}

		// Takes a list of unit IDs and maps it 

		public void SetBuildableItems() {


			Console.WriteLine("Checking buildable items" + CanBuild.ToString());
			BuildableItems = new Dictionary<int, PackedScene>();



			if ((CanBuild == true) && (QueueMenuJson != null)) {

				Console.WriteLine("Setting buildable items");
				
				Dictionary<int, PackedScene> Units = JsonHandler.GetIBuildables();
				List<int> buildableIDs = JsonHandler.GetBuildableList(QueueMenuJson);

				foreach (int key in buildableIDs) {

					Console.WriteLine("Adding buildable item");
					BuildableItems.Add(key, Units[key]);
				}
			}
			QueueMenu.PopulateMenu(BuildableItems, this);
		}

		// Adds an item to build

		public void Build(PackedScene item) {

			
			// IBuildable buildItem = (IBuildable)item.Instance();
			// buildItem.IsPreviewScene = false;

			
			QueueMenu.Enqueue(item);


			if (BuildCountdown <= 0) {

				BuildCountdown = BuildTimer;
			}

		}

		public void SetRallyPoint(Vector3 rallyPoint) {


			Console.WriteLine("Setting building path in building");
			RallyPoint = rallyPoint;
		}


		public void Finalize() {

			PackedScene newItem = QueueMenu.Pop();

			Unit buildUnit = (Unit)newItem.Instance();

			GetTree().Root.AddChild(buildUnit);
			buildUnit.IsPreviewScene = false;

			buildUnit.SetCollisionLayerBit(2, true);
			buildUnit.SetCollisionMaskBit(0, true);			
			buildUnit.SetCollisionMaskBit(1, true);
			buildUnit.SetCollisionMaskBit(2, true);



			buildUnit.Translation = Translation + BuildTranslation;


			if (RallyPoint != null) {

				Console.WriteLine("Setting building path in unit");
				buildUnit.NewTargetPoint(RallyPoint);
			}


			if (QueueMenu.Index > 0) {

				BuildCountdown = BuildTimer;
			}
			

		}


		public void CloseMenu(int teamID) {

			if ((CanBuild == true) && (IsPreviewScene == false) && (teamID == TeamID)) {

				QueueMenu.Close(teamID);
			}
		}


		// Method that allows entities to interact with projectiles.

		public void ProjectileHit(float damage, Transform projectileTransform){

		}

	}
}
