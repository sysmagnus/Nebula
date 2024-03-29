using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Nebula.Common.Models;

namespace Nebula.Modules.Inventory.Models;

[BsonIgnoreExtraElements]
public class LocationDetail : IGenericModel
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Identificador de la empresa al que pertenece.
    /// </summary>
    public string CompanyId { get; set; } = string.Empty;

    /// <summary>
    /// Identificador de Ubicación.
    /// </summary>
    public string LocationId { get; set; } = string.Empty;

    /// <summary>
    /// Identificador del Producto.
    /// </summary>
    public string ProductId { get; set; } = string.Empty;

    /// <summary>
    /// Nombre del Producto.
    /// </summary>
    public string ProductName { get; set; } = string.Empty;

    /// <summary>
    /// Código de Barras.
    /// </summary>
    public string Barcode { get; set; } = string.Empty;

    /// <summary>
    /// Cantidad Máxima de Inventario.
    /// </summary>
    public int QuantityMax { get; set; }

    /// <summary>
    /// Cantidad Mínima de Inventario.
    /// </summary>
    public int QuantityMin { get; set; }
}
