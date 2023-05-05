using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Nebula.Database.Helpers;

namespace Nebula.Database.Models.Common;

[BsonIgnoreExtraElements]
public class Product : IGeneric
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Descripción del producto.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Tipo IGV Sunat.
    /// </summary>
    public string IgvSunat { get; set; } = TipoIGV.Gravado;

    /// <summary>
    /// Impuesto a la bolsa plástica.
    /// </summary>
    public string Icbper { get; set; } = "NO";

    /// <summary>
    /// Valor Unitario (monto sin IGV).
    /// </summary>
    public decimal ValorUnitario { get; set; }

    /// <summary>
    /// Precio Venta Unitario (Incluye IGV).
    /// </summary>
    public decimal PrecioVentaUnitario { get; set; }

    /// <summary>
    /// Código de Barra.
    /// </summary>
    public string Barcode { get; set; } = "-";

    /// <summary>
    /// Código de producto SUNAT.
    /// </summary>
    public string CodProductoSUNAT { get; set; } = "-";

    /// <summary>
    /// Tipo de Bien/Servicio.
    /// </summary>
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// Unidad de Medida.
    /// </summary>
    public string UndMedida { get; set; } = string.Empty;

    /// <summary>
    /// Categoría de producto.
    /// </summary>
    public string Category { get; set; } = string.Empty;

    /// <summary>
    /// Control de Inventario en Tiempo Real.
    /// </summary>
    public string ControlStock { get; set; } = TipoControlStock.NONE;

    /// <summary>
    /// Path de la imagen del producto.
    /// </summary>
    public string PathImage { get; set; } = string.Empty;

    #region Lotes_de_Producción

    /// <summary>
    /// Lote de Producción.
    /// </summary>
    public string? LotNumber { get; set; } = string.Empty;

    /// <summary>
    /// Fecha de Vencimiento.
    /// </summary>
    public string? ExpirationDate { get; set; } = string.Empty;

    /// <summary>
    /// Fecha Recordatorio.
    /// </summary>
    public string? ReminderDate { get; set; } = string.Empty;

    #endregion
}
