using Godot;

namespace MagicSmoke {

	/// <summary>
	/// This class provides an extremely fancy orbit camera that is capable of recentering itself behind or in front of its target when idle, as well as avoiding obstacles such as terrain by zooming in.<br />
	/// Camera will orbit the node specified in its configuration. Preferably, camera and the target node should be at the same level of the scene tree.<br />
	/// The camera's offset from its target is expressed in terms of elevation, azimuth and distance.
	/// </summary>
	public class OrbitCamera : Camera {

		#region Camera Configuration 

		#region Camera Configuration: Behavior

		/// <summary>
		/// Whether the camera should position itself behind its target or in front
		/// </summary>
		[Export]
		public bool PositionBehindTarget {get; set;} = true;

		/// <summary>
		/// Whether the camera should use acceleration when zooming, or use simple linear zooming
		/// </summary>
		[Export]
		public bool UseZoomAcceleration { get; set; } = true;

		/// <summary>
		/// Whether the camera should attempt to recenter itself when idle
		/// </summary>
		[Export]
		public bool RecenterWhenIdle { get; set; } = true;

		/// <summary>
		/// Whether the camera should attempt to avoid clipping through obstacles
		/// </summary>
		[Export]
		public bool AvoidObstacles { get; set; } = true;

		#endregion

		#region Camera Configuration: Obstacle Avoidance

		/// <summary>
		/// Collision mask to use with the obstacle detection area
		/// </summary>
		[Export(PropertyHint.Layers3dPhysics)]
		public uint ObstacleDetectionCollisionMask { get; set; }

		/// <summary>
		/// Collision layer to use with the obstacle detection area
		/// </summary>
		[Export(PropertyHint.Layers3dPhysics)]
		public uint ObstacleDetectionCollisionLayer { get; set; }

		/// <summary>
		/// Radius of the sphere used to detect clipping through obstacles
		/// </summary>
		[Export]
		public float ObstacleDetectionRadius { get; set; } = 0.25f;

		/// <summary>
		/// How fast the camera should zoom in when it's colliding in with an obstacle
		/// </summary>
		[Export]
		public float ObstacleAvoidanceSpeed { get; set; } = 10f;

		#endregion

		#region Camera Configuration: Offset and Bounds

		/// <summary>
		/// Current elevation of the camera
		/// </summary>
		[Export]
		public float Elevation { get; set; } = 0f;

		/// <summary>
		/// Current azimuth of the camera
		/// </summary>
		[Export]
		public float Azimuth { get; set; } = 0f;

		/// <summary>
		/// Current distance of the camera
		/// </summary>
		[Export]
		public float Distance { get; set; } = 4f;

		/// <summary>
		/// Minimum distance of the camera from its target
		/// </summary>
		[Export]
		public float MinimumDistance { get; set; } = 1f;

		/// <summary>
		/// Maximum distance of the camera from its target
		/// </summary>
		[Export]
		public float MaximumDistance { get; set; } = 50f;

		/// <summary>
		/// Minimum camera elevation
		/// </summary>
		[Export]
		public float MinimumElevation { get; set; } = -45f;

		/// <summary>
		/// Maximum camera elevation
		/// </summary>
		[Export]
		public float MaximumElevation { get; set; } = 45f;

		#endregion

		#region Camera Configuration: Mouse and Zoom Sensitivity

		/// <summary>
		/// How sensitive the camera is to mouse movement, in radians per pixel of mouse acceleration in the X and Y directions
		/// </summary>
		[Export]
		public Vector2 MouseSensitivity {get; set;} = new Vector2(0.8f, 0.5f); 

		/// <summary>
		/// Units of zoom per mouse wheel movement per second
		/// </summary>
		[Export]
		public float ZoomSensitivity { get; set; } = 15f; 

		#endregion

		#region Camera Configuration: Recentering

		/// <summary>
		/// When recentering the camera, deadzone where its orientation is close enough to the target's orientation
		/// </summary>
		[Export]
		public float RecenterDeadzone { get; set; } = 0.01f;

		/// <summary>
		/// How fast the camera will recenter itself, in degrees per second
		/// </summary>
		[Export]
		public float RecenterSpeed { get; set; } = 80f;

