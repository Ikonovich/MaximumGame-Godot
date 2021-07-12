using System;
using System.Collections.Generic;
using Godot;

namespace MaxGame { 

	// <summary>
	// This class is used by classes that need to emit or detect global signals. 
	// </summary>

	
	public class SignalGenerator : Node {
	

	
			
		// This signal is emitted whenever the resources for a team change. It signals all HUDs and buttons
		// (not just the ones for one team) tp update their available resources.

		[Signal]
		public delegate void UpdateResources();

		// Called to alert the team HUD that the build menu is being open.
		[Signal]
		public delegate void MenuOpened(int teamID);

		[Signal]
		public delegate void MenuClosed(int teamID);
		[Signal]
		public delegate void MenuTakeover(int teamID);


		// This closes any open menu for a particular player.
		[Signal]
		public delegate void CloseMenu(int teamID);
		
		//  This event is called whenever a button from the build menu is pressed.
		[Signal]
		public delegate void BuildButtonPressedEvent(IBuildable buildItem, Node sourceNode);

		// These events activate and deactivate the RaySelector's build mode.
		[Signal]
		public delegate void EnterBuildMode();
		
		[Signal]
		public delegate void ExitBuildMode();


		public void EmitUpdateResources() {

			EmitSignal(nameof(UpdateResources));

		}

		public void EmitBuildButtonPressedEvent() {

			EmitSignal(nameof(BuildButtonPressedEvent));

		}

		public void EmitEnterBuildMode() {

			EmitSignal(nameof(EnterBuildMode));

		}

		public void EmitExitBuildMode() {

			EmitSignal(nameof(ExitBuildMode));

		}

		public void EmitMenuOpened(int teamID) {


			Console.WriteLine("Open build menu signal sent");

			EmitSignal(nameof(MenuOpened), teamID);
		}

		public void EmitMenuClosed(int teamID) {

			EmitSignal(nameof(MenuClosed), teamID);
		}

		public void EmitMenuTakeover(int teamID) {

			EmitSignal(nameof(MenuTakeover), teamID);
		}

		public void EmitCloseMenu(int teamID) {

			EmitSignal(nameof(CloseMenu), teamID);
		}
	}
}
