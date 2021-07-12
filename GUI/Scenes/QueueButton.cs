using System;
using Godot;

namespace MaxGame {

	public class QueueButton : PanelContainer {

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

		
		// Stores the camera's offsets for this particular item.

		protected float Xoffset { get; set; } = 0.0f;

		protected float Yoffset { get; set; } = 0.0f;
	
		protected float Zoffset { get; set; } = 0.0f;

		// Determines how long the button displays the pressed style.

		protected float PressedTimer = 0.2f;
		protected float PressedCountdown = 0.0f;

		// Stores the parent of this button. Should be set by the parent.

		public QueueMenu Parent;


		public override void _Ready() {

			SignalGenerator = GetTree().Root.GetNode<SignalGenerator>("Node/SignalGenerator");

			// Connecting mouse signals
			Connect("mouse_entered", this, "MouseEntered");
			Connect("mouse_exited", this, "MouseExited");

			// Connect game controller signals

			GameController = GetTree().Root.GetNode<GameController>("Node/GameController");

			
			// Initializing the styleboxes.
			// These styleboxes are visible by default, when the mouse is hovering over the panel, and
			// briefly when the button is pressed, respecitvely.

			Resource normal = ResourceLoader.Load("res://GUI/Styles/QueueButtonNormal.tres");
			Resource hover = ResourceLoader.Load("res://GUI/Styles/QueueButtonHover.tres");
			Resource pressed = ResourceLoader.Load("res://GUI/Styles/QueueButtonHover.tres");

			NormalStyle = (StyleBoxFlat)normal;
			HoverStyle = (StyleBoxFlat)hover;
			PressedStyle = (StyleBoxFlat)pressed;


			// Setting the item scene in the modeling area.

			PreviewScene = GetNode<Node>("ViewportContainer/Viewport/MainScene/PreviewScene");

			

			// Accessing the camera and setting it's position.
			Camera = GetNode<Camera>("ViewportContainer/Viewport/MainScene/Camera");




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

			}
		}       

		public void MouseExited() {

			Console.WriteLine("Mouse exiting");
			// Reverts to the normal stylebox and resets the pressed counter.
			AddStyleboxOverride("panel", NormalStyle);
			PressedCountdown = 0.0f;
		}

		public override void _GuiInput(InputEvent @event) {


			if (Input.IsActionJustReleased("shoot") && (IsHidden == false)) {

				Parent.Dequeue(ItemScene);

				PressedCountdown = PressedTimer;


			}
		}



		public void SetScene(PackedScene itemScene) {


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

			PreviewScene.AddChild(Item as Spatial);
			
			Camera.Translation = new Vector3(Xoffset, Yoffset, Zoffset);

		}

	}
}

