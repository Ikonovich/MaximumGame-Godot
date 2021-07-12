using System;
using Godot;
using MagicSmoke;

namespace MaxGame {

	public class CruiseMissileLauncher : StaticTurret {

		private MissileLauncher Launcher;
		
		public override void _Ready() {

			DebugRenderer = GetNode<DebugRenderer>(DebugRenderer.ComponentPath);
			//DetectionArea = GetNode<Area>("DetectionArea");

			//DetectionArea.Connect("body_entered", this, "TargetDetected");
			//DetectionArea.Connect("body_exited", this, "TargetGone");

			//HealthBar = GetNode<HealthBar>("StatusBar");
			//HealthBar.UpdateHealth(Health, MaxHealth);

			Launcher = GetNode<MissileLauncher>("Launcher");

		}

		public override void _PhysicsProcess(float delta) {

			
			
			if (Input.IsActionJustPressed("missile_test")) {
				Shoot();
			}
		}

		public override void Shoot() {

			Launcher.Shoot();
		}

	}
}
