using System;
using System.Collections.Generic;
using Godot;


namespace MaxGame { 

	

	public class QueueMenu : MarginContainer {


		// Stores the parent of this menu. Should be set by the parent.

		public IConstructor Parent;

		
		// Stores the index of the *next* item to be added to the queue.
		public int Index = 0;

		
		// Used to store the waiting build queue.
		protected PackedScene[] QueueArray;

		// Stores the page, or multiplier of 9, the main menu is on.
		protected int Page = 0;

		// Helper nodes

		protected GameController GameController;
		protected SignalGenerator SignalGenerator;

		protected List<BuildMenuButton> ButtonList;
		protected List<QueueButton> QueueButtonList;





		public override void _Ready() {


			GameController = GetTree().Root.GetNode<GameController>("Node/GameController");
			SignalGenerator = GetTree().Root.GetNode<SignalGenerator>("Node/SignalGenerator");
			
			SignalGenerator.Connect("CloseMenu", this, nameof(Close));



			// Populate the button list

			ButtonList = new List<BuildMenuButton>();

			BuildMenuButton button1 = GetNode<BuildMenuButton>("PanelContainer/VBoxContainer/HBoxContainer/VBoxContainer2/HBoxContainer/PanelContainer1");
			BuildMenuButton button2 = GetNode<BuildMenuButton>("PanelContainer/VBoxContainer/HBoxContainer/VBoxContainer2/HBoxContainer/PanelContainer2");
			BuildMenuButton button3 = GetNode<BuildMenuButton>("PanelContainer/VBoxContainer/HBoxContainer/VBoxContainer2/HBoxContainer/PanelContainer3");
			BuildMenuButton button4 = GetNode<BuildMenuButton>("PanelContainer/VBoxContainer/HBoxContainer/VBoxContainer2/HBoxContainer2/PanelContainer4");
			BuildMenuButton button5 = GetNode<BuildMenuButton>("PanelContainer/VBoxContainer/HBoxContainer/VBoxContainer2/HBoxContainer2/PanelContainer5");
			BuildMenuButton button6 = GetNode<BuildMenuButton>("PanelContainer/VBoxContainer/HBoxContainer/VBoxContainer2/HBoxContainer2/PanelContainer6");
			BuildMenuButton button7 = GetNode<BuildMenuButton>("PanelContainer/VBoxContainer/HBoxContainer/VBoxContainer2/HBoxContainer3/PanelContainer7");
			BuildMenuButton button8 = GetNode<BuildMenuButton>("PanelContainer/VBoxContainer/HBoxContainer/VBoxContainer2/HBoxContainer3/PanelContainer8");
			BuildMenuButton button9 = GetNode<BuildMenuButton>("PanelContainer/VBoxContainer/HBoxContainer/VBoxContainer2/HBoxContainer3/PanelContainer9");
		

			ButtonList.Add(button1);
			ButtonList.Add(button2);
			ButtonList.Add(button3);
			ButtonList.Add(button4);
			ButtonList.Add(button5);
			ButtonList.Add(button6);
			ButtonList.Add(button7);
			ButtonList.Add(button8);
			ButtonList.Add(button9);

			// Populate the queue
			QueueButtonList = new List<QueueButton>();
			QueueButton queueButton1 = GetNode<QueueButton>("PanelContainer/VBoxContainer/PanelContainer/HBoxContainer/PanelContainer1");
			QueueButton queueButton2 = GetNode<QueueButton>("PanelContainer/VBoxContainer/PanelContainer/HBoxContainer/PanelContainer2");
			QueueButton queueButton3 = GetNode<QueueButton>("PanelContainer/VBoxContainer/PanelContainer/HBoxContainer/PanelContainer3");
			QueueButton queueButton4 = GetNode<QueueButton>("PanelContainer/VBoxContainer/PanelContainer/HBoxContainer/PanelContainer4");
			QueueButton queueButton5 = GetNode<QueueButton>("PanelContainer/VBoxContainer/PanelContainer/HBoxContainer/PanelContainer5");
			QueueButton queueButton6 = GetNode<QueueButton>("PanelContainer/VBoxContainer/PanelContainer/HBoxContainer/PanelContainer6");
			QueueButton queueButton7 = GetNode<QueueButton>("PanelContainer/VBoxContainer/PanelContainer/HBoxContainer/PanelContainer7");
			QueueButton queueButton8 = GetNode<QueueButton>("PanelContainer/VBoxContainer/PanelContainer/HBoxContainer/PanelContainer8");


			QueueButtonList.Add(queueButton1);
			QueueButtonList.Add(queueButton2);
			QueueButtonList.Add(queueButton3);
			QueueButtonList.Add(queueButton4);
			QueueButtonList.Add(queueButton5);
			QueueButtonList.Add(queueButton6);
			QueueButtonList.Add(queueButton7);
			QueueButtonList.Add(queueButton8);

			QueueArray = new PackedScene[8];
			PopulateQueue(QueueArray);


		}

