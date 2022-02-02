using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
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

        public RenderTarget2D PenCanvas;
        public Texture2D PenDummyTexture;
        public readonly SpriteBatch PenSpriteBatch;
        public readonly Effect PenLineShader;
        public readonly EffectParameter PenLineShaderAspectRatio;
        public readonly EffectParameter PenLineShaderLinePoint1;
        public readonly EffectParameter PenLineShaderLinePoint2;
        public readonly EffectParameter PenLineShaderLineThickness;
        private bool _penRendering;

        private float _aspectRatio;

        public MonoScratchRenderer(MonoScratchRuntime runtime) {
            Runtime = runtime;

            SpriteBatch = new SpriteBatch(Runtime.GraphicsDevice);

            Width = 480;
            Height = 360;
            PixelScale = 2;

            SpriteShader = new Effect(GraphicsDevice, File.ReadAllBytes("Shaders/Sprite.mgfx"));
            SpriteShaderBrightnessEffect = SpriteShader.Parameters["BrightnessEffect"] ?? throw new SystemException("Missing parameter on shader!");

            PenCanvas = new RenderTarget2D(GraphicsDevice, Width, Height, false, SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
            PenDummyTexture = new Texture2D(GraphicsDevice, 1, 1);
            PenSpriteBatch = new SpriteBatch(Runtime.GraphicsDevice);
            PenLineShader = new Effect(GraphicsDevice, File.ReadAllBytes("Shaders/PenLine.mgfx"));
            PenLineShaderLinePoint1 = PenLineShader.Parameters["LinePoint1"] ?? throw new SystemException("Missing parameter on shader!");
            PenLineShaderLinePoint2 = PenLineShader.Parameters["LinePoint2"] ?? throw new SystemException("Missing parameter on shader!");
            PenLineShaderLineThickness = PenLineShader.Parameters["LineThickness"] ?? throw new SystemException("Missing parameter on shader!");
            PenLineShaderAspectRatio = PenLineShader.Parameters["AspectRatio"] ?? throw new SystemException("Missing parameter on shader!");
            _penRendering = false;

            _aspectRatio = ((float)Height) / Width;
            PenLineShaderAspectRatio.SetValue(_aspectRatio);
            PenClear();

            PenDrawLine(150, -80, -150, -80, Color.Red, 10);
            PenDrawLine(-150, 80, 150, -80, Color.Black, 10);
            PenDrawLine(0, 0, 0, 0, Color.Blue, 100);
        }

        public void Render() {
            TryStopPenRendering(null);

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

        private void TryStartPenRendering() {
            if (!_penRendering) {
                GraphicsDevice.SetRenderTarget(PenCanvas);
                PenSpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp);
                _penRendering = true;
            }
        }

        private void TryStopPenRendering(RenderTarget2D? renderTarget = null) {
            if (_penRendering) {
                PenSpriteBatch.End();
                GraphicsDevice.SetRenderTarget(renderTarget);
                _penRendering = false;
            }
        }

        public void PenClear() {
            if (!_penRendering) {
                GraphicsDevice.SetRenderTarget(PenCanvas);
            }
            GraphicsDevice.Clear(Color.Transparent);
            if (!_penRendering) {
                GraphicsDevice.SetRenderTarget(null);
            }
        }

        public void PenDrawLine(int x1, int y1, int x2, int y2, Color color, int thickness) {
            TryStartPenRendering();

            PenLineShaderLinePoint1.SetValue(new Vector2(((float)x1) / Width + 0.5f, (0.5f * _aspectRatio) - ((float)y1) / Height));
            PenLineShaderLinePoint2.SetValue(new Vector2(((float)x2) / Width + 0.5f, (0.5f * _aspectRatio) - ((float)y2) / Height));
            PenLineShaderLineThickness.SetValue(((float)thickness) / Width / 2);
            PenLineShader.CurrentTechnique.Passes[0].Apply();
            PenSpriteBatch.Draw(PenDummyTexture, new Rectangle(0, 0, Width, Height), null, color);
        }

    }
}