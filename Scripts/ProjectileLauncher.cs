using System;
using Godot;

namespace MaxGame {

	public class ProjectileLauncher : Weapon {


		// How much this launcher will increase the damage of it's projectiles.
		[Export]
		public float DamageMultiplier = 1.0f;


		// Determines whether or not the weapon is enabled.
		[Export]
		public bool IsWeaponEnabled = true;

		[Export]
		public PackedScene ProjectileScene;

		public override void _Ready() {

		}

		public override void _PhysicsProcess(float delta) {

			
		}

		public override void Shoot() {

			Console.WriteLine("Cannon firing");
		

			Projectile projectile = (Projectile)ProjectileScene.Instance();
			Node root = GetTree().Root;
			root.AddChild(projectile);

			projectile.GlobalTransform = GlobalTransform;
			projectile.Scale = new Vector3(4,4,4);

			float damage = projectile.ProjectileDamage * DamageMultiplier;
			

		}


	}
}
