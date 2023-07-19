using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Nebula.Common.Models;

namespace Nebula.Modules.Auth.Models;

[BsonIgnoreExtraElements]
public class User : IGenericModel
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = string.Empty;

    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string RolesId { get; set; } = string.Empty;
}
