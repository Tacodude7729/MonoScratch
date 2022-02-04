
using System.Globalization;

namespace MonoScratch.Compiler {

    public static class BlockUtils {
        public const NumberStyles Styles =
                NumberStyles.AllowDecimalPoint |
                NumberStyles.AllowExponent |
                NumberStyles.AllowLeadingSign |
                NumberStyles.AllowLeadingWhite |
                NumberStyles.AllowTrailingWhite |
                NumberStyles.AllowTrailingSign;

        public static string StringToNumString(string value) {
            if (double.TryParse(value, out double result)) {
                if (result.ToString() == value) return value + "d";
            }
            return SourceGenerator.StringValue(value);
        }

        public static string StringToBool(string value) {
            if (!(value == "" || value == "0" || value == "false"))
                return "true";
            return "false";
        }

        public static double StringToNum(string _stringValue) {
            if (double.TryParse(_stringValue, Styles, null, out double numberValue))
                return double.IsNaN(numberValue) ? 0 : numberValue;
            return 0;
        }
    }
}