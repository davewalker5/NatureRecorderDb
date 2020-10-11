using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace NatureRecorder.Entities.DataExchange
{
    [ExcludeFromCodeCoverage]
    public abstract class ExportableEntity
    {
        private const string DateTimeFormat = "dd/MM/yyyy";

        /// <summary>
        /// Create a CSV field value from a property value
        /// </summary>
        /// <param name="value"></param>
        /// <param name="isLastColumn"></param>
        protected string MakeCsvField(object value, bool isLastColumn = false)
        {
            string csvValue = "";

            if (value != null)
            {
                string type = value.GetType().Name;
                switch (type)
                {
                    case "DateTime":
                        csvValue = ((DateTime)value).ToString(DateTimeFormat);
                        break;
                    default:
                        csvValue = value.ToString();
                        break;
                }
            }

            string separator = (isLastColumn) ? "" : ",";
            return $"\"{csvValue}\"{separator}";
        }

        /// <summary>
        /// Return a date time from a CSV field
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        protected static DateTime GetDateTimeFromField(string field)
        {
            return DateTime.ParseExact(field, DateTimeFormat, CultureInfo.CurrentCulture);
        }

        /// <summary>
        /// Return a nullable date time from a CSV field
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        protected static DateTime? GetNullableDateTimeFromField(string field)
        {
            DateTime? date = null;

            if (!string.IsNullOrEmpty(field))
            {
                DateTime nonNullDateTime = DateTime.ParseExact(field, DateTimeFormat, CultureInfo.CurrentCulture);
                date = nonNullDateTime;
            }

            return date;
        }
    }
}
