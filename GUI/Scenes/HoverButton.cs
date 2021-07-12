using System;
using Godot;

namespace MaxGame {

	public class HoverButton : Area, IInteractable {

		private TextureButton Button;
		private Viewport Viewport;
		private Sprite3D Sprite;
		private GameController GameController;

		// Controls how long the hover button shows after a cursor interaction.
		private float ShowDuration = 2.0F;
		private float ShowCountdown = 0.0f;
		
		// This stores the parent node. For a single layer button that is the Owner parameter, or 
		// for a second layer button it will be Owner.Owner. Etc.
		PlayableUnit Parent;


		public override void _Ready() {

			GameController = GetTree().Root.GetNode<GameController>("Node/GameController");


			Sprite = GetNode<Sprite3D>("Sprite3D");
			Button = GetNode<TextureButton>("Sprite3D/Viewport/TextureButton");
			Viewport = GetNode<Viewport>("Sprite3D/Viewport");

			Sprite.Texture = Viewport.GetTexture();

			// Set the parent node.
			Parent = (PlayableUnit)Owner;
			
		}

		public override void _PhysicsProcess(float delta) {

			// This code handles hiding the button after a certain period of time,

			if (ShowCountdown > 0) {
				ShowCountdown -= delta;

				if (ShowCountdown <= 0) {
					Hide();
				}
			}

		}

		public void CursorInteract() {



			Button.Pressed = true;

		}

		public void Selected() {

			if (ShowCountdown > 0) {

				GameController.SetPlayer(Parent);
			}


		}

		public void Deselected() {

		}

		public void ShowItem() {

			ShowCountdown = ShowDuration;
			Show();
		}

		
		public void HideItem() {

			ShowCountdown = 0;
			Hide();
		}

	}
}
