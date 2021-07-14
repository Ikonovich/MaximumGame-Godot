using System;
using Godot;


public class RadialMenuPrototype : Area {

	// Determines the radius of the open menu
	[Export]
	int Radius = 360;

	// Controls how quickly the tween opens the menu. Smaller numbers are faster.
	[Export]
	float Speed = 1.0f;

	private TextureButton TopButton;
	private Control Buttons;
	private Tween OpenAnimation;

	// This bool tracks whether the menu is currently open or closed
	private bool Open = false;

	
	// Keeps track of the number of children this node has. Used to determine button spacing. 
	protected int ChildCount;

	public override void _Ready() {


		TopButton = GetNode<TextureButton>("Sprite3D/Viewport/TextureButton");

		OpenAnimation = GetNode<Tween>("Tween");

		OpenAnimation.Connect("tween_all_completed", this, "ToggleState");

		Connect("pressed", this, "OpenClose");



		 // Set and hide the buttons

		Buttons = GetNode<Control>("Buttons");
		//Buttons.Hide();

		// get number of children
		ChildCount = GetChildCount();


		// Hide the children behind the central texture

		foreach (Node childActual in Buttons.GetChildren()) {

			
			Area child = (Area)childActual;
			child.GlobalTransform = GlobalTransform;
			child.Hide();

		}
	}

	// Handles opening and closing the menu.
	public virtual void OpenClose() {

		// Disable the button while the menu is opening.
		TopButton.Disabled = true;

		if (Open) {
			Hide();
		}
		else {
			Show();
		}


	}

	// This toggles the stored state of the menu from Open to Close or vice versa upon
	// completion of the tween.
	public virtual void ToggleState() {

		TopButton.Disabled = false;
		Open = !Open;

		if(!Open) {

			Hide();
		}
		else {
			Show();
		}	


	}

	public virtual void Hide() {


	}

	public virtual void Show() {

		// Calculating the radial spacing between the objects.
		float spacing = (( 2 * (float)Math.PI) / ChildCount);

		// Circular positioning vector
		Vector3 circVector = new Vector3(Radius, 0, 0);

		// Scaling vector
		Vector3 scalingVector = new Vector3(0.5f, 0.5f, 0.5f);


		foreach (Node childActual in Buttons.GetChildren()) {

			// Casting the nodes to TextureButtons

			Area child = (Area)childActual;

			// Calculates the spacing for an individual button.
			float itemSpacing = ((spacing * Buttons.GetPositionInParent()) - ((float)Math.PI / 2));

			// Calculates the final position for an individual button.
			Vector3 destination = (child.GlobalTransform.origin + circVector.Rotated(Vector3.Up, itemSpacing));



			// Interpolates between the current position and the final position of the buttons.
			OpenAnimation.InterpolateProperty(child, "global_transform", child.GlobalTransform.origin, 
			destination, Speed, Tween.TransitionType.Back,Tween.EaseType.Out);


			//OpenAnimation.InterpolateProperty(child, "rect_scale", scalingVector, Vector2.One, 
			//Speed, Tween.TransitionType.Linear);
		}



	}
}
