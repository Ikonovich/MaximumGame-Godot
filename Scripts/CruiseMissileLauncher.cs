using System;
using Godot;
using MagicSmoke;

namespace MaxGame {

	public class CruiseMissileLauncher : Building {

		private MissileLauncher Launcher;
		
		
		public override void _Ready() {

			SelectionEffect = GetNode<Spatial>("SelectionEffect");
			SelectionEffect.Hide();


			Launcher = GetNode<MissileLauncher>("Launcher");

		}

		public override void _PhysicsProcess(float delta) {

			
		}

		public void Shoot() {

		}

		public override void SetTarget(Vector3 targetPoint) {


			Console.WriteLine("Setting cruise missile target point");
			//TargetPoint = targetPoint;

			Launcher.Shoot(targetPoint);		}

		
		public override void SetTarget(IDestructible targetItem) {


			Console.WriteLine("Setting cruise missile target object");
			//TargetItem = targetItem;
			Launcher.Shoot(targetItem);
		}



	}	
}
