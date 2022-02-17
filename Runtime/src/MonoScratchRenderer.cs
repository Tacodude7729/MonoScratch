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

        public readonly int PixelScale;
        public int Width, Height;

        public GraphicsDevice GraphicsDevice => Runtime.GraphicsDevice;

        public readonly RenderTarget2D Canvas;
        public readonly SpriteBatch CanvasSpriteBatch;

        public readonly SpriteBatch CostumeSpriteBatch;
        public readonly Effect CostumeShader;
        public readonly EffectParameter CostumeShaderBrightnessEffect;

        private float _aspectRatio;

        public MonoScratchRenderer(MonoScratchRuntime runtime) {
            Runtime = runtime;

            Width = 480;
            Height = 360;
            PixelScale = 3;

            Canvas = new RenderTarget2D(GraphicsDevice, Width * PixelScale, Height * PixelScale, false, GraphicsDevice.PresentationParameters.BackBufferFormat, DepthFormat.None);
            CanvasSpriteBatch = new SpriteBatch(GraphicsDevice);

            CostumeSpriteBatch = new SpriteBatch(Runtime.GraphicsDevice);
            CostumeShader = new Effect(GraphicsDevice, File.ReadAllBytes("Shaders/Sprite.mgfx"));
            CostumeShaderBrightnessEffect = CostumeShader.Parameters["BrightnessEffect"] ?? throw new SystemException("Missing parameter on shader!");

            PenCanvas = new RenderTarget2D(GraphicsDevice, Width, Height, false, SurfaceFormat.Color, DepthFormat.None, 8, RenderTargetUsage.PreserveContents);
            _penLineEffect = new Effect(GraphicsDevice, File.ReadAllBytes("Shaders/PenLine.mgfx"));
            _penLineEffectCanvasSize = _penLineEffect.Parameters["CanvasSize"] ?? throw new SystemException("Missing parameter on shader!");
            _penLineBuffer = new PenLineBuffer();
            _penSpriteBatch = new SpriteBatch(GraphicsDevice);

            _aspectRatio = ((float)Height) / Width;

            _penLineEffectCanvasSize.SetValue(new Vector2(Width, Height));
        }

        public void Render() {

            // Render Pen
            PenRender();

            // Render Canvas (Background, Pen and Sprites)
            GraphicsDevice.SetRenderTarget(Canvas);
            GraphicsDevice.Clear(Color.White);
            CostumeSpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp);
            CostumeShader.CurrentTechnique.Passes[0].Apply();
            foreach (IMonoScratchTarget target in Runtime.Targets.Backward()) {
                RenderTarget(target, CostumeSpriteBatch, PixelScale);

                if (target is IMonoScratchStage)
                    CostumeSpriteBatch.Draw(PenCanvas, new Rectangle(0, 0, Width * PixelScale, Height * PixelScale), null, Color.White);
            }
            CostumeSpriteBatch.End();

            // Put the Canvas onto the Backbuffer
            GraphicsDevice.SetRenderTarget(null);
            CanvasSpriteBatch.Begin();
            CanvasSpriteBatch.Draw(Canvas, Vector2.Zero, null, Color.White);
            CanvasSpriteBatch.End();
        }

        private void RenderTarget(IMonoScratchTarget target, SpriteBatch sb, int pixelScale) {
            if (target.RenderVisible) {
                MonoScratchCostume costume = target.CurrentCostume;
                int x = target.RenderX;
                int y = target.RenderY;
                int rotation = target.RenderRotation;

                float scale = pixelScale * target.RenderScale / costume.BitmapResolution;

                sb.Draw(costume.Texture,
                    new Vector2(pixelScale * (x + Width / 2f), pixelScale * (Height / 2f - y)),
                    null, Color.White,
                    (float)(Math.PI * (rotation - 90) / 180), costume.RotationCenter,
                    new Vector2(scale),
                    SpriteEffects.None, 0);
            }
        }

        //
        // Pen Stuff
        //

        public RenderTarget2D PenCanvas;

        private Effect _penLineEffect;
        private EffectParameter _penLineEffectCanvasSize;
        private SpriteBatch _penSpriteBatch;

        private PenLineBuffer _penLineBuffer;
        private bool _penTargetBound, _penSpriteBatchStarted;

        public void PenStamp(IMonoScratchSprite sprite) {
            PenRender();
            PenEnsureTarget();
            if (!_penSpriteBatchStarted) {
                _penSpriteBatch.Begin();
                _penSpriteBatchStarted = true;
            }
            RenderTarget(sprite, _penSpriteBatch, 1);
        }

        public void PenDrawLine(double x0, double y0, double x1, double y1, double lineThickness, Color color) {
            float lineDiffX = (float)(x1 - x0);
            float lineDiffY = (float)(y0 - y1);
            float lineLength = MathF.Sqrt((lineDiffX * lineDiffX) + (lineDiffY * lineDiffY));

            float offset = (lineThickness == 1 || lineThickness == 3) ? 0.5f : 0;
            Vector2 linePoint = new Vector2((float)x0 + offset, (float)y0 + offset);
            Vector2 linePointDiff = new Vector2(lineDiffX, lineDiffY);
            Vector2 lineLengthThickness = new Vector2(lineLength, (float)lineThickness);
            float lineColorAlpha = color.A / 255f;
            Vector4 lineColor = new Vector4(lineColorAlpha * color.R / 255f, lineColorAlpha * color.G / 255f, lineColorAlpha * color.B / 255f, lineColorAlpha);
            _penLineBuffer.AddLine(linePoint, linePointDiff, lineColor, lineLengthThickness);

            Runtime.RedrawRequested = true;
        }

        private void PenEnsureTarget() {
            if (!_penTargetBound)
                GraphicsDevice.SetRenderTarget(PenCanvas);
        }

        public void PenClear() {
            PenEnsureTarget();
            GraphicsDevice.Clear(Color.Transparent);
            _penLineBuffer.Clear();
        }

        private void PenRender() {
            if (_penSpriteBatchStarted) {
                _penSpriteBatch.End();
                _penSpriteBatchStarted = false;
            }
            if (!_penLineBuffer.IsEmpty) {
                PenEnsureTarget();

                GraphicsDevice.BlendState = BlendState.AlphaBlend;
                GraphicsDevice.DepthStencilState = DepthStencilState.None;

                _penLineEffect.CurrentTechnique.Passes[0].Apply();
                GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, _penLineBuffer.Vertices, 0, _penLineBuffer.TriangleCount);

                _penLineBuffer.Clear();
            }
        }

    }
}