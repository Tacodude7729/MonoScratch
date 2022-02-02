namespace MonoScratch.Runtime {
    public static class Utils {

        public static MonoScratchThread StartThread(MonoScratchThread.ScratchFunction function) {
            MonoScratchThread thread = new MonoScratchThread(function);
            Program.Runtime.Threads.Add(thread);
            return thread;
        }

        public static double StringToNumber(string str) {
            if (double.TryParse(str, MonoScratchValue.Styles, null, out double numberValue))
                return double.IsNaN(numberValue) ? 0 : numberValue;
            return 0;
        }

    }
}