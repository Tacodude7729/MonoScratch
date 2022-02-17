using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoScratch.Runtime {

    public struct PenLineVertex : IVertexType {
        public readonly Vector2 Position;
        public readonly Vector2 LinePoint;
        public readonly Vector2 LinePointDiff;
        public readonly Vector4 LineColor;
        public readonly Vector2 LineLengthThickness;

        public PenLineVertex(Vector2 position, Vector2 linePoint, Vector2 linePointDiff, Vector4 lineColor, Vector2 lineLengthThickness) {
            Position = position;
            LinePoint = linePoint;
            LinePointDiff = linePointDiff;
            LineColor = lineColor;
            LineLengthThickness = lineLengthThickness;
        }

        private static readonly VertexDeclaration _vertexDeclaration = new VertexDeclaration(
            new VertexElement(sizeof(float) * 0, VertexElementFormat.Vector2, VertexElementUsage.Position, 0), // Position
            new VertexElement(sizeof(float) * 2, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0), // LinePoint
            new VertexElement(sizeof(float) * 4, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 1), // LinePointDiff
            new VertexElement(sizeof(float) * 6, VertexElementFormat.Vector4, VertexElementUsage.Color, 0), // LineColor
            new VertexElement(sizeof(float) * 10, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 2) // LineLengthThickness
        );
        public VertexDeclaration VertexDeclaration => _vertexDeclaration;
    }

}