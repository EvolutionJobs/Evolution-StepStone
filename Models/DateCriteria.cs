namespace EvoApi.Services.StepStone.Models
{
    using Newtonsoft.Json;
    using System;

    class DateCriteriaConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
                return;
            else if (value is DateCriteria dc)
                writer.WriteValue(dc.ToString(serializer.DateFormatString));
            else if (value is DateTime d)
                writer.WriteValue(d.ToString(serializer.DateFormatString));
            else if (value is string s)
                writer.WriteValue(s);
            else
                writer.WriteValue(value);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            string v = (string)reader.Value;
            if (DateTime.TryParse(v, out var d))
                return (DateCriteria)d;

            return (DateCriteria)v;
        }

        public override bool CanConvert(Type objectType) =>
            objectType == typeof(DateCriteria) ||
            objectType == typeof(string) ||
            objectType == typeof(DateTime) ||
            objectType == typeof(DateTime?);
    }

    /// <summary>Represent a filter criteria value that can be either a date or a facet string.</summary>
    [JsonConverter(typeof(DateCriteriaConverter))]
    public class DateCriteria
    {
        DateCriteria() { }

        DateCriteria(string facet) { this.strValue = facet; }

        DateCriteria(DateTime date) { this.dateValue = date; }

        readonly string strValue;

        readonly DateTime? dateValue;

        public override string ToString() =>
            this.dateValue != null ? this.dateValue.Value.ToString() : this.strValue;

        public string ToString(string dateFormatString) =>
            this.dateValue != null ? this.dateValue.Value.ToString(dateFormatString) : this.strValue;

        public static implicit operator DateCriteria(DateTime d) =>
            new DateCriteria(d);

        public static implicit operator DateCriteria(string s) =>
            new DateCriteria(s);

        public static implicit operator string(DateCriteria crit) =>
            crit?.ToString();
    }
}
