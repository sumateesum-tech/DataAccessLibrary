using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace DataAccessLibrary
{
    public static class DataValidator
    {
        public static void ThrowIfEmpty(string value, string fieldName)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException(fieldName + " is required.");
        }

        public static void ThrowIfNull(object value, string fieldName)
        {
            if (value == null)
                throw new ArgumentNullException(fieldName);
        }

        public static void ValidateLength(string value, string fieldName, int maxLength)
        {
            if (!string.IsNullOrEmpty(value) && value.Length > maxLength)
                throw new ArgumentException(fieldName + " exceeds maximum length of " + maxLength + ".");
        }

        public static bool IsInteger(string value)
        {
            int result;
            return int.TryParse(value, out result);
        }

        public static bool IsDecimal(string value)
        {
            decimal result;
            return decimal.TryParse(value, NumberStyles.Number, CultureInfo.InvariantCulture, out result);
        }

        public static bool IsDate(string value)
        {
            DateTime result;
            return DateTime.TryParse(value, CultureInfo.InvariantCulture, out result);
        }

        public static string SafeString(object value)
        {
            return value == null ? string.Empty : value.ToString().Trim();
        }
    }
}
