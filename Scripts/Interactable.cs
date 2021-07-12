using System;
using Godot;


// This class is intended to function as the basis for static world items that can be interacted
// with, including showing tool tips on mouseover, having inventory, changeable sign text, etc.
// All nodes using this script MUST have a Control object for displaying tooltips.

public class Interactable : Spatial {

	// Health/energy bar pop up

	[Export]
	public int MaxHealth = 100;

	[Export]
	public int Health = 100;

	// Determines which team the object belongs to.
	[Export]
	public int Team = 1;


	// This determines how much damage the particular instance takes.
	// 1.0 is full damage, 0.5 is 1/2 damage, 0.0 is no damage, etc.
	[Export]
	public float DamageMultiplier = 1.0f;
	
	protected HealthBar HealthBar;


	public override void _Ready() {


		HealthBar = GetNode<HealthBar>("StatusBar");
		HealthBar.UpdateHealth(Health, MaxHealth);
		
	}

	public void CursorInteract(Transform transform) {


		HealthBar.Show();
	}

	public virtual void Selected() {
		Console.WriteLine("Selected");

	}

	public void ProjectileHit(float damage, Transform projectileTransform) {

		Health -= (int)(damage * DamageMultiplier);
		HealthBar.UpdateHealth(Health, MaxHealth);

		Console.WriteLine("Projectlle Hit");
	}

}
