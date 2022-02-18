
using MonoScratch.Share;

namespace MonoScratch.Compiler {

    public static class BlockUtils {

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
            if (MonoScratchShare.TryParseNum(_stringValue, out double numberValue))
                return double.IsNaN(numberValue) ? 0 : numberValue;
            return 0;
        }
    }
}