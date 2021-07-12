using Godot;
using System;

public class AlienUnarmed : KinematicBody
{
	
	// This should appear whenever the user's mouse enters
	// the space of the object for more than a moment.
	
	private Popup popup;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GetNode("CollisionShape").Connect("mouse_entered", this, nameof(OnMouseEntered));
		
		popup = GetNode<Popup>("Popup");
		
		popup.PopupCenteredRatio(0.75f);
		
		
	}
	
	public void OnMouseEntered() {
		
	
	}

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
