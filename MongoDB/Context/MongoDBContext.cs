using MongoDB.Driver;
using MongoDB.Entity;

namespace MongoDB.Context
{ 
    public class MongoDBContext
    {
        private readonly IMongoDatabase _database;

        public MongoDBContext(IConfiguration configuration)
        {
            var connectionString = "mongodb://localhost:27017/ali";
            var client = new MongoClient(connectionString);
            _database = client.GetDatabase("ali");
        }

        // create a collection 
        public IMongoCollection<Product> Products => _database.GetCollection<Product>("Products");
    }

}