		/// <summary>
		/// How long the camera will wait for no user input before recentering itself
		/// </summary>
		[Export]
		public float RecenterTimeoutSeconds { get; set; } = 1f;

		#endregion

		#region Camera Configuration: Motion

		/// <summary>
		// Time without user camera movement before the camera's velocity will start to decay, in seconds
		/// </summary>
		[Export]
		public float VelocityDecayTimeoutSeconds {get; set;} = 0.5f;

		/// <summary>
		/// Velocity decay rate for the camera's motion
		/// </summary>
		[Export]
		public float VelocityDecayRate { get; set; } = 4f;

		/// <summary>
		/// Acceleration decay rate for the camera's motion
		/// </summary>
		[Export]
		public float AccelerationDecayRate { get; set; } = 8f;

		/// <summary>
		/// Maximum velocity the camera can move at
		/// </summary>
		[Export]
		public float MaxLinearVelocity { get; set; } = 10f;

		#endregion

		#region Camera Configuration: Target

		protected NodePath cameraTargetPath;
		/// <summary>
		/// Path to the camera's current target node. Can be changed at runtime.
		/// </summary>
		[Export]
		public NodePath CameraTargetPath {
			get => cameraTargetPath;
			set {
				cameraTargetPath = value;
				if(initialized) UpdateTarget();
			}
		}

		/// <summary>
		/// Reference to the current target node
		/// </summary>
		public Spatial TargetNode { get; protected set; }

		#endregion
	  
		#endregion // End config region
		protected Area obstacleTestArea;
		
		protected MotionState cameraMotion;
		protected float lastMovementTimeSeconds;
		protected bool initialized;

		public override void _Ready() {
			// Update the obstacle test area's collider shape and layer/mask
			obstacleTestArea = GetNode<Area>("ObstacleTestArea");
			obstacleTestArea.CollisionLayer = ObstacleDetectionCollisionLayer;
			obstacleTestArea.CollisionMask = ObstacleDetectionCollisionMask;
			var areaCollider = obstacleTestArea.GetNode<CollisionShape>("CollisionShape");
			areaCollider.Shape = new SphereShape {
				Radius = ObstacleDetectionRadius
			};
			obstacleTestArea.Monitoring = AvoidObstacles;

			// Initialize the camera's motion state
			cameraMotion = new MotionState() {
				AccelerationDecayRate = AccelerationDecayRate,
				VelocityDecayRate = VelocityDecayRate,
				MaxVelocity = MaxLinearVelocity,
				UseAccelerationDecay = true,
				UseVelocityDecay = false
			};
			lastMovementTimeSeconds = OS.GetTicksMsec() / 1000f;
			UpdateTarget();
			initialized = true;
		}

		public override void _Process(float delta) {
			
			// Update the camera's motion
			cameraMotion.Update(delta);

			// Apply the motions' X and Y velocity to azimuth and elevation
			Azimuth += cameraMotion.Velocity.x * delta;
			Elevation += cameraMotion.Velocity.y * delta;
			// Apply the motion's Z velocity to the camera distance, if configured
			if(UseZoomAcceleration) Distance += cameraMotion.Velocity.z * delta;

			// Restrict the elevation and distance as configured
			Elevation = Mathf.Clamp(Elevation, -MaximumElevation, MaximumElevation);
			Distance = Mathf.Clamp(Distance, MinimumDistance, MaximumDistance);

			var now = OS.GetTicksMsec() / 1000f;

			cameraMotion.UseVelocityDecay = now - lastMovementTimeSeconds > VelocityDecayTimeoutSeconds;

			if(RecenterWhenIdle) RecenterCameraIfIdle(delta);
			if(AvoidObstacles) CheckObstaclesAndAvoid(delta);

			UpdateTransform();            
		}

		protected void UpdateTransform() {
			if(TargetNode != null) LookAtFromPosition(TargetNode.GlobalTransform.origin + CalculateAngularOffset(), TargetNode.GlobalTransform.origin, Vector3.Up);
		}

