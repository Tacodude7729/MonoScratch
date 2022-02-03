
using MonoScratch.Project;
using System.Collections.Generic;
using System;
using Microsoft.Xna.Framework;

namespace MonoScratch.Runtime {

    public class MonoScratchRuntime : Game {
        public readonly List<MonoScratchThread> Threads;

        public readonly List<IMonoScratchSprite> DefaultSprites;
        public readonly IMonoScratchStage Stage;
        public readonly ProjectSettings Settings;

        public readonly TargetLinkedList Targets;

        public readonly GraphicsDeviceManager Graphics;
        public readonly MonoScratchRenderer Renderer;

        public bool RedrawRequested;

        public MonoScratchRuntime() {
            Threads = new List<MonoScratchThread>();
            DefaultSprites = Interface.GetSprites();
            Stage = Interface.GetStage();
            Targets = new TargetLinkedList();
            Settings = Interface.GetSettings();

            foreach (IMonoScratchSprite sprite in DefaultSprites)
                sprite.LayerNode = Targets.InsertLast(sprite);
            Targets.InsertLast(Stage);

            Graphics = new GraphicsDeviceManager(this);

            Window.AllowUserResizing = false;
            Graphics.PreferredBackBufferWidth = 960;
            Graphics.PreferredBackBufferHeight = 720;
            Graphics.ApplyChanges();

            Renderer = new MonoScratchRenderer(this);

            IsFixedTimeStep = true;
            TargetElapsedTime = TimeSpan.FromSeconds(1d / Settings.FPS);
        }

        protected override void Initialize() {
            Stage.Assets.Load();
            foreach (IMonoScratchSprite sprite in DefaultSprites)
                sprite.Assets.Load();

            base.Initialize();

            OnGreenFlag();
        }

        public void OnGreenFlag() {
            foreach (IMonoScratchTarget target in Targets.Forward())
                target.OnGreenFlag();
        }

        public void Step() {
            int numActiveThreads = 1;
            bool ranFirstTick = false;

            while (Threads.Count != 0
                && numActiveThreads != 0
                && (!RedrawRequested || Settings.TurboMode)
            ) {
                numActiveThreads = 0;
                bool removedThread = false;

                for (int i = 0; i < Threads.Count; i++) {
                    MonoScratchThread thread = Threads[i];

                    if (!ranFirstTick && thread.Status == ThreadStatus.YIELD_TICK) {
                        thread.Status = ThreadStatus.YIELD;
                    }

                    if (thread.Status == ThreadStatus.YIELD) {
                        thread.Step();
                    }

                    if (thread.Status == ThreadStatus.DONE) {
                        removedThread = true;
                    }

                    if (thread.Status == ThreadStatus.YIELD) {
                        ++numActiveThreads;
                    }
                }

                ranFirstTick = true;

                if (removedThread)
                    Threads.RemoveAll(thread => thread.Status == ThreadStatus.DONE);
            }
        }

        protected override void Update(GameTime gameTime) {
            Step();
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
        public int Count { get; private set; }

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
            Node node = new Node(this, other);
            if (Last == null) {
                Last = node;
                First = node;
            } else {
                Last.After = node;
                node.Before = Last;
                Last = node;
            }
            ++Count;
            return node;
        }

        public Node InsertFirst(IMonoScratchTarget other) {
            Node node = new Node(this, other);
            if (First == null) {
                Last = node;
                First = node;
            } else {
                First.Before = node;
                node.After = First;
                First = node;
            }
            ++Count;
            return node;
        }

        public class Node {
            public Node? Before, After;
            public IMonoScratchTarget Sprite;
            public readonly TargetLinkedList List;

            public Node(TargetLinkedList list, IMonoScratchTarget sprite) {
                Sprite = sprite;
                List = list;
            }

            public void Remove() {
                if (Before != null)
                    Before.After = After;
                if (After != null)
                    After.Before = Before;
                Before = null;
                After = null;
                --List.Count;
            }

            public Node InsertBefore(IMonoScratchTarget other) {
                Node node = new Node(List, other);
                if (Before != null) {
                    Before.After = node;
                    node.Before = Before;
                }
                Before = node;
                node.After = this;
                ++List.Count;
                return node;
            }

            public Node InsertAfter(IMonoScratchTarget other) {
                Node node = new Node(List, other);
                if (After != null) {
                    After.Before = node;
                    node.After = After;
                }
                After = node;
                node.Before = this;
                ++List.Count;
                return node;
            }
        }
    }
}