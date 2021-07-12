using Godot;
using System.Collections.Generic;


namespace MagicSmoke {
	public class DebugRenderer : ImmediateGeometry {

		public static string ComponentPath {  get; } = "/root/DebugRenderer";

		protected Queue<IDebugPrimitive> primitives = new Queue<IDebugPrimitive>();
		protected List<IDebugPrimitive> timedPrimitives = new List<IDebugPrimitive>();

		[Export]
		public SpatialMaterial DebugMaterial { get; set; }

		protected void RenderPrimitives() {
			if(primitives.Count == 0 && timedPrimitives.Count == 0) return;
			MaterialOverride = DebugMaterial;
			foreach(var timed in timedPrimitives) timed.Render(this);
			while(primitives.Count > 0) {
				var primitive = primitives.Dequeue();
				primitive.Render(this);
				if(primitive.Duration > 0) {
					timedPrimitives.Add(primitive);
				}
			}
		}
		public override void _PhysicsProcess(float delta)
		{
			Clear();
			var now = OS.GetTicksMsec();
			timedPrimitives.RemoveAll(p => now - p.StartTime > p.Duration); // Remove any timed primitives that have expired
			RenderPrimitives();
		}
		public void QueueLineSegment(Vector3 start, Vector3 end, Color startColor, Color endColor, uint duration = 0) {
			primitives.Enqueue(new DebugLineSegment {
				StartTime = OS.GetTicksMsec(),
				Start = start,
				End = end,
				StartColor = startColor,
				EndColor = endColor,
				Duration = duration,
			});
		}

		public void QueuePoint(Vector3 position, Color color, uint duration = 0) {
			primitives.Enqueue(new DebugPoint {
				StartTime = OS.GetTicksMsec(),
				Position = position,
				Color = color,
				Duration = duration,
			});
		}

		public void QueueWireframeTriangle(Vector3 v0, Vector3 v1, Vector3 v2, Color color, uint duration = 0) {
			QueueLineSegment(v0, v1, color, duration: duration);
			QueueLineSegment(v1, v2, color, duration: duration);
			QueueLineSegment(v2, v0, color, duration: duration);
		}

		public void QueueWireframePyramid(Vector3 origin, Vector3 direction, float height, float baseSize, Color color, uint duration = 0) {
			baseSize /= 2f;
			var transform = new Transform(Quat.Identity, origin);
			transform.SetLookAt(origin, origin + direction, Vector3.Up);
			// We're going to build the pyramid in local coordinate space and then transform it with the given direction
			var vBot = new Vector3[] {
				new Vector3(-baseSize, baseSize, 0f),
				new Vector3(-baseSize, -baseSize, 0f),
				new Vector3(baseSize, -baseSize, 0f),
				new Vector3(baseSize, baseSize, 0f),
			};
			var peak = transform.Xform(new Vector3(0f, 0f, -height)); // The peak should face -Z to work correctly with the transform

			for(int i = 0; i < 4; i++) vBot[i] = transform.Xform(vBot[i]);

			// Bottom face
			QueueLineSegment(vBot[0], vBot[1], color, duration: duration);
			QueueLineSegment(vBot[1], vBot[2], color, duration: duration);
			QueueLineSegment(vBot[2], vBot[3], color, duration: duration);
			QueueLineSegment(vBot[3], vBot[0], color, duration: duration);
			// Lines from bottom corners to peak
			QueueLineSegment(vBot[0], peak, color, duration: duration);
			QueueLineSegment(vBot[1], peak, color, duration: duration);
			QueueLineSegment(vBot[2], peak, color, duration: duration);
			QueueLineSegment(vBot[3], peak, color, duration: duration);
		}

		public void QueueWireframeCube(Vector3 center, float size, Color color, uint duration = 0) {
			size /= 2f;
			var vTop = new Vector3[] {
				center + new Vector3(size, size, size),
				center + new Vector3(-size, size, size),
				center + new Vector3(-size, size, -size),
				center + new Vector3(size, size, -size)
			};
			var vBot = new Vector3[] {
				center + new Vector3(size, -size, size),
				center + new Vector3(-size, -size, size),
				center + new Vector3(-size, -size, -size),
				center + new Vector3(size, -size, -size)
			};
			// Top face
			QueueLineSegment(vTop[0], vTop[1], color, duration: duration);
			QueueLineSegment(vTop[1], vTop[2], color, duration: duration);
			QueueLineSegment(vTop[2], vTop[3], color, duration: duration);
			QueueLineSegment(vTop[3], vTop[0], color, duration: duration);
			// Bottom face
			QueueLineSegment(vBot[0], vBot[1], color, duration: duration);
			QueueLineSegment(vBot[1], vBot[2], color, duration: duration);
			QueueLineSegment(vBot[2], vBot[3], color, duration: duration);
			QueueLineSegment(vBot[3], vBot[0], color, duration: duration);
			// Top to bottom connections
			for(var i = 0; i < 4; i++) {
				QueueLineSegment(vBot[i], vTop[i], color, duration: duration);
			}

		}
		public void QueueLineSegment(Vector3 start, Vector3 end, Color color, uint duration = 0) => QueueLineSegment(start, end, color, color, duration);
		
	}
}
