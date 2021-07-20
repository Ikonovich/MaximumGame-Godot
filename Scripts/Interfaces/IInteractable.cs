using System;
using Godot;


namespace MaxGame {

// This class is intended to function as the basis for static world items that can be interacted
// with, including showing tool tips on mouseover, having inventory, changeable sign text, etc.
// All nodes using this script MUST have a Control object for displaying tooltips.


	public interface IInteractable {

		
		bool IsSelected { get; set; }

		int TeamID { get; set; }

		void CursorInteract();

		void Selected();

		void Deselected();

		void RightSelected();

		void SetTarget(IDestructible target);

		void SetTarget(Vector3 targetPoint);

		
	}
}
