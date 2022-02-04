using System.Collections.Generic;

namespace MonoScratch.Runtime {

    public class MonoScratchThread {

        public delegate IEnumerable<YieldReason> ScratchFunction();

        public readonly ScratchFunction Function;
        public ThreadStatus Status;

        private IEnumerator<YieldReason> _enumerator;
        private bool _executing;
        private bool _restartQueued;

        public MonoScratchThread(ScratchFunction function) {
            Function = function;
            Status = ThreadStatus.YIELD;
            _enumerator = function.Invoke().GetEnumerator();
            _executing = false;
            _restartQueued = false;
        }

        public void Restart() {
            if (_executing) {
                _restartQueued = true;
                return;
            }
            Status = ThreadStatus.YIELD;
            _enumerator = Function.Invoke().GetEnumerator();
        }

        public void Step() {
            _executing = true;
            if (Status == ThreadStatus.YIELD) {
                if (!_enumerator.MoveNext()) {
                    Status = ThreadStatus.DONE;
                }
            }
            if (_restartQueued) {
                Status = ThreadStatus.YIELD;
                _enumerator = Function.Invoke().GetEnumerator();
                _restartQueued = false;
            }
            _executing = false;
        }

        public YieldReason YieldReason => _enumerator.Current;
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