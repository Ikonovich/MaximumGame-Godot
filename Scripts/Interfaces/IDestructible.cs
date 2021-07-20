using System;
using Godot;

namespace MaxGame {

	public interface IDestructible : IInteractable {

		// Health stats
		int MaxHealth { get; set; }
		int Health { get; set; }

		// This determines how much damage the particular instance takes.
		// 1.0 is full damage, 0.5 is 1/2 damage, 0.0 is no damage, etc. Defaults to 1.
		float DamageMultiplier { get; set; }

		// Method that allows entities to interact with projectiles.

		// Stores what team this object is on
		int TeamID { get; set; }

		void ProjectileHit(float damage, Transform projectileTransform);
		
	}
}
