using System;
using System.Collections.Generic;

namespace MonoScratch.Runtime {

    public abstract class MonoScratchThread {

        public ThreadStatus Status;

        private bool _executing;
        private bool _restartQueued;

        public readonly Delegate FunctionDelegate;

        public MonoScratchThread(Delegate functionDelegate) {
            FunctionDelegate = functionDelegate;
            Status = ThreadStatus.YIELD;
            _executing = false;
            _restartQueued = false;
        }

        public void Restart() {
            if (_executing) _restartQueued = true;
            else RunRestart();
        }

        public void Step() {
            _executing = true;
            RunStep();
            _executing = false;
            if (_restartQueued) RunRestart();
        }

        protected virtual void RunRestart() {
            Status = ThreadStatus.YIELD;
            _restartQueued = false;
        }

        protected abstract void RunStep();
    }

    public class MonoScratchInstantThread : MonoScratchThread {

        public delegate void InstantFunction();
        public readonly InstantFunction Function;

        public MonoScratchInstantThread(InstantFunction function) : base(function) {
            Function = function;
        }

        protected override void RunStep() {
            Function();
            Status = ThreadStatus.DONE;
        }
    }

    public class MonoScratchYieldingThread : MonoScratchThread {
        public delegate IEnumerable<YieldReason> YieldingFunction();

        public readonly YieldingFunction Function;
        private IEnumerator<YieldReason> _enumerator;
        public YieldReason YieldReason => _enumerator.Current;

        public MonoScratchYieldingThread(YieldingFunction function) : base(function) {
            Function = function;
            _enumerator = function.Invoke().GetEnumerator();
        }

        protected override void RunRestart() {
            _enumerator = Function.Invoke().GetEnumerator();
            base.RunRestart();
        }

        protected override void RunStep() {
            if (!_enumerator.MoveNext()) {
                Status = ThreadStatus.DONE;
            }
        }
    }

    public enum ThreadStatus : int {
        YIELD,
        YIELD_TICK,
        DONE
    }

    public enum YieldReason : int {
        YIELD,
        HARD_YIELD
    }

}