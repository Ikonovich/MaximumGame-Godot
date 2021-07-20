using System;
using System.Collections.Generic;
using Godot;


namespace MaxGame {        
		

	public class Outline : Spatial {
		

		protected List<MeshInstance> OutlineList;


		// <remarks>
		// These floats determine how long the item is outlined after the last cursor interaction.
		// OutlineCountdown is reset to OutlineTimer at every interaction, and counted dowd in _Process.
		// </remarks>

		float OutlineTimer = 0.15f;
		float OutlineCountdown = 0.0f;


		public override void _Ready() {

			OutlineList = new List<MeshInstance>();
			foreach (Node node in GetChildren()) {

				MeshInstance tempMesh = (MeshInstance)node;

				OutlineList.Add(tempMesh);

			}
			HideOutline();
		}

		public void _Process(float delta) {

			if (OutlineCountdown > 0) {

				OutlineCountdown -= delta;

				if (OutlineCountdown <= 0) {

					HideOutline();
				}
			}
		}

		public void ShowHoverOutline() {

			if (OutlineCountdown <= 0) {

				Console.WriteLine("Outline countdown 0");


				for (int i = 0; i < OutlineList.Count; i++) {

					Console.WriteLine("Showing outline");
					OutlineList[i].Show();
				}
			}
			OutlineCountdown = OutlineTimer;

		}

		public void HideOutline() {

			for (int i = 0; i < OutlineList.Count; i++) {

				OutlineList[i].Hide();
			}

		}



	}
}
