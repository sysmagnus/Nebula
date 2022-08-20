using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using Nebula.Database.Models.Common;

namespace Nebula.Database.Services.Common;

public class UserService : CrudOperationService<User>
{
    public UserService(IOptions<DatabaseSettings> options) : base(options) { }

    public async Task<List<User>> GetListAsync(string? query, int limit = 25)
    {
        var filter = Builders<User>.Filter.Empty;
        if (!string.IsNullOrEmpty(query))
            filter = Builders<User>.Filter.Regex("UserName", new BsonRegularExpression(query.ToUpper(), "i"));
        return await _collection.Find(filter).Limit(limit).ToListAsync();
    }

    public async Task<User> GetByUserNameAsync(string userName)
    {
        var filter = Builders<User>.Filter.Eq(x => x.UserName, userName);
        return await _collection.Find(filter).FirstOrDefaultAsync();
    }
}