namespace AndrejKrizan.EntityFramework.PostgreSql.Settings
{
    public class DatabaseSettings
    {
        // Properties
        public string ConnectionString { get; set; }

        // Constructors
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        /// <summary>Constructor for IConfiguration.Get deserialization.</summary>
        public DatabaseSettings() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    }
}
