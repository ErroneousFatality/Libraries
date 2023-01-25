namespace AndrejKrizan.DotNet.TypeConverters
{
    public class DateOnlyStringConverter : TypeConverter<DateOnly, string>
    {
        public DateOnlyStringConverter() : base(
            date => date.ToString(),
            DateOnly.Parse
        )
        { }
    }
}
