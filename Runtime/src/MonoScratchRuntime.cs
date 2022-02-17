
using MonoScratch.Project;
using System.Collections.Generic;
using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;

namespace MonoScratch.Runtime {

    public class MonoScratchRuntime : Game {

        public readonly List<IMonoScratchSprite> DefaultSprites;
        public readonly IMonoScratchStage Stage;
        public readonly ProjectSettings Settings;

        public readonly TargetLinkedList Targets;

        public readonly GraphicsDeviceManager Graphics;
        public readonly MonoScratchRenderer Renderer;

        private readonly List<MonoScratchThread> _threads;
        private readonly Dictionary<MonoScratchThread.ScratchFunction, MonoScratchThread> _threadFuncMap;

        public Stopwatch TimerStopwatch;
        public double Timer => TimerStopwatch.ElapsedTicks / (double)Stopwatch.Frequency;
        private Stopwatch _projectStopwatch;

        public bool RedrawRequested;

        public MonoScratchRuntime() {
            DefaultSprites = Interface.GetSprites();
            Stage = Interface.GetStage();
            Targets = new TargetLinkedList();
            Settings = Interface.GetSettings();

            _threads = new List<MonoScratchThread>();
            _threadFuncMap = new Dictionary<MonoScratchThread.ScratchFunction, MonoScratchThread>();

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

            TimerStopwatch = new Stopwatch();
            _projectStopwatch = new Stopwatch();
        }

        protected override void Initialize() {
            Stage.Assets.Load();
            foreach (IMonoScratchSprite sprite in DefaultSprites)
                sprite.Assets.Load();

            OnGreenFlag();
            TimerStopwatch.Start();

            base.Initialize();
        }

        public MonoScratchThread StartThread(MonoScratchThread.ScratchFunction function, bool restartExisting) {
            if (_threadFuncMap.TryGetValue(function, out MonoScratchThread? thread)) {
                if (restartExisting) {
                    thread.Restart();
                }
                return thread;
            }
            thread = new MonoScratchThread(function);
            _threads.Add(thread);
            _threadFuncMap.Add(function, thread);
            return thread;
        }

        public void OnGreenFlag() {
            foreach (IMonoScratchTarget target in Targets.Forward())
                target.OnGreenFlag();
        }

        public void Step() {
            if (!_projectStopwatch.IsRunning) _projectStopwatch.Start();
            int numActiveThreads = 1;
            bool ranFirstTick = false;

            while (_threads.Count != 0
                && numActiveThreads != 0
                && (!RedrawRequested || Settings.TurboMode)
            ) {
                numActiveThreads = 0;
                bool removedThread = false;

                for (int i = 0; i < _threads.Count; i++) {
                    MonoScratchThread thread = _threads[i];

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
                    _threads.RemoveAll(thread => {
                        if (thread.Status == ThreadStatus.DONE) {
                            _threadFuncMap.Remove(thread.Function);
                            return true;
                        }
                        return false;
                    });
            }

            if (_threads.Count == 0 && Settings.CloseWhenDone) {
                Log.Info($"Finished in {_projectStopwatch.ElapsedMilliseconds / 1000d}s");
                Exit();
            }
        }

        protected override void Update(GameTime gameTime) {
            Step();
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime) {
            Graphics.PreferredBackBufferWidth = Renderer.Width * Renderer.PixelScale;
            Graphics.PreferredBackBufferHeight = Renderer.Height * Renderer.PixelScale;
            Graphics.ApplyChanges();

            Renderer.Render();
            RedrawRequested = false;
            base.Draw(gameTime);
        }
    }
}