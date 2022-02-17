using Microsoft.Xna.Framework;

namespace MonoScratch.Runtime {

    public class PenLineBuffer {

        public const int DefaultCapacity = 32;
        public const int SizeMultiplier = 2;

        public PenLineVertex[] Vertices { get; private set; }
        public int Capacity => Vertices.Length;

        public int VertexCount { get; private set; }
        public int TriangleCount => VertexCount / 3;
        public int LineCount => VertexCount / 6;

        public bool IsEmpty => VertexCount == 0;

        public PenLineBuffer() {
            Vertices = new PenLineVertex[DefaultCapacity * 6];
            VertexCount = 0;
        }

        public void AddLine(Vector2 linePoint, Vector2 linePointDiff, Vector4 lineColor, Vector2 lineLengthThickness) {
            if (VertexCount == Capacity) {
                PenLineVertex[] newVertices = new PenLineVertex[Capacity * SizeMultiplier];
                Vertices.CopyTo(newVertices, 0);
                Vertices = newVertices;
            }

            Vertices[VertexCount++] = new PenLineVertex(new Vector2(1f, 0f), linePoint, linePointDiff, lineColor, lineLengthThickness);
            Vertices[VertexCount++] = new PenLineVertex(new Vector2(0f, 0f), linePoint, linePointDiff, lineColor, lineLengthThickness);
            Vertices[VertexCount++] = new PenLineVertex(new Vector2(1f, 1f), linePoint, linePointDiff, lineColor, lineLengthThickness);

            Vertices[VertexCount++] = new PenLineVertex(new Vector2(1f, 1f), linePoint, linePointDiff, lineColor, lineLengthThickness);
            Vertices[VertexCount++] = new PenLineVertex(new Vector2(0f, 0f), linePoint, linePointDiff, lineColor, lineLengthThickness);
            Vertices[VertexCount++] = new PenLineVertex(new Vector2(0f, 1f), linePoint, linePointDiff, lineColor, lineLengthThickness);
        }

        public void Clear() {
            VertexCount = 0;
        }
    }
}