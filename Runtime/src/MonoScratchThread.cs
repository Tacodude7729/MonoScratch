using System.Collections.Generic;

namespace MonoScratch.Runtime {

    public class MonoScratchThread {

        public delegate IEnumerable<YieldReason> ScratchFunction();

        public ScratchFunction Function;
        public IEnumerator<YieldReason> Enumerator;

        public ThreadStatus Status;

        public MonoScratchThread(ScratchFunction function) {
            Function = function;
            Enumerator = function.Invoke().GetEnumerator();
            Status = ThreadStatus.YIELD;
        }

        public void Step() {
            if (Status == ThreadStatus.YIELD) {
                if (!Enumerator.MoveNext()) {
                    Status = ThreadStatus.DONE;
                }
            }
        }

        public YieldReason YieldReason => Enumerator.Current;
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