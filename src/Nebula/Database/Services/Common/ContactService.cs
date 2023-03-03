using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using Nebula.Database.Models.Common;

namespace Nebula.Database.Services.Common;

public class ContactService : CrudOperationService<Contact>
{
    public ContactService(IOptions<DatabaseSettings> options) : base(options) { }

    public async Task<Contact> GetContactByDocumentAsync(string document) =>
        await _collection.Find(x => x.Document == document).FirstOrDefaultAsync();

    public async Task<List<Contact>> GetContactsAsync(string query = "", int limit = 25)
    {
        var builder = Builders<Contact>.Filter;
        var filter = builder.Or(builder.Regex("Document", new BsonRegularExpression(query, "i")),
            builder.Regex("Name", new BsonRegularExpression(query.ToUpper(), "i")));
        return await _collection.Find(filter).Limit(limit).ToListAsync();
    }
}