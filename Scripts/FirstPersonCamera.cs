using System;
using Godot;

namespace MaxGame {


	public class FirstPersonCamera : Camera {


		protected double ShakeMagnitude = 0;
		protected double ShakeExponent = 2.0;
		protected double ShakeDecay = 0;
		protected float NoiseIncrement = 0; 

		// Noise generator to randomize shaking.
		OpenSimplexNoise Noise;
		Random Random;

		protected Vector3 DefaultTranslation;

		public override void _Ready() {

			
			DefaultTranslation = Translation;

			//Set up noise generation

			Noise = new OpenSimplexNoise();
			Random = new System.Random();
			Noise.Seed = Random.Next();
			Noise.Octaves = 3;
			Noise.Period = 2;


		}

		public override void _PhysicsProcess(float delta) {

			if (ShakeMagnitude > 0) {
				Shake();
				ShakeMagnitude -= (ShakeDecay * delta);

				if (ShakeMagnitude <= 0) {
					Translation = DefaultTranslation;
				}
			}
		}

		// The two following methods provide a shake effect, for use with things like explosions.
		// Magnitude determines the initial strength of the effect.
		// Decay determines how quickly the magnitude decreases.
		// Initializes the shake effect.
		public void StartShake(double magnitude, double decay) {

			ShakeMagnitude = magnitude;
			ShakeDecay = decay;
		}
		// Continues the shake until the magnitude is zero or less.

		public void Shake() {
			
			float multiplier = (float)(Math.Pow(ShakeMagnitude, ShakeExponent));

			Vector2 maxOffset = new Vector2(30,20);
			float maxRoll = 0.1F;

			NoiseIncrement += 1;
			Rotation = new Vector3((maxRoll * multiplier * Noise.GetNoise2d(Noise.Seed, NoiseIncrement)), Rotation.y, Rotation.z);
			VOffset = maxOffset.x * multiplier * Noise.GetNoise2d(Noise.Seed * 2, NoiseIncrement);
			HOffset = maxOffset.y * multiplier * Noise.GetNoise2d(Noise.Seed * 3, NoiseIncrement);	;

		}


	}















}
