using System.Globalization;

namespace MonoScratch.Runtime {
    public class MonoScratchValue {

        public static MonoScratchValue ZERO = new MonoScratchValue(0);

        // Changes to this should be synced with BlockUtils in Compiler
        public const NumberStyles Styles =
            NumberStyles.AllowDecimalPoint |
            NumberStyles.AllowExponent |
            NumberStyles.AllowLeadingSign |
            NumberStyles.AllowLeadingWhite |
            NumberStyles.AllowTrailingWhite |
            NumberStyles.AllowTrailingSign;

        public string? _stringValue = null;
        public double _numberValue = 0;

        public MonoScratchValue() {
        }

        public MonoScratchValue(MonoScratchValue val) {
            Set(val);
        }

        public MonoScratchValue(string val) {
            Set(val);
        }

        public MonoScratchValue(double val) {
            Set(val);
        }

        public MonoScratchValue(bool val) {
            Set(val);
        }

        public void Set(MonoScratchValue val) {
            _stringValue = val._stringValue;
            _numberValue = val._numberValue;
        }

        public void Set(string val) {
            _stringValue = val;
            if (double.TryParse(_stringValue, Styles, null, out _numberValue))
                _numberValue = double.IsNaN(_numberValue) ? 0 : _numberValue;
            else _numberValue = 0;
        }

        public void Set(double val) {
            _stringValue = null;
            _numberValue = val;
        }

        public void Set(bool val) {
            _stringValue = val ? "true" : "false";
            _numberValue = double.NaN;
        }

        // Changes to this should be synced with BlockUtils in Compiler
        public double AsNumber() {
            return _numberValue;
        }

        public string AsString() {
            return _stringValue ?? _numberValue.ToString();
        }

        public bool AsBool() {
            if (_stringValue != null) {
                return !(_stringValue == "" || _stringValue == "0" || _stringValue == "false");
            }
            return !double.IsNaN(_numberValue) && _numberValue != 0;
        }

        public override string ToString() {
            return AsString();
        }

        public int Compare(MonoScratchValue other) {
            return Compare(this, other);
        }

        private static int CompareNotNaN(double n1, double n2) {
            n1 -= n2;
            return n1 < 0 ? -1 : (n1 == 0 ? 0 : 1);
        }

        private static int CompareStrings(string s1, string s2) {
            return string.Compare(s1.ToLower(), s2.ToLower());
        }

        public static int Compare(double n1, double n2) {
            if (double.IsNaN(n1) || double.IsNaN(n2))
                return string.Compare(n1.ToString(), n2.ToString());
            return CompareNotNaN(n1, n2);
        }

        public static int Compare(string v1, string v2) {
            double n1;
            if (!double.TryParse(v1, Styles, null, out n1))
                n1 = double.NaN;

            double n2;
            if (!double.TryParse(v2, Styles, null, out n2))
                n2 = double.NaN;

            if (double.IsNaN(n1) || double.IsNaN(n2))
                return string.Compare(v1.ToLower(), v2.ToLower());

            return CompareNotNaN(n1, n2);
        }

        // These two exist to avoid the construction of a MonoScratchValue when comparing
        //  a MonoScratchValue and a double directly.
        public static int Compare(MonoScratchValue v1, double n2) {
            double n1;
            if (v1._stringValue != null) {
                if (!double.TryParse(v1._stringValue, Styles, null, out n1))
                    n1 = double.NaN;
            } else n1 = v1._numberValue;
            if (double.IsNaN(n1) || double.IsNaN(n2))
                return CompareStrings(v1.AsString(), n2.ToString());
            return CompareNotNaN(n1, n2);
        }

        public static int Compare(double n1, MonoScratchValue v2) {
            double n2;
            if (v2._stringValue != null) {
                if (!double.TryParse(v2._stringValue, Styles, null, out n2))
                    n2 = double.NaN;
            } else n2 = v2._numberValue;
            if (double.IsNaN(n1) || double.IsNaN(n2))
                return CompareStrings(n1.ToString(), v2.AsString());
            return CompareNotNaN(n1, n2);
        }

        public static int Compare(MonoScratchValue v1, MonoScratchValue v2) {
            double n1;
            if (v1._stringValue != null) {
                if (!double.TryParse(v1._stringValue, Styles, null, out n1))
                    n1 = double.NaN;
            } else n1 = v1._numberValue;
            double n2;
            if (v2._stringValue != null) {
                if (!double.TryParse(v2._stringValue, Styles, null, out n2))
                    n2 = double.NaN;
            } else n2 = v2._numberValue;
            if (double.IsNaN(n1) || double.IsNaN(n2))
                return CompareStrings(v1.AsString(), v2.AsString());
            return CompareNotNaN(n1, n2);
        }

        public static implicit operator MonoScratchValue(double value) => new MonoScratchValue(value);
        public static implicit operator MonoScratchValue(string value) => new MonoScratchValue(value);
        public static implicit operator MonoScratchValue(bool value) => new MonoScratchValue(value);

    }
}