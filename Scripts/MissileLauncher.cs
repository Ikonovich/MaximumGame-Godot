using System;
using Godot;

namespace MaxGame {

	public class MissileLauncher : ProjectileLauncher {


		public override void _Ready() {

		// Getting the projectile scene.
		ProjectileScene = (PackedScene)ResourceLoader.Load("res://Actors/Missile.tscn");

		}

		public override void _PhysicsProcess(float delta) {

			if (ReloadCountdown > 0) {
				ReloadCountdown -= delta;
			}
		}

		public override void Shoot() {
		
			if (ReloadCountdown <= 0) {

				Missile projectile = (Missile)ProjectileScene.Instance();
				Node root = GetTree().Root;
				root.AddChild(projectile);

				projectile.GlobalTransform = GlobalTransform;
				projectile.Scale = new Vector3(1,1,1);

				float damage = projectile.ProjectileDamage * DamageMultiplier;

				ReloadCountdown = ReloadTime;

				Console.WriteLine("Missile Launcher fired");

				
			}

		}


	}
}
