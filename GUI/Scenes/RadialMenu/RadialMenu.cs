using System;
using System.Collections.Generic;
using Godot;

namespace MaxGame {

		// <summary>
		//  This class directs the creation and positioning of the radial menu buttons when 
		// the radial menu is brought up. 
		// It can hold up to 6 buttons.
		// When the menu is closed, this class tells each button instance to destroy itself.
		//
		// </summary>

	
	public class RadialMenu : Spatial {

		[Export]
		public PackedScene ButtonOneScene;

		[Export]
		public PackedScene ButtonTwoScene;

		[Export]
		public PackedScene ButtonThreeScene;

		[Export]
		public PackedScene ButtonFourScene;

		[Export]
		public PackedScene ButtonFiveScene;

		[Export]
		public PackedScene ButtonSixScene;

		protected List<PackedScene> ButtonSceneList;

		protected List<RadialButton> ButtonList;

		// Stores the TeamID. Must be assigned from the parent Unit.
		public int TeamID { get; set; }

		public Spatial Parent;

		// <remarks>
		// This stores the active player unit for orienting the menu towards them.
		// </remarks>

		protected PlayableUnit Player;

		protected SignalGenerator SignalGenerator;

		protected bool MenuOpen = false;


		// <remarks>
		// Stores the radius of the menu. The first button will be placed directly above the unit at this
		// distance, while remaining buttons will be rotated off of it by 2PI/ButtonCount
		// In ShowMenu, this is set to the square root of the distance betwen the user and the menu.
		// </remarks>

		protected float Radius;


		public override void _Ready() {

			
			SignalGenerator = GetTree().Root.GetNode<SignalGenerator>("Node/SignalGenerator");
			SignalGenerator.Connect("PlayerSet", this, nameof(SetPlayer));

			Parent = (Spatial)GetParent();

			ButtonSceneList = new List<PackedScene>();

			ButtonSceneList.Add(ButtonOneScene);
			ButtonSceneList.Add(ButtonTwoScene);
			ButtonSceneList.Add(ButtonThreeScene);
			ButtonSceneList.Add(ButtonFourScene);
			ButtonSceneList.Add(ButtonFiveScene);
			ButtonSceneList.Add(ButtonSixScene);




		}

		public override void _PhysicsProcess(float delta) {

			if (MenuOpen == true) {


				Console.WriteLine("Moving unit radial menu");

				// Gets the distance between this unit and the player and uses it to make 
				// the size and height of the menu follow the inverse square law.
				
				GlobalTransform = Parent.GlobalTransform;

				double distance = (Double)(GlobalTransform.origin - Player.GlobalTransform.origin).Length();
				float radius = (float)Math.Pow(distance, (1.0/3.0));
				float height = (float)Math.Sqrt(distance);

				Vector3 circularVector = new Vector3(0.0f, radius, 0.0f);

				float offset = 2 * (float)Math.PI / ButtonSceneList.Count;


				// Causes the menu to point at the player.
				LookAt(Player.GlobalTransform.origin, Vector3.Up);

				Translation += new Vector3(0.0f, height * 3.0f, 0.0f);



				for (int i = 0; i < ButtonList.Count; i++) {
						
					RadialButton tempButton = ButtonList[i];
				

					tempButton.GlobalTransform = Parent.GlobalTransform;
					Vector3 buttonPosition = circularVector.Rotated(Vector3.Forward, offset * i);
					tempButton.Translation = buttonPosition * radius;
					tempButton.LookAt(Player.GlobalTransform.origin, Vector3.Up);
					tempButton.Scale = tempButton.DefaultScale * (float)distance / 100;

					tempButton.TeamID = TeamID;


					// if (distance > 50) {

					// 	tempButton.Scale = tempButton.DefaultScale * radius / 2;
					// }



				}
				
			}
		}


		public void ShowMenu() {

			
			if (MenuOpen == false) {
				ButtonList = new List<RadialButton>();

				for (int i = 0; i < ButtonSceneList.Count; i++) {

					if (ButtonSceneList[i] != null) {

						Console.WriteLine("Adding button " + i.ToString() + " of " + ButtonSceneList.Count.ToString());

						
						RadialButton tempButton = (RadialButton)ButtonSceneList[i].Instance();
						ButtonList.Add(tempButton);
						AddChild(tempButton);

					}


				}
				MenuOpen = true;
			}
		}


		public void HideMenu() {
			
			if (ButtonList != null) {

				for (int i = 0; i < ButtonList.Count; i++) {

					RemoveChild(ButtonList[i]);

				}
				MenuOpen = false;
			}
		}



		public void PopulateMenu(List<RadialButton> buttonList) {

			ButtonList = buttonList;

		}


		// <remarks>
		//  This method is activated by the PlayerSet signal from the signal generator,
		// which is called by any playable unit when the player switches to it.
		// </remarks>
		public void SetPlayer(PlayableUnit player, int teamID) {

			Console.WriteLine("Setting player");


			//if (teamID == Parent.TeamID) {

				Console.WriteLine("Setting player check passed");

			Player = player;
			//}
		}
	}

}
