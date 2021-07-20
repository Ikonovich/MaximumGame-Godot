using System;
using Godot;

namespace MaxGame {

	public class MissileLauncher : ProjectileLauncher {


		public override void _Ready() {

		// Getting the projectile scene.
		ProjectileScene = (PackedScene)ResourceLoader.Load("res://Actors/Missile.tscn");

		}

		public override void _PhysicsProcess(float delta) {

		}

		public void Shoot(IInteractable target) {
			


			Missile projectile = (Missile)ProjectileScene.Instance();
			Node root = GetTree().Root;
			root.AddChild(projectile);

			projectile.GlobalTransform = GlobalTransform;
			projectile.Scale = new Vector3(1,1,1);

			projectile.SetTarget(target);
			float damage = projectile.ProjectileDamage * DamageMultiplier;


			Console.WriteLine("Missile Launcher fired");
			
		}
		
		

		public void Shoot(Vector3 target) {


			Missile projectile = (Missile)ProjectileScene.Instance();
			Node root = GetTree().Root;
			root.AddChild(projectile);

			projectile.GlobalTransform = GlobalTransform;
			projectile.Scale = new Vector3(1,1,1);
			projectile.SetTarget(target);
			float damage = projectile.ProjectileDamage * DamageMultiplier;

			Console.WriteLine("Missile Launcher fired");
			


		}


	}
}
