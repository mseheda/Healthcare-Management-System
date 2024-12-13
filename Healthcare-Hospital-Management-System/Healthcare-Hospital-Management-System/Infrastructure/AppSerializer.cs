using System.Globalization;

namespace Healthcare_Hospital_Management_System.Infrastructure
{
    public class AppSerializer
    {
        public string SerializeDateTime(DateTime dateTime)
        {
            return dateTime.ToString("o", CultureInfo.CurrentCulture);
        }

        public DateTime DeserializeDateTime(string dateTimeString)
        {
            return DateTime.Parse(dateTimeString, CultureInfo.CurrentCulture, DateTimeStyles.RoundtripKind);
        }

        public string SerializeNumber<T>(T number) where T : struct, IFormattable
        {
            return number.ToString(null, CultureInfo.CurrentCulture);
        }

        public T DeserializeNumber<T>(string numberString) where T : struct, IConvertible
        {
            var typeCode = Type.GetTypeCode(typeof(T));
            switch (typeCode)
            {
                case TypeCode.Int32:
                    return (T)(object)int.Parse(numberString, CultureInfo.CurrentCulture);
                case TypeCode.Double:
                    return (T)(object)double.Parse(numberString, CultureInfo.CurrentCulture);
                default:
                    throw new NotSupportedException($"Type {typeof(T)} is not supported.");
            }
        }
    }
}
