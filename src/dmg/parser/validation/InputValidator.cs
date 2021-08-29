using System;

namespace DMG.Parser.Validation {
    public static class InputValidationExtensions {
        public static int ValidateCastToInt(this object obj) {
            try {
                return Convert.ToInt32(obj);
            } catch (InvalidCastException) {
                throw new ValidationException("Integer value expected");
            }
        }

        public static bool ValidateCastToBool(this object obj) {
            try {
                return Convert.ToBoolean(obj);
            } catch (InvalidCastException) {
                throw new ValidationException("Boolean value expected");
            }
        }

        public static string ValidateCastToString(this object obj) {
            try {
                return Convert.ToString(obj);
            } catch (InvalidCastException) {
                throw new ValidationException("String value expected");
            }
        }

        public static T ValidateCastToEnum<T>(this object obj) {
            var val = obj.ValidateCastToString().ToLowerInvariant().Trim();

            foreach (T enumConst in Enum.GetValues(typeof(T))) {
                if (Enum.GetName(typeof(T), enumConst).ToLowerInvariant().Trim().Equals(val)) {
                    return enumConst;
                }
            }

            throw new ValidationException("Provided value is not a valid option");
        }
    }
}
