using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Nebula.Common.Models;
using Nebula.Modules.Inventory.Helpers;

namespace Nebula.Modules.Inventory.Models;

[BsonIgnoreExtraElements]
public class ProductStock : IGenericModel
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Identificador Almacén.
    /// </summary>
    public string WarehouseId { get; set; } = string.Empty;

    /// <summary>
    /// Identificador Producto.
    /// </summary>
    public string ProductId { get; set; } = string.Empty;

    /// <summary>
    /// Identificador del Lote del producto.
    /// </summary>
    public string ProductLoteId { get; set; } = string.Empty;

    /// <summary>
    /// Tipo de Registro.
    /// </summary>
    public string Type { get; set; } = InventoryType.ENTRADA;

    /// <summary>
    /// Cantidad de Productos.
    /// </summary>
    public long Quantity { get; set; }
}
