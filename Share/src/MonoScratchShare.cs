using System.Globalization;

namespace MonoScratch.Share {
    public static class MonoScratchShare {
        public const NumberStyles Styles =
                NumberStyles.AllowDecimalPoint |
                NumberStyles.AllowExponent |
                NumberStyles.AllowLeadingSign |
                NumberStyles.AllowLeadingWhite |
                NumberStyles.AllowTrailingWhite |
                NumberStyles.AllowTrailingSign;

        public static bool TryParseNum(string input, out double result) {
            return double.TryParse(input, Styles, null, out result);
        }
    }
}