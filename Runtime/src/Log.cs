using System;
using System.Threading;

namespace MonoScratch.Runtime {

    public static class Log {
        private static string Prefix() {
            return $"[{Thread.CurrentThread.Name ?? "?"}]";
        }

        public static void Info(string? s, string pre = "") {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"{Prefix()}{pre}[INFO] {s}");
        }

        public static void Warn(string? s, string pre = "") {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"{Prefix()}{pre}[WARN] {s}");
        }

        public static void Warn(string? s, Exception e, string pre = "") {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"{Prefix()}{pre}[WARN] {s}\n{e}");
        }

        public static void Error(string? s, string pre = "") {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"{Prefix()}{pre}[ERR] {s}");
        }

        public static void Error(string? s, Exception e, string pre = "") {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"{Prefix()}{pre}[ERR] {s}\n{e}");
        }
    }
}