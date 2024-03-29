using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Nebula.Common.Models;

namespace Nebula.Modules.Cashier.Models;

/// <summary>
/// Detalle de caja diaria.
/// </summary>
[BsonIgnoreExtraElements]
public class CashierDetail : IGenericModel
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Identificador de la empresa al que pertenece.
    /// </summary>
    public string CompanyId { get; set; } = string.Empty;

    /// <summary>
    /// Identificador CajaDiaria.
    /// </summary>
    public string CajaDiariaId { get; set; } = string.Empty;

    /// <summary>
    /// Clave foránea comprobante de venta.
    /// </summary>
    public string InvoiceSaleId { get; set; } = "-";

    /// <summary>
    /// Configura el Tipo de comprobante.
    /// </summary>
    public string DocType { get; set; } = "NOTA";

    /// <summary>
    /// Serie y Número de documento.
    /// </summary>
    public string Document { get; set; } = "-";

    /// <summary>
    /// Identificador de Contacto.
    /// </summary>
    public string ContactId { get; set; } = "-";

    /// <summary>
    /// Nombre de Contacto.
    /// </summary>
    public string ContactName { get; set; } = "-";

    /// <summary>
    /// Observación de la Operación.
    /// </summary>
    public string Remark { get; set; } = "-";

    /// <summary>
    /// Definir Tipo de registro.
    /// </summary>
    public string TypeOperation { get; set; } = string.Empty;

    /// <summary>
    /// Forma de Pago (Credito|Contado).
    /// </summary>
    public string FormaPago { get; set; } = string.Empty;

    /// <summary>
    /// Monto de la Operación.
    /// </summary>
    public decimal Amount { get; set; }

    /// <summary>
    /// Hora de la Operación.
    /// </summary>
    public string Hour { get; set; } = DateTime.Now.ToString("HH:mm");

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
}
