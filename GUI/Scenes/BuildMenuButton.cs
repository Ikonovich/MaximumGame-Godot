using System;
using Godot;

namespace MaxGame {

	public class BuildMenuButton : PanelContainer {

		GameController GameController;
		SignalGenerator SignalGenerator;

		// Keeps track of whether or not this button is hidden..
		public bool IsHidden = false;

		protected StyleBoxFlat NormalStyle;
		protected StyleBoxFlat HoverStyle;
		protected StyleBoxFlat PressedStyle;

		// Stores the scene that this button displays.

		protected PackedScene ItemScene;

		protected Node PreviewScene;

		protected IBuildable Item;

		// Stores the camera for the scene.
		protected Camera Camera;

		// Stores the cost display pop up
		public Popup CostPopup;
		
		// Stores the camera's offsets for this particular item.

		protected float Xoffset { get; set; } = 0.0f;

		protected float Yoffset { get; set; } = 0.0f;
	
		protected float Zoffset { get; set; } = 0.0f;

		// Stores the text boxes displaying the resource cost / available resources.

		protected RichTextLabel ItemName;
		protected RichTextLabel EnergyLabel;
		protected RichTextLabel CrystalLabel;
		protected RichTextLabel MetalLabel;

		// Determines how long the button displays the pressed style.

		protected float PressedTimer = 0.2f;
		protected float PressedCountdown = 0.0f;

		// Stores an IConstructor to call the Build method.
		protected IConstructor Constructor;

		public override void _Ready() {

			SignalGenerator = GetTree().Root.GetNode<SignalGenerator>("Node/SignalGenerator");

			// Connecting mouse signals
			Connect("mouse_entered", this, "MouseEntered");
			Connect("mouse_exited", this, "MouseExited");

			// Connect game controller signals

			GameController = GetTree().Root.GetNode<GameController>("Node/GameController");
			SignalGenerator.Connect("UpdateResources", this, "UpdateResources");

			// Getting the text boxes
			ItemName = GetNode<RichTextLabel>("Node/Popup/PanelContainer/VBoxContainer/HBoxContainer1/Name");
			EnergyLabel = GetNode<RichTextLabel>("Node/Popup/PanelContainer/VBoxContainer/HBoxContainer2/PowerLabel");
			MetalLabel = GetNode<RichTextLabel>("Node/Popup/PanelContainer/VBoxContainer/HBoxContainer3/MetalLabel");
			CrystalLabel = GetNode<RichTextLabel>("Node/Popup/PanelContainer/VBoxContainer/HBoxContainer4/CrystalLabel");

			// Initializing the styleboxes.
			// These styleboxes are visible by default, when the mouse is hovering over the panel, and
			// briefly when the button is pressed, respecitvely.

			Resource normal = ResourceLoader.Load("res://Game Assets/GUI/Styles/ButtonNormal.tres");
			Resource hover = ResourceLoader.Load("res://Game Assets/GUI/Styles/ButtonHover.tres");
			Resource pressed = ResourceLoader.Load("res://Game Assets/GUI/Styles/ButtonPressed.tres");

			NormalStyle = (StyleBoxFlat)normal;
			HoverStyle = (StyleBoxFlat)hover;
			PressedStyle = (StyleBoxFlat)pressed;


			// GSetting the item scene in the modeling area.

			PreviewScene = GetNode<Node>("ViewportContainer/Viewport/MainScene/PreviewScene");


			// Accessing the camera and setting it's position.
			Camera = GetNode<Camera>("ViewportContainer/Viewport/MainScene/Camera");

			// Getting the cost pop up
			CostPopup = GetNode<Popup>("Node/Popup");


		}

		public override void _PhysicsProcess(float delta) {


			// Handles style reversion after the button is pressed.
			// Reverts to hover style because exiting the panel after button press reverts to normal already.
			// This means that if this method gets activated, the mouse is still inside the panel.

			if (PressedCountdown > 0) {
				PressedCountdown -= delta;

				if (PressedCountdown <= 0) {
					AddStyleboxOverride("panel", HoverStyle);
				}
			}
		}


		public void MouseEntered() {

			if ((IsHidden == false) && (PressedCountdown <= 0)) {

				// Changes the stylebox, changing the color of the outline.
				AddStyleboxOverride("panel", HoverStyle);

				// Shows the cost popup as a modal.

				CostPopup.Show();
				CostPopup.RectGlobalPosition = RectGlobalPosition + new Vector2(200.0f, 250.0f);
				
			}
		}       

		public void MouseExited() {

			Console.WriteLine("Mouse exiting");

			// Reverts to the normal stylebox and resets the pressed counter.
			AddStyleboxOverride("panel", NormalStyle);
			PressedCountdown = 0.0f;
			CostPopup.Hide();
		}

		public override void _GuiInput(InputEvent @event) {

			//if (@event is InputEventMouseButton mouseEvent && mouseEvent.ButtonIndex == (int)ButtonList.Left && mouseEvent.Pressed) {

			if (Input.IsActionJustReleased("shoot") && (IsHidden == false)) {


				// Sets the pressed style and countdown timer.
				PressedCountdown = PressedTimer;
				AddStyleboxOverride("panel", PressedStyle);


				PackedScene tempScene = (PackedScene)ItemScene.Duplicate();



				Constructor.Build(tempScene);

				PressedCountdown = PressedTimer;
				CostPopup.Hide();


			}
		}

		// Updates the resource text labels, using the player's Team ID to get them from the GameController.

		public void UpdateResources() {
			
			if (Item != null) {
				TeamState state = GameController.GetTeamState(Constructor.TeamID);


				Console.WriteLine(state.ResourceDict[ResourceType.Energy].ToString());
				Console.WriteLine(state.ResourceDict[ResourceType.Metal].ToString());
				Console.WriteLine(state.ResourceDict[ResourceType.Crystal].ToString());
				
				ItemName.Text = Item.Name;
				EnergyLabel.Text = Item.EnergyCost.ToString() + " / " + state.ResourceDict[ResourceType.Energy].ToString();
				MetalLabel.Text = Item.MetalCost.ToString() + " / " + state.ResourceDict[ResourceType.Metal].ToString();
				CrystalLabel.Text = Item.CrystalCost.ToString() + " / " + state.ResourceDict[ResourceType.Crystal].ToString();
			}

		}

		public void SetScene(PackedScene itemScene, IConstructor constructor) {

			Constructor = constructor;

			if (PreviewScene.GetChildCount() > 0) {
				var child = PreviewScene.GetChild(0);
				PreviewScene.RemoveChild(child);

			}

			Console.WriteLine("Setting button scene");
			ItemScene = itemScene;

			Item = (IBuildable)ItemScene.Instance();

			Item.IsPreviewScene = true;

			// Getting offsets for the camera. 

			Xoffset = Item.PreviewXoffset;
			Yoffset = Item.PreviewYoffset;
			Zoffset = Item.PreviewZoffset;

			UpdateResources();

			PreviewScene.AddChild(Item as Spatial);
			
			Camera.Translation = new Vector3(Xoffset, Yoffset, Zoffset);

		}

	}
}

