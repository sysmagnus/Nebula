using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Nebula.Common.Models;

namespace Nebula.Modules.Cashier.Models;

[BsonIgnoreExtraElements]
public class CajaDiaria : IGenericModel
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Identificador de la empresa al que pertenece.
    /// </summary>
    public string CompanyId { get; set; } = string.Empty;

    /// <summary>
    /// ID Serie de facturación.
    /// </summary>
    public string InvoiceSerieId { get; set; } = string.Empty;

    /// <summary>
    /// Series de facturación.
    /// </summary>
    public string Terminal { get; set; } = string.Empty;

    /// <summary>
    /// Estado Caja (ABIERTO|CERRADO).
    /// </summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// Monto Apertura.
    /// </summary>
    public decimal TotalApertura { get; set; }

    /// <summary>
    /// Monto para el dia siguiente.
    /// </summary>
    public decimal TotalCierre { get; set; }

    /// <summary>
    /// Turno Operación de caja.
    /// </summary>
    public string Turno { get; set; } = string.Empty;

    /// <summary>
    /// Fecha de Operación.
    /// </summary>
    public string CreatedAt { get; set; } = DateTime.Now.ToString("yyyy-MM-dd");

    /// <summary>
    /// Año de registro.
    /// </summary>
    public string Year { get; set; } = DateTime.Now.ToString("yyyy");

    /// <summary>
    /// Mes de registro.
    /// </summary>
    public string Month { get; set; } = DateTime.Now.ToString("MM");

    /// <summary>
    /// Identificador del Almacén.
    /// Esta propiedad se usa solo para mostrar datos.
    /// </summary>
    [BsonIgnore]
    public string WarehouseId { get; set; } = string.Empty;
}
