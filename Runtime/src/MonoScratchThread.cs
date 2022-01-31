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
            Status = ThreadStatus.RUNNING;
        }

        public void Step() {
            if (Status == ThreadStatus.RUNNING) {
                if (!Enumerator.MoveNext()) {
                    Status = ThreadStatus.DONE;
                }
            }
        }
    }

    public enum ThreadStatus : int {
        RUNNING,
        DONE
    }

    public enum YieldReason : int {
        YIELD
    }

}