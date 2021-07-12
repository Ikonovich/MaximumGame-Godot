using System;
using System.Collections.Generic;
using Godot;
using MaxGame;

namespace MaxGame {




	

	public class HUD : Control {
		
		// Helper nodes

		protected GameController GameController;
		protected SignalGenerator SignalGenerator;


		// Crosshair for all playable units. 
		protected TextureRect CrossHair;

		protected TextureProgress Bar;

		protected MarginContainer BuildMenu;

		protected TextureRect WeaponIconRect; 
		protected Texture CurrentWeaponIcon;

		protected Label EnergyCounter;
		protected Label MetalCounter;
		protected Label CrystalCounter;

		// Stores the default build menu items, specifically the currently available structures.

		protected List<PackedScene> DefaultMenuItems;

		// Keeps track of which page the inventory is on, determining what multplier to use
		// to access the list of inventory objects.

		protected int Page = 0;

		// Stores the 9 menu buttons
		
		protected List<BuildMenuButton> ButtonList;



		// Stores the team ID. Has to be set from the parent.
		public int TeamID = 0;
			

		public override void _Ready() {

			
			GameController = GetTree().Root.GetNode<GameController>("Node/GameController");
			SignalGenerator = GetTree().Root.GetNode<SignalGenerator>("Node/SignalGenerator");

			SignalGenerator.Connect("CloseMenu", this, nameof(CloseBuildMenu));

			// Populate the button list

			ButtonList = new List<BuildMenuButton>();

			BuildMenuButton button1 = GetNode<BuildMenuButton>("BuildMenu/PanelContainer/VBoxContainer/HBoxContainer/VBoxContainer2/HBoxContainer/PanelContainer1");
			BuildMenuButton button2 = GetNode<BuildMenuButton>("BuildMenu/PanelContainer/VBoxContainer/HBoxContainer/VBoxContainer2/HBoxContainer/PanelContainer2");
			BuildMenuButton button3 = GetNode<BuildMenuButton>("BuildMenu/PanelContainer/VBoxContainer/HBoxContainer/VBoxContainer2/HBoxContainer/PanelContainer3");
			BuildMenuButton button4 = GetNode<BuildMenuButton>("BuildMenu/PanelContainer/VBoxContainer/HBoxContainer/VBoxContainer2/HBoxContainer2/PanelContainer4");
			BuildMenuButton button5 = GetNode<BuildMenuButton>("BuildMenu/PanelContainer/VBoxContainer/HBoxContainer/VBoxContainer2/HBoxContainer2/PanelContainer5");
			BuildMenuButton button6 = GetNode<BuildMenuButton>("BuildMenu/PanelContainer/VBoxContainer/HBoxContainer/VBoxContainer2/HBoxContainer2/PanelContainer6");
			BuildMenuButton button7 = GetNode<BuildMenuButton>("BuildMenu/PanelContainer/VBoxContainer/HBoxContainer/VBoxContainer2/HBoxContainer3/PanelContainer7");
			BuildMenuButton button8 = GetNode<BuildMenuButton>("BuildMenu/PanelContainer/VBoxContainer/HBoxContainer/VBoxContainer2/HBoxContainer3/PanelContainer8");
			BuildMenuButton button9 = GetNode<BuildMenuButton>("BuildMenu/PanelContainer/VBoxContainer/HBoxContainer/VBoxContainer2/HBoxContainer3/PanelContainer9");
			


			ButtonList.Add(button1);
			ButtonList.Add(button2);
			ButtonList.Add(button3);
			ButtonList.Add(button4);
			ButtonList.Add(button5);
			ButtonList.Add(button6);
			ButtonList.Add(button7);
			ButtonList.Add(button8);
			ButtonList.Add(button9);



			CrossHair = GetNode<TextureRect>("CrossHair");
			Bar = GetNode<TextureProgress>("HealthBar");
			WeaponIconRect = GetNode<TextureRect>("WeaponSelector/WeaponIcon/TextureRect");

			EnergyCounter = GetNode<Label>("TopBar/PanelContainer/HBoxContainer/EnergyDisplay/EnergyCount");
			MetalCounter = GetNode<Label>("TopBar/PanelContainer/HBoxContainer/MetalDisplay/MetalCount");
			CrystalCounter = GetNode<Label>("TopBar/PanelContainer/HBoxContainer/CrystalDisplay/CrystalCount");

			// Getting the build menu.
			BuildMenu = GetNode<MarginContainer>("BuildMenu");
			CloseBuildMenu(TeamID);

			// Initialize the counters to zero
			EnergyCounter.Text = "0";
			MetalCounter.Text = "0";
			CrystalCounter.Text = "0";

		}

