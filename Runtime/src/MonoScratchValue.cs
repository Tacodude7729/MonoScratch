using System.Globalization;

namespace MonoScratch.Runtime {
    public struct MonoScratchValue {
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
        }

        public void Set(double val) {
            _stringValue = null;
            _numberValue = val;
        }

        public void Set(bool val) {
            _stringValue = val ? "true" : "false";
        }

        // Changes to this should be synced with BlockUtils in Compiler
        public double AsNumber() {
            if (_stringValue != null) {
                if (double.TryParse(_stringValue, Styles, null, out double numberValue))
                    return double.IsNaN(numberValue) ? 0 : numberValue;
                return 0;
            }
            return double.IsNaN(_numberValue) ? 0 : _numberValue;
        }

        public string AsString() {
            return _stringValue ?? (_stringValue = _numberValue.ToString());
        }

        public bool AsBool() {
            if (_stringValue != null) {
                return !(_stringValue == "" || _stringValue == "0" || _stringValue == "false");
            }
            return !double.IsNaN(_numberValue) && _numberValue != 0;
        }
    }
}