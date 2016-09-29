using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Nop.Plugin.Api.Converters
{
    public class ApiTypeConverter : IApiTypeConverter
    {
        public DateTime? ToDateTimeNullable(string value)
        {
            DateTime result;

            var formats = new string[]
            {
                "yyyy-MM-dd",
                "yyyy-MM-ddTHH:mm",
                "yyyy-MM-ddTHH:mm:ss",
                "yyyy-MM-ddTHH:mm:sszzz",
            };

            if (DateTime.TryParseExact(value, formats, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out result))
            {
                return result;
            }

            return null;
        }

        public int ToInt(string value)
        {
            int result;

            if (int.TryParse(value, out result))
            {
                return result;
            }

            return 0;
        }

        public int? ToIntNullable(string value)
        {
            int result;

            if (int.TryParse(value, out result))
            {
                return result;
            }

            return null;
        }

        public IList<int> ToListOfInts(string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                List<string> stringIds = value.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                List<int> intIds = new List<int>();

                foreach (var id in stringIds)
                {
                    int intId;
                    if (int.TryParse(id, out intId))
                    {
                        intIds.Add(intId);
                    }
                }

                intIds = intIds.Distinct().ToList();
                return intIds.Count > 0 ? intIds : null;
            }

            return null;
        }

        public bool? ToStatus(string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                if (value.Equals("published", StringComparison.InvariantCultureIgnoreCase))
                {
                    return true;
                }
                else if (value.Equals("unpublished", StringComparison.InvariantCultureIgnoreCase))
                {
                    return false;
                }
            }

            return null;
        }

        public object ToEnumNullable(string value, Type type)
        {
            if (!string.IsNullOrEmpty(value))
            {
                Type enumType = Nullable.GetUnderlyingType(type);

                var enumNames = enumType.GetEnumNames();

                if (enumNames.Any(x => x.ToLowerInvariant().Equals(value.ToLowerInvariant())))
                {
                    return Enum.Parse(enumType, value, true);
                }
            }

            return null;
        }
    }
}