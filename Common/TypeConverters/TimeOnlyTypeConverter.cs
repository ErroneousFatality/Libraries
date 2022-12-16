namespace AndrejKrizan.Common.TypeConverters
{
    public class TimeOnlyStringConverter : TypeConverter<TimeOnly, string>
    {
        public TimeOnlyStringConverter() : base(
            time => time.ToString(),
            TimeOnly.Parse
        )
        { }
    }
}
