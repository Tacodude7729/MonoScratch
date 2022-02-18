using System.Globalization;
using Microsoft.Xna.Framework;
using System;
using MonoScratch.Share;

namespace MonoScratch.Runtime {
    public static class Utils {

        public static Random Rand = new Random();

        public static double StringToNumber(string str) {
            if (MonoScratchShare.TryParseNum(str, out double numberValue))
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

        public static Color ToColor(MonoScratchValue value) {
            if (value._stringValue?.StartsWith("#") ?? false) {
                if (value._stringValue.Length == 7) {
                    if (int.TryParse(value._stringValue.Substring(1), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out int rgb)) {
                        return ToColor(rgb);
                    }
                }
                return Color.Black;
            } else return ToColor(value.AsNumber());
        }

        public static Color ToColor(double value) {
            uint iValue = (uint)value;
            byte a = (byte)((iValue >> 24) & 0xFF);
            byte r = (byte)((iValue >> 16) & 0xFF);
            byte g = (byte)((iValue >> 8) & 0xFF);
            byte b = (byte)(iValue & 0xFF);
            return new Color(r, g, b, a > 0 ? a : (byte)255);
        }

        public static double Random(double a, double b) { // TODO this don't work for rand(0.0, 1.0)
            // if ((int)a == a && (int)b == b) {
            //     if (b > a)
            //         return Rand.Next((int)a, (int)b);
            //     else
            //         return Rand.Next((int)b, (int)a);
            // }
            if (b > a)
                return a + Rand.NextDouble() * (b - a);
            else
                return b + Rand.NextDouble() * (a - b);
        }

        public static double Tan(double angle) {
            angle = angle % 360;
            switch (angle) {
                case -270:
                case 90:
                    return double.PositiveInfinity;
                case -90:
                case 270:
                    return double.NegativeInfinity;
                default:
                    return Math.Tan((Math.PI * angle) / 180);
            }
        }
    }
}