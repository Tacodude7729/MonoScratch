using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;

//
// This file contains the code to render everything.
// It deals with shaders, sprite effects, the pen, etc.
//

namespace MonoScratch.Runtime {

    public class MonoScratchRenderer {

        public readonly MonoScratchRuntime Runtime;
        public readonly SpriteBatch SpriteBatch;

        public readonly int PixelScale;
        public int Width, Height;

        public GraphicsDevice GraphicsDevice => Runtime.GraphicsDevice;

        public readonly Effect SpriteShader;
        public readonly EffectParameter SpriteShaderBrightnessEffect;


        private float _aspectRatio;

        public MonoScratchRenderer(MonoScratchRuntime runtime) {
            Runtime = runtime;

            SpriteBatch = new SpriteBatch(Runtime.GraphicsDevice);

            Width = 480;
            Height = 360;
            PixelScale = 3;

            SpriteShader = new Effect(GraphicsDevice, File.ReadAllBytes("Shaders/Sprite.mgfx"));
            SpriteShaderBrightnessEffect = SpriteShader.Parameters["BrightnessEffect"] ?? throw new SystemException("Missing parameter on shader!");

            PenCanvas = new RenderTarget2D(GraphicsDevice, Width, Height, false, SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
            _penLineEffect = new Effect(GraphicsDevice, File.ReadAllBytes("Shaders/PenLine.mgfx"));
            _penLineEffectCanvasSize = _penLineEffect.Parameters["CanvasSize"] ?? throw new SystemException("Missing parameter on shader!");
            _penLineBuffer = new List<PenStrokeVertex>();

            _aspectRatio = ((float)Height) / Width;

            _penLineEffectCanvasSize.SetValue(new Vector2(Width, Height));

            PenClear();
            PenDrawLine(150, -80, -150, -80, 2, 1, 0, 0);
            PenDrawLine(-150, 80, 150, -80, 20, 0, 0, 0);
            PenDrawLine(0, 0, -200, 0, 50, 0, 1, 0);
            PenFlush();
        }

        public void Render() {

            // VertexPositionColorTexture
            GraphicsDevice.SetRenderTarget(null);
            GraphicsDevice.Clear(Color.White);
            SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp);
            SpriteShader.CurrentTechnique.Passes[0].Apply();

            Runtime.Stage.DrawStage(this);
            SpriteBatch.Draw(PenCanvas, new Rectangle(0, 0, Width * PixelScale, Height * PixelScale), null, Color.White);
            foreach (IMonoScratchSprite sprite in Runtime.Targets.Backward())
                sprite.DrawSprite(this);

            SpriteBatch.End();
        }

        public void RenderCostume(MonoScratchCostume costume, int x, int y, int rotation, int scale) {
            SpriteBatch.Draw(costume.Texture,
                new Vector2(PixelScale * (x + Width / 2f), PixelScale * (Height / 2f - y)),
                null, Color.White,
                (float)(Math.PI * (rotation - 90) / 180), costume.RotationCenter,
                new Vector2(PixelScale * (scale / 100f) / costume.BitmapResolution, PixelScale * (scale / 100f) / costume.BitmapResolution),
                SpriteEffects.None, 0);
        }

        //
        // Pen Stuff
        //

        public RenderTarget2D PenCanvas;

        private Effect _penLineEffect;
        private EffectParameter _penLineEffectCanvasSize;

        private List<PenStrokeVertex> _penLineBuffer;

        private struct PenStrokeVertex : IVertexType {
            public readonly Vector2 Position;
            public readonly Vector2 LinePoint;
            public readonly Vector2 LinePointDiff;
            public readonly Vector4 LineColor;
            public readonly Vector2 LineLengthThickness;

            public PenStrokeVertex(Vector2 position, Vector2 linePoint, Vector2 linePointDiff, Vector4 lineColor, Vector2 lineLengthThickness) {
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

        public void PenDrawLine(float x0, float y0, float x1, float y1, float lineThickness, float r, float g, float b) {
            float lineDiffX = x1 - x0;
            float lineDiffY = y0 - y1;
            float lineLength = MathF.Sqrt((lineDiffX * lineDiffX) + (lineDiffY * lineDiffY));

            Vector2 linePoint = new Vector2(x0, y0);
            Vector2 linePointDiff = new Vector2(lineDiffX, lineDiffY);
            Vector2 lineLengthThickness = new Vector2(lineLength, lineThickness);
            Vector4 lineColor = new Vector4(r, g, b, 1f);

            _penLineBuffer.Add(new PenStrokeVertex(new Vector2(1f, 0f), linePoint, linePointDiff, lineColor, lineLengthThickness));
            _penLineBuffer.Add(new PenStrokeVertex(new Vector2(0f, 0f), linePoint, linePointDiff, lineColor, lineLengthThickness));
            _penLineBuffer.Add(new PenStrokeVertex(new Vector2(1f, 1f), linePoint, linePointDiff, lineColor, lineLengthThickness));

            _penLineBuffer.Add(new PenStrokeVertex(new Vector2(1f, 1f), linePoint, linePointDiff, lineColor, lineLengthThickness));
            _penLineBuffer.Add(new PenStrokeVertex(new Vector2(0f, 0f), linePoint, linePointDiff, lineColor, lineLengthThickness));
            _penLineBuffer.Add(new PenStrokeVertex(new Vector2(0f, 1f), linePoint, linePointDiff, lineColor, lineLengthThickness));
        }

        public void PenClear() {
            GraphicsDevice.SetRenderTarget(PenCanvas);
            GraphicsDevice.Clear(Color.Transparent);
        }

        private void PenFlush() {
            GraphicsDevice.SetRenderTarget(PenCanvas);

            GraphicsDevice.BlendState = BlendState.AlphaBlend;
            GraphicsDevice.DepthStencilState = DepthStencilState.None;

            _penLineEffect.CurrentTechnique.Passes[0].Apply();
            GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, _penLineBuffer.ToArray(), 0, _penLineBuffer.Count / 3);

            _penLineBuffer.Clear();
        }

    }
}