		public override void _PhysicsProcess(float delta) {

			UpdateResources();
		}

		
		public void UpdateHealth(int value, int full) {

			Bar.MaxValue = full;
			Bar.Value = value;

		}

		public void WeaponSwap(Texture weaponIcon) {


			CurrentWeaponIcon = weaponIcon;
			WeaponIconRect.Texture = CurrentWeaponIcon;

		}

		public void UpdateResources() {

			if (TeamID != 0) {

				TeamState teamState = GameController.GetTeamState(TeamID);
				
				EnergyCounter.Text = teamState.ResourceDict[ResourceType.Energy].ToString();
				MetalCounter.Text = teamState.ResourceDict[ResourceType.Metal].ToString();
				CrystalCounter.Text = teamState.ResourceDict[ResourceType.Crystal].ToString();

			}

		}

		// This method takes a list of packed scenes and passes them to the build menu.

		public void OpenBuildMenu(Dictionary<int, PackedScene> menuItems, Node constructor) {

				// This is a top level menu, so we need to force any other open menus on this team to close
				// whenever it's activated.


				PopulateMenu(menuItems, constructor);
				BuildMenu.Show();
				CrossHair.Hide();
		}

		public void CloseBuildMenu(int teamID) {

			if (teamID == TeamID) {

				for (int i = 0; i < ButtonList.Count; i++) {

					ButtonList[i].MouseExited();
				}
				Input.SetMouseMode(Input.MouseMode.Captured);

				BuildMenu.Hide();
				CrossHair.Show();
				Page = 0;
			}
			
		}


		// Called only from functions internal to HUD, this method sets the scene and visibility for 
		// all build menu buttons.
		// If the list of available scenes is shorter than the list of buttons, the remaining
		// buttons are set as transparent.

		// Page, which is set and stored locally, is multplied by 9 to get the offset used to get scenes from the scene list.
		// Page 0 is the home page, so no offset is applied.

		private void PopulateMenu(Dictionary<int, PackedScene> sceneDict, Node constructorNode) {

			int offset = Page * 9;

			IConstructor constructor = (IConstructor)constructorNode;

			List<PackedScene> sceneList = new List<PackedScene>();

			foreach (KeyValuePair<int, PackedScene> kvp in sceneDict) {

				sceneList.Add(kvp.Value);
			}

			Console.WriteLine("Populating build menu.");

			for (int i = offset; i < (offset + 9); i++) {

				if (i < sceneList.Count) {

					BuildMenuButton tempButton = ButtonList[i - offset];
					tempButton.Modulate = new Color(tempButton.Modulate.r, tempButton.Modulate.g, tempButton.Modulate.b, 1.0f);
					tempButton.IsHidden = false;


					ViewportContainer viewportContainer= tempButton.GetNode<ViewportContainer>("ViewportContainer");
					viewportContainer.Show();

					tempButton.SetScene(sceneList[i], constructor);

					Console.WriteLine("Setting scene " + i.ToString());
				}
				else {
					Console.WriteLine("Hiding button " + i.ToString());
					BuildMenuButton tempButton = ButtonList[i - offset];

					tempButton.IsHidden = true;
					tempButton.Modulate = new Color(tempButton.Modulate.r, tempButton.Modulate.g, tempButton.Modulate.b, 0.0f);
					ViewportContainer viewportContainer = tempButton.GetNode<ViewportContainer>("ViewportContainer");
					viewportContainer.Hide();
				}

			}

		}


	}
}