		protected void UpdateTarget() {
			try {
				if(CameraTargetPath == null) throw new System.Exception($"CameraTargetPath is null!");
				TargetNode = GetNode<Spatial>(CameraTargetPath);
				UpdateTransform();
			}
			catch(System.Exception e) {
				GD.PushWarning($"OrbitCamera: Error setting target node, is the path valid and pointing at a Spatial node?: {e.Message}");
			}
		}


		/// <summary>
		/// Calculate the camera's offset from its target 
		/// </summary>
		protected Vector3 CalculateAngularOffset() {

			var horizontalDistance = Distance * Mathf.Cos(Mathf.Deg2Rad(Elevation));
			var verticalDistance = Distance * Mathf.Sin(Mathf.Deg2Rad(Elevation));

			var offsetX = horizontalDistance * Mathf.Sin(Mathf.Deg2Rad(PositionBehindTarget ? Azimuth : Azimuth + 180f));
			var offsetZ = horizontalDistance * Mathf.Cos(Mathf.Deg2Rad(PositionBehindTarget ? Azimuth : Azimuth + 180f));

			return new Vector3(offsetX, verticalDistance, offsetZ);
		}

		public override void _UnhandledInput(InputEvent inputEvent) {
			// Check if the mouse is moving
			if(inputEvent is InputEventMouseMotion mouseEvent && Input.GetMouseMode() == Input.MouseMode.Captured) {
				// We'll only pan left or right if the player is holding the panCamera button
				if(Input.IsActionPressed("panCamera")) {
					cameraMotion.ApplyAcceleration(new Vector3(-mouseEvent.Relative.x * MouseSensitivity.x, 0f, 0f));
					lastMovementTimeSeconds = OS.GetTicksMsec() / 1000f;
				}
				// Pan up or down regardless of the panCamera button
				cameraMotion.ApplyAcceleration(new Vector3(0f, mouseEvent.Relative.y * MouseSensitivity.y, 0f));
			}
			if(inputEvent is InputEventMouseButton buttonEvent && Input.GetMouseMode() == Input.MouseMode.Captured) {
				// Check if the mouse wheel is moving
				if(buttonEvent.IsPressed()) {
					if(buttonEvent.ButtonIndex == (int)ButtonList.WheelUp) {
						if(UseZoomAcceleration) cameraMotion.ApplyAcceleration(new Vector3(0f, 0f, -ZoomSensitivity));
						else Distance -= ZoomSensitivity * GetProcessDeltaTime();
					}
					if(buttonEvent.ButtonIndex == (int)ButtonList.WheelDown) {
						if(UseZoomAcceleration) cameraMotion.ApplyAcceleration(new Vector3(0f, 0f, ZoomSensitivity));
						else Distance += ZoomSensitivity * GetProcessDeltaTime();
					}
				}
			}
		}

		/// <summary>
		/// Recenter the camera behind or in front of the target node, if the idle timeout has expired
		/// </summary>
		protected void RecenterCameraIfIdle(float delta) {
			if(TargetNode == null) return;
			var currentTime = OS.GetTicksMsec() / 1000f;
			if (currentTime - lastMovementTimeSeconds > RecenterTimeoutSeconds) {
				// We will compare the camera's right vector with the target object's forward vector
				var cameraRight = GlobalTransform.basis.x;
				// If the camera is configured to sit behind the node, then we have to negate node's forward vector to compare them
				var targetForward = PositionBehindTarget ? -TargetNode.GlobalTransform.basis.z : TargetNode.GlobalTransform.basis.z;
				// If the camera is directly behind the target node, the dot product of the two vectors will be zero
				var difference = cameraRight.Dot(targetForward);
				// If the dot product of the camera's right with the target's forward vector is less than the deadzone, move it back into alignment with the target
				if (Mathf.Abs(difference) > RecenterDeadzone) {
					var recenterAmount = RecenterSpeed * delta;
					if (difference < 0) {
						Azimuth += recenterAmount;
					}
					else {
						Azimuth -= recenterAmount;
					}
				}
			}
		}

		/// <summary>
		/// Check if the camera is close to any obstacles, and avoid them by zooming in
		/// </summary>
		protected void CheckObstaclesAndAvoid(float delta) {
			if(obstacleTestArea.GetOverlappingBodies().Count > 0) {
				Distance -= ObstacleAvoidanceSpeed * delta;
			}
		}
	}
}
