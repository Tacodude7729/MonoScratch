using System.Threading;
using MonoScratch.Project;

namespace MonoScratch.Runtime {

    public static class Program {

        public static MonoScratchRuntime Runtime;

        static Program() {
            Runtime = new MonoScratchRuntime();
        }

        public static void Main() {
            Thread.CurrentThread.Name = "Main";
            Runtime.Run();
        }
    }
}