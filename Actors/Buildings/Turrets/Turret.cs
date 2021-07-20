using System;
using Godot;

namespace MaxGame {

	/// <summary>
	/// This is a generic class for rotating turrets. It accepts node paths pointing to a weapon scene
	/// as well as horizontal and vertical pivot scenes. 
	/// 
	///
	/// This class uses an area to detect enemies within sensor 
	/// range and a float parameter Range to determine if it can hit units when requested by the player. 
	///
	/// It uses the TargetItem and TargetPoint parameters from the Building class to keep track of it's 
	/// target.
	/// It can pass either of these parameters to launched projectiles. This was necessary to allow
	/// missiles to track moving targets.
	/// 
	/// The fire rate is limited by the attached weapon rather than the turret itself.
	/// </summary>

	public class Turret : Building {

		

		[Export]
		public NodePath WeaponPath;

		[Export]
		public NodePath HorizontalPivotPath;

		[Export]
		public NodePath VerticalPivotPath;

		
		[Export]
		public NodePath FireAnimationPath;
		
		[Export]
		public float Range;

		[Export]
		public float FireTime = 3.0f;

		protected float FireCountdown = 0.0f;

		protected Weapon Weapon;

		protected Spatial HorizontalPivot;

		protected Spatial VerticalPivot;

		protected AnimationPlayer FireAnimation;


		protected Spatial LookingNode;

		protected Area SensorArea;
		



		/// This bool tracks whether or not the weapon is aimed at the target's current position.
		// If it is, it is set to true and enables firing.
		protected bool OnTarget = false;

		public override void _Ready() {
		
			Setup();
		}

		public override void Setup() {

			Weapon = GetNode<Weapon>(WeaponPath);
			HorizontalPivot = GetNode<Spatial>(HorizontalPivotPath);
			VerticalPivot = GetNode<Spatial>(VerticalPivotPath);
			FireAnimation = GetNode<AnimationPlayer>(FireAnimationPath);

			SensorArea = GetNode<Area>("TurretPackage/SensorArea");

			SelectionEffect = GetNode<Spatial>("TurretPackage/SelectionEffect");

			LookingNode = GetNode<Spatial>("TurretPackage/LookingNode");



		}

		public override void _PhysicsProcess(float delta) {


			if (FireCountdown > 0) {
				FireCountdown -= delta;
			}


			if (TargetItem != null) {

				Spatial tempItem = (Spatial)TargetItem;
				TargetPoint = tempItem.GlobalTransform.origin;
			}

			if (TargetPoint != Vector3.Zero) {

				RotateTurret(TargetPoint, delta);

				if ((OnTarget == true) && (FireCountdown <= 0)) {

					Shoot();
				}
			}
		}

		public void Shoot() {
			Weapon.Shoot();
			FireAnimation.Play("Take 001", -1, 1, false);
			FireCountdown = FireTime;
		}


		/// <param name="targetPoint"> A vector3 for the turret to aim at. </param>

		public void RotateTurret(Vector3 targetPoint, float delta) {

			LookingNode.LookAt(targetPoint, Vector3.Up);

			Console.WriteLine(LookingNode.Rotation.ToString());
			
			float yDifference = (LookingNode.Rotation.y - HorizontalPivot.Rotation.y) * 2 * (float)Math.PI;

			


			// Solving for the angle the turret needs to be at to hit the target.
			// Uses the formula theta = arctan((v^2 +- sqrt(v^4 - g(gd^2 + 2hv^2))) / gd)
			// Where v = velocity, g = gravity, d = distance, and h = height.

			float velocity = 100.0f;
			float gravity = 9.8f;
			float distance = new Vector2(targetPoint.x, targetPoint.z).Length();
			float height = targetPoint.y;


			float formulaInner = (float)Math.Pow(velocity, 4) - gravity * ((gravity * distance * distance) + (2 * height * velocity * velocity));
			double formulaOuter = (double)(velocity * velocity + Math.Sqrt(formulaInner)) / (gravity * distance);

			float theta = (float)Math.Atan(formulaOuter);

			//float xDifference = theta - ((VerticalPivot.RotationDegrees.x) / 180 * (float)Math.PI)

			float xDifference = (theta - (VerticalPivot.RotationDegrees.x) / 180 * (float)Math.PI);

			Console.WriteLine("Current angle: " + (VerticalPivot.RotationDegrees / 180 * (float)Math.PI).ToString());
			Console.WriteLine("Desired angle theta: " + theta.ToString());


			if ((Math.Abs(yDifference) > 0.05f) || (Math.Abs(xDifference)  > 0.05)) {

				OnTarget = false;

				if (Math.Abs(yDifference) > 0.01f) { 
					//Console.WriteLine("Rotating turret y");
					HorizontalPivot.RotateY(0.01f * (float)Math.Sign(yDifference));
				}

				if (Math.Abs(xDifference) > 0.01f && (Math.Abs(yDifference) < 2.0f)) {
					//Console.WriteLine("Rotating turret x");
					VerticalPivot.RotateX(0.01f * (float)Math.Sign(xDifference));
				}
			
			}
			else {
				OnTarget = true;
			}
		}
	}
}
