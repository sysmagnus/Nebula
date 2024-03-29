using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using Nebula.Common.Models;
using Nebula.Modules.Inventory.Helpers;

namespace Nebula.Modules.Inventory.Models;

[BsonIgnoreExtraElements]
public class Transferencia : IGenericModel
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Identificador de la empresa al que pertenece.
    /// </summary>
    public string CompanyId { get; set; } = string.Empty;

    /// <summary>
    /// Nombre de Usuario.
    /// </summary>
    public string User { get; set; } = string.Empty;

    /// <summary>
    /// Identificador Almacén Origen.
    /// </summary>
    public string WarehouseOriginId { get; set; } = string.Empty;

    /// <summary>
    /// Nombre Almacén Origen.
    /// </summary>
    public string WarehouseOriginName { get; set; } = string.Empty;

    /// <summary>
    /// Identificador Almacén Destino.
    /// </summary>
    public string WarehouseTargetId { get; set; } = string.Empty;

    /// <summary>
    /// Nombre Almacén Destino.
    /// </summary>
    public string WarehouseTargetName { get; set; } = string.Empty;

    /// <summary>
    /// Estado Inventario.
    /// </summary>
    public string Status { get; set; } = InventoryStatus.BORRADOR;

    /// <summary>
    /// Observación.
    /// </summary>
    public string Remark { get; set; } = string.Empty;

    /// <summary>
    /// Fecha de Registro.
    /// </summary>
    public string CreatedAt { get; set; } = DateTime.Now.ToString("yyyy-MM-dd");

    /// <summary>
    /// Año de Registro.
    /// </summary>
    public string Year { get; set; } = DateTime.Now.ToString("yyyy");

    /// <summary>
    /// Mes de Registro.
    /// </summary>
    public string Month { get; set; } = DateTime.Now.ToString("MM");
}
