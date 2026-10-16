namespace Tctrieu.VnptIt.Crud.Infrastructure.Persistence.Mongo
{
    // Cấu hình MongoDB, đọc từ section "MongoDb" trong appsettings.json
    public class MongoSettings
    {
        public const string SECTION_NAME = "MongoDb";

        public string ConnectionString { get; set; } = default!;
        public string DatabaseName { get; set; } = default!;
    }
}
