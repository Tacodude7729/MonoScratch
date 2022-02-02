
using MonoScratch.Project;
using System.Collections.Generic;
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoScratch.Runtime {

    public class MonoScratchRuntime : Game {

        public List<MonoScratchThread> Threads;

        public List<IMonoScratchSprite> DefaultSprites;
        public IMonoScratchStage Stage;

        public TargetLinkedList Targets;

        public readonly GraphicsDeviceManager Graphics;
        public readonly MonoScratchRenderer Renderer;

        public MonoScratchRuntime() {
            Threads = new List<MonoScratchThread>();
            DefaultSprites = Interface.GetSprites();
            Stage = Interface.GetStage();
            Targets = new TargetLinkedList();

            foreach (IMonoScratchSprite sprite in DefaultSprites)
                Targets.InsertLast(sprite);

            Graphics = new GraphicsDeviceManager(this);

            Window.AllowUserResizing = false;
            Graphics.PreferredBackBufferWidth = 960;
            Graphics.PreferredBackBufferHeight = 720;
            Graphics.ApplyChanges();

            Renderer = new MonoScratchRenderer(this);
        }

        protected override void Initialize() {
            Stage.Assets.Load();
            foreach (IMonoScratchSprite sprite in DefaultSprites)
                sprite.Assets.Load();

            base.Initialize();
        }

        public void Step() {
            foreach (IMonoScratchSprite sprite in DefaultSprites) {
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
            Renderer.Render();
            base.Draw(gameTime);
        }
    }

    // Over engineered? Maybe.
    // Fast? Probably not.
    // Stylish? Yes
    public class TargetLinkedList {

        public Node? First, Last;

        public TargetLinkedList() { }

        public IEnumerable<IMonoScratchTarget> Forward() {
            Node? node = First;
            while (node != null) {
                yield return node.Sprite;
                node = node.After;
            }
        }

        public IEnumerable<IMonoScratchTarget> Backward() {
            Node? node = Last;
            while (node != null) {
                yield return node.Sprite;
                node = node.Before;
            }
        }

        public Node InsertLast(IMonoScratchTarget other) {
            Node node = new Node(other);
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

        public Node InsertFirst(IMonoScratchTarget other) {
            Node node = new Node(other);
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

        public class Node {
            public Node? Before, After;
            public IMonoScratchTarget Sprite;

            public Node(IMonoScratchTarget sprite) {
                Sprite = sprite;
            }

            public Node InsertBefore(IMonoScratchTarget other) {
                Node node = new Node(other);
                if (Before != null) {
                    Before.After = node;
                    node.Before = Before;
                }
                Before = node;
                node.After = this;
                return node;
            }

            public Node InsertAfter(IMonoScratchTarget other) {
                Node node = new Node(other);
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