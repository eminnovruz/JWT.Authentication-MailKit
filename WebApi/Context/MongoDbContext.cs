using Microsoft.Extensions.Options;
using MongoDB.Driver;
using WebApi.Configuration.MongoDb;
using WebApi.Models;

namespace WebApi.Context;

public class MongoDbContext
{
    private readonly IMongoDatabase _database;

    public MongoDbContext(IOptions<MongoDbConfiguration> settings)
    {
        MongoClient client = new MongoClient(settings.Value.ConnectionString);

        _database = client.GetDatabase(settings.Value.DatabaseName);
    }

    public IMongoCollection<User> Users => _database.GetCollection<User>("users");
}