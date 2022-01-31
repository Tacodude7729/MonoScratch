namespace MonoScratch.Runtime {
    public static class Utils {

        public static void StartThread(MonoScratchThread.ScratchFunction function) {
            Program.Runtime.Threads.Add(new MonoScratchThread(function));
        }

        public static double StringToNumber(string str) {
            if (double.TryParse(str, MonoScratchValue.Styles, null, out double numberValue))
                return double.IsNaN(numberValue) ? 0 : numberValue;
            return 0;
        }

    }
}