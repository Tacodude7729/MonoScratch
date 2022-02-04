namespace MonoScratch.Runtime {
    public static class Utils {
        public static double StringToNumber(string str) {
            if (double.TryParse(str, MonoScratchValue.Styles, null, out double numberValue))
                return double.IsNaN(numberValue) ? 0 : numberValue;
            return 0;
        }

        public static double Devide(double a, double b) {
            if (b == 0) {
                if (a == 0) return double.NaN;
                if (a < 0) return double.NegativeInfinity;
                if (a > 0) return double.PositiveInfinity; 
            }
            return a / b;
        }

        public static double Mod(double n, double modulus) {
            double result = n % modulus;
            if (result / modulus < 0) result += modulus;
            return result;
        }

    }
}