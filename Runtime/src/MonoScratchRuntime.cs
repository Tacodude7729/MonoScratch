
using MonoScratch.Project;
using System.Collections.Generic;
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoScratch.Runtime {

    public class MonoScratchRuntime : Game {

        public List<MonoScratchThread> Threads;

        public List<IMonoScratchSprite> Sprites;
        public IMonoScratchStage Stage;

        public readonly GraphicsDeviceManager Graphics;
        public readonly SpriteBatch SpriteBatch;
        public RenderInfo RenderInfo;

        public MonoScratchRuntime() {
            Threads = new List<MonoScratchThread>();
            Sprites = Interface.GetSprites();
            Stage = Interface.GetStage();

            Graphics = new GraphicsDeviceManager(this);

            Window.AllowUserResizing = false;
            Graphics.PreferredBackBufferWidth = 960;
            Graphics.PreferredBackBufferHeight = 720;
            Graphics.ApplyChanges();

            SpriteBatch = new SpriteBatch(GraphicsDevice);
            RenderInfo = new RenderInfo(this, SpriteBatch, 480, 360, 3);
        }

        protected override void Initialize() {
            Stage.Assets.Load();
            foreach (IMonoScratchSprite sprite in Sprites)
                sprite.Assets.Load();

            base.Initialize();
        }

        private void Step() {
            foreach (IMonoScratchSprite sprite in Sprites) {
                sprite.OnGreenFlag();
            }
            Stage.OnGreenFlag();

            while (Threads.Count != 0) {
                foreach (MonoScratchThread thread in Threads) {
                    thread.Step();
                }
                Threads.RemoveAll(thread => thread.Status == ThreadStatus.DONE);
            }
        }

        protected override void Update(GameTime gameTime) {
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime) {
            GraphicsDevice.Clear(Color.White);
            SpriteBatch.Begin();

            Stage.DrawStage(RenderInfo);

            foreach (IMonoScratchSprite sprite in Sprites)
                sprite.DrawSprite(RenderInfo);

            SpriteBatch.End();
            base.Draw(gameTime);
        }
    }

    public record RenderInfo(MonoScratchRuntime RT, SpriteBatch SB, int Width, int Height, float PixelScale);

    // Over engineered? Maybe.
    // Fast? Probably not.
    // Stylish? Yes
    public class SpriteLinkedList {

        public SpriteLinkedListNode? First, Last;

        public SpriteLinkedList() { }

        public SpriteLinkedListNode InsertLast(IMonoScratchSprite other) {
            SpriteLinkedListNode node = new SpriteLinkedListNode(other);
            if (Last == null) {
                Last = node;
                First = node;
            } else {
                Last.After = node;
                node.Before = Last;
                Last = node;
            }
            return node;
        }

        public SpriteLinkedListNode InsertFirst(IMonoScratchSprite other) {
            SpriteLinkedListNode node = new SpriteLinkedListNode(other);
            if (First == null) {
                Last = node;
                First = node;
            } else {
                First.Before = node;
                node.After = First;
                First = node;
            }
            return node;
        }

        public class SpriteLinkedListNode {
            public SpriteLinkedListNode? Before, After;
            public IMonoScratchSprite Sprite;

            public SpriteLinkedListNode(IMonoScratchSprite sprite) {
                Sprite = sprite;
            }

            public SpriteLinkedListNode InsertBefore(IMonoScratchSprite other) {
                SpriteLinkedListNode node = new SpriteLinkedListNode(other);
                if (Before != null) {
                    Before.After = node;
                    node.Before = Before;
                }
                Before = node;
                node.After = this;
                return node;
            }

            public SpriteLinkedListNode InsertAfter(IMonoScratchSprite other) {
                SpriteLinkedListNode node = new SpriteLinkedListNode(other);
                if (After != null) {
                    After.Before = node;
                    node.After = After;
                }
                After = node;
                node.Before = this;
                return node;
            }
        }
    }
}