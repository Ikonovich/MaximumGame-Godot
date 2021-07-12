using Godot;

namespace MagicSmoke {

    public interface IDebugPrimitive {
        uint StartTime { get; }
        uint Duration { get; set; }
        void Render(DebugRenderer renderer);
    }

    public class DebugPoint : IDebugPrimitive {
        public uint StartTime { get; set; }
        public uint Duration { get; set; }
        public Vector3 Position { get; set; }
        public Color Color { get; set; }
        public void Render(DebugRenderer renderer) {
            renderer.Begin(Mesh.PrimitiveType.Points);
            renderer.SetColor(Color);
            renderer.AddVertex(Position);
            renderer.End();
        }
    }
    public class DebugLineSegment : IDebugPrimitive {

        public uint StartTime { get; set; }
        public uint Duration { get; set; }
        public Vector3 Start { get; set; }
        public Vector3 End { get; set; }

        public Color StartColor { get; set; } 
        public Color EndColor { get; set; }

        public void Render(DebugRenderer renderer) {
            renderer.Begin(Mesh.PrimitiveType.Lines);
            renderer.SetColor(StartColor);
            renderer.AddVertex(Start);
            renderer.SetColor(EndColor);
            renderer.AddVertex(End);
            renderer.End();
        }
    }
}