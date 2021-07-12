using System;
using Godot;

namespace MaxGame {

	public class ProjectileLauncher : Weapon {


		// How much this launcher will increase the damage of it's projectiles.
		[Export]
		public float DamageMultiplier = 1.0f;

		
		// How long the unit takes between shots, in seconds.
		[Export]
		public float ReloadTime = 1.0f;

		// Countdown to next shot.
		protected float ReloadCountdown = 0.0f;


		// Determines whether or not the weapon is enabled.
		[Export]
		public bool IsWeaponEnabled = true;


		protected PackedScene ProjectileScene;

		public override void _Ready() {

			// Getting the projectile scene.
			ProjectileScene = (PackedScene)ResourceLoader.Load("res://Actors/Bullet.tscn");

		}

		public override void _PhysicsProcess(float delta) {

			if (ReloadCountdown > 0) {
				ReloadCountdown -= delta;
			}

		}

		public override void Shoot() {

			Console.WriteLine("Cannon firing");
		
			if (ReloadCountdown <= 0) {

				Projectile projectile = (Projectile)ProjectileScene.Instance();
				Node root = GetTree().Root;
				root.AddChild(projectile);

				projectile.GlobalTransform = GlobalTransform;
				projectile.Scale = new Vector3(4,4,4);

				float damage = projectile.ProjectileDamage * DamageMultiplier;

				ReloadCountdown = ReloadTime;
				
			}

		}


	}
}