		public void Open(Dictionary<int, PackedScene> menuItems, Node constructorNode) {


			PopulateMenu(menuItems, constructorNode);
			Show();

			IConstructor constructor = (IConstructor)constructorNode;
			int teamID = constructor.TeamID;

			SignalGenerator.EmitMenuOpened(teamID);
		}

		
		public void Close(int teamID) {

			if (teamID == Parent.TeamID) {
				
				Hide();
				Page = 0;

				// Sends a mouse exited signal to each button to refresh them.

				for (int i = 0; i < ButtonList.Count; i++) {

					ButtonList[i].MouseExited();
				}

				SignalGenerator.EmitMenuClosed(teamID);
			}
		}



		// Removes the first item from the list and returns the removed item.

		
		public PackedScene Pop() {

			PackedScene output = QueueArray[0];
			Dequeue(output);

			return output;
			
		}

		public void Enqueue(PackedScene item) {

			if (Index < 9) {
				QueueArray[Index] = item;
				Index += 1;
				PopulateQueue(QueueArray);
			}

			Console.WriteLine(Index.ToString());
		}

		public void Dequeue(PackedScene item) {

			Console.WriteLine(Index.ToString());

			int itemIndex = 0;

			for (int i = 0; i < Index; i++) {

				if (QueueArray[i] == item) {

					itemIndex = i;
				}
			}
			


			// for (int i = 0; i < itemIndex; i++) {

			// 	tempList.Add(QueueArray[i]);
			// }

			 for (int i = itemIndex + 1; i < Index; i++) {

			 	QueueArray[i] = QueueArray[i + 1];
			 }

			Index -= 1;

			PopulateQueue(QueueArray);
		}


		// This method sets the scene and visibility for all queue buttons.
		// Unlike setscene for the control menu, the queue takes IBuildable items directly.
		// This is primarily for convenience.
		// If the list of available scenes is shorter than the list of buttons, the remaining
		// buttons are set as transparent.

		private void PopulateQueue(PackedScene[] itemArray) {

			Console.WriteLine("Populating queue.");

			for (int i = 0; i < 8; i++) {

				
				QueueButton tempButton = QueueButtonList[i];

				if (i < Index) {

					tempButton.Modulate = new Color(tempButton.Modulate.r, tempButton.Modulate.g, tempButton.Modulate.b, 1.0f);
					tempButton.IsHidden = false;
					tempButton.Parent = this;


					ViewportContainer viewportContainer= tempButton.GetNode<ViewportContainer>("ViewportContainer");
					viewportContainer.Show();

					tempButton.SetScene(itemArray[i]);

					Console.WriteLine("Setting scene " + i.ToString());
				}
				else {
					Console.WriteLine("Hiding button " + i.ToString());
					tempButton.Parent = this;


					tempButton.IsHidden = true;
					tempButton.Modulate = new Color(tempButton.Modulate.r, tempButton.Modulate.g, tempButton.Modulate.b, 0.0f);
					ViewportContainer viewportContainer = tempButton.GetNode<ViewportContainer>("ViewportContainer");
					viewportContainer.Hide();
				}

			}

		}


		// This method sets the scene and visibility for all build menu buttons.
		// If the list of available scenes is shorter than the list of buttons, the remaining
		// buttons are set as transparent.

		// Page, which is set and stored locally, is multplied by 9 to get the offset used to get scenes from the scene list.
		// Page 0 is the home page, so no offset is applied.

		public void PopulateMenu(Dictionary<int, PackedScene> sceneDict, Node constructorNode) {

			int offset = Page * 9;

			IConstructor constructor = (IConstructor)constructorNode;

			List<PackedScene> sceneList = new List<PackedScene>();

			foreach (KeyValuePair<int, PackedScene> kvp in sceneDict) {

				sceneList.Add(kvp.Value);
			}

			//Console.WriteLine("Populating build menu.");

			for (int i = offset; i < (offset + 9); i++) {


				if (i < sceneList.Count) {

					BuildMenuButton tempButton = ButtonList[i - offset];
					tempButton.Modulate = new Color(tempButton.Modulate.r, tempButton.Modulate.g, tempButton.Modulate.b, 1.0f);
					tempButton.IsHidden = false;


					ViewportContainer viewportContainer= tempButton.GetNode<ViewportContainer>("ViewportContainer");
					viewportContainer.Show();

					tempButton.SetScene(sceneList[i], constructor);

					//Console.WriteLine("Setting scene " + i.ToString());
				}
				else {
					//Console.WriteLine("Hiding button " + i.ToString());
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
