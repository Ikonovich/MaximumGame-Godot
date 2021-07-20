using System;
using Godot;


namespace MaxGame {

	public class RadialButton : StaticBody, IInteractable {

		protected Outline Outline;

		public Vector3 DefaultScale;

		public int TeamID { get; set; }
		
		public bool IsSelected { get; set; } = false;


		public override void _Ready() {

			Setup();
		}

		public void Setup() {

			Outline = GetNode<Outline>("Outline");

			DefaultScale = this.Scale;
			
		}


		public void DestroyButton() {

			QueueFree();
		}

		// Begin IInteractable implementation

		public void CursorInteract() {

			Outline.ShowHoverOutline();
		}

		public virtual void Selected() {

		}

		public virtual void Deselected() {


		}
		
		public virtual void RightSelected() {


			
		}
		
		public virtual void SetTarget(IDestructible target) {
			
	
		}
		
		public virtual void SetTarget(Vector3 target) {
		
		}

		// End IInteractable implementation
	}
}
