namespace MonoScratch.Runtime {
    public static class Utils {

        public static void StartThread(MonoScratchThread.ScratchFunction function) {
            Program.Runtime.Threads.Add(new MonoScratchThread(function));
        }
    }
}