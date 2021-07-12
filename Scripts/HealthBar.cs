using System;
using Godot;

public class HealthBar : Sprite3D {

	private Viewport Viewport;

	private TextureProgress Bar;

	// Determines how long the status bar appears for after a single interaction.
	private float ShowDuration = 3.0f;

	// Counts down to hiding the status bar.
	private float ShowCountdown = 0.0f;


	public override void _Ready() {
		
		Viewport = GetNode<Viewport>("Viewport");
		Texture = Viewport.GetTexture();
		Bar = GetNode<TextureProgress>("Viewport/HealthBar");
		

		

	}

	public override void _PhysicsProcess(float delta) {


		// This handles hiding the healthbar after a certain period of time if
		// the unit is not selected.

		if (ShowCountdown > 0) {
			ShowCountdown -= delta;
		}

		if ((ShowCountdown <= 0)) {
			Hide();
		
		}


	}

	public void UpdateHealth(int value, int full) {

		Bar.MaxValue = full;
		Bar.Value = value;

	}

	public void ShowItem() {

		Show();

		ShowCountdown = ShowDuration;
	}

	public void HideItem() {

		Hide();
		ShowCountdown = 0;
	}
}
