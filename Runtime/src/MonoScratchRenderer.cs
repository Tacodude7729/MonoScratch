using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;

namespace MonoScratch.Runtime {

    public class MonoScratchRenderer {

        public readonly MonoScratchRuntime Runtime;
        public readonly SpriteBatch SpriteBatch;

        public readonly int PixelScale;
        public int Width, Height;

        public GraphicsDevice GraphicsDevice => Runtime.GraphicsDevice;

        public Effect? SpriteShader;

        public MonoScratchRenderer(MonoScratchRuntime runtime) {
            Runtime = runtime;

            SpriteBatch = new SpriteBatch(Runtime.GraphicsDevice);

            Width = 480;
            Height = 360;
            PixelScale = 2;

            SpriteShader = null!;
        }

        public void LoadShaders() {
            SpriteShader = new Effect(GraphicsDevice, File.ReadAllBytes("Shaders/Test.mgfx"));
            Log.Info("Loaded Test.mgfx!");
        }

        public void Render() {
            GraphicsDevice.Clear(Color.White);

            SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            SpriteShader!.CurrentTechnique.Passes[0].Apply();

            Runtime.Stage.DrawStage(this);

            foreach (IMonoScratchSprite sprite in Runtime.Sprites)
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

    }
}