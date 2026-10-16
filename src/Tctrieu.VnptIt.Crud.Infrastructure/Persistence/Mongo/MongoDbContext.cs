using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using Tctrieu.VnptIt.Crud.Domain.Entities;

namespace Tctrieu.VnptIt.Crud.Infrastructure.Persistence.Mongo
{
    // Giữ kết nối MongoDB và các collection, đăng ký singleton (MongoClient tự quản lý connection pool)
    public class MongoDbContext
    {
        private const string FILES_COLLECTION = "files";

        private static readonly object _mapLock = new object();

        private readonly IMongoDatabase _database;

        public MongoDbContext(MongoSettings settings)
        {
            RegisterClassMaps();

            var client = new MongoClient(settings.ConnectionString);
            _database = client.GetDatabase(settings.DatabaseName);
        }

        public IMongoCollection<StoredFile> Files => _database.GetCollection<StoredFile>(FILES_COLLECTION);

        // Map entity sang document bằng code để tầng Domain không phụ thuộc MongoDB
        private static void RegisterClassMaps()
        {
            lock (_mapLock)
            {
                if (BsonClassMap.IsClassMapRegistered(typeof(StoredFile)))
                    return;

                BsonClassMap.RegisterClassMap<StoredFile>(cm =>
                {
                    cm.MapIdMember(x => x.Id)
                        .SetIdGenerator(StringObjectIdGenerator.Instance)
                        .SetSerializer(new StringSerializer(BsonType.ObjectId));
                    cm.MapMember(x => x.OriginalName).SetElementName("original_name");
                    cm.MapMember(x => x.StoredPath).SetElementName("stored_path");
                    cm.MapMember(x => x.ContentType).SetElementName("content_type");
                    cm.MapMember(x => x.Size).SetElementName("size");
                    cm.MapMember(x => x.UploadedBy).SetElementName("uploaded_by");
                    cm.MapMember(x => x.CreatedAt).SetElementName("created_at");
                    cm.SetIgnoreExtraElements(true);
                });
            }
        }
    }
}
