
using MonoScratch.Project;
using System.Collections.Generic;
using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace MonoScratch.Runtime {

    public class MonoScratchRuntime : Game {

        public readonly ProjectSettings Settings;

        public readonly List<IMonoScratchSprite> DefaultSprites;
        public readonly IMonoScratchStage Stage;
        public readonly TargetLinkedList Targets;

        public readonly GraphicsDeviceManager Graphics;
        public readonly MonoScratchRenderer Renderer;

        public KeyboardState KeyboardState { get; private set; }
        public MouseState MouseState { get; private set; }

        private readonly List<MonoScratchThread> _threads;
        private readonly Dictionary<Delegate, MonoScratchThread> _threadFuncMap;

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
            _threadFuncMap = new Dictionary<Delegate, MonoScratchThread>();

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

        private MonoScratchThread StartThread(MonoScratchThread newThread, bool restartExisting) {
            if (_threadFuncMap.TryGetValue(newThread.FunctionDelegate, out MonoScratchThread? thread)) {
                if (restartExisting)
                    thread.Restart();
                return thread;
            }
            _threads.Add(newThread);
            _threadFuncMap.Add(newThread.FunctionDelegate, newThread);
            return newThread;
        }

        public MonoScratchThread StartThread(MonoScratchYieldingThread.YieldingFunction function, bool restartExisting)
            => StartThread(new MonoScratchYieldingThread(function), restartExisting);

        public MonoScratchThread StartThread(MonoScratchInstantThread.InstantFunction function, bool restartExisting)
            => StartThread(new MonoScratchInstantThread(function), restartExisting);

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
                            _threadFuncMap.Remove(thread.FunctionDelegate);
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
            KeyboardState = Keyboard.GetState();
            MouseState = Mouse.GetState();
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