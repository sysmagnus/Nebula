using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Nebula.Database.Models.Inventory;

public class AjusteInventarioDetail : Generic
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Clave foranea Ajuste Inventario.
    /// </summary>
    public string AjusteInventarioId { get; set; } = string.Empty;

    /// <summary>
    /// Identificador del Producto.
    /// </summary>
    public string ProductId { get; set; } = string.Empty;

    /// <summary>
    /// Nombre del Producto.
    /// </summary>
    public string ProductName { get; set; } = string.Empty;

    /// <summary>
    /// Cantidad Existente.
    /// </summary>
    public long CantExistente { get; set; }

    /// <summary>
    /// Cantidad Contada.
    /// </summary>
    public long CantContada { get; set; }

    /// <summary>
    /// Cantidad Restante.
    /// </summary>
    public long CantRestante { get; set; }
}
