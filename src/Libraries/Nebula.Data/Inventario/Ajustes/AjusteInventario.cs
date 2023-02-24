﻿using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Nebula.Core.Constants;

namespace Nebula.Data.Inventario.Ajustes;

/// <summary>
/// Inicializa los contadores de una ubicación.
/// </summary>
[BsonIgnoreExtraElements]
public class AjusteInventario : IGenericDocument
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Nombre del Usuario.
    /// </summary>
    public string User { get; set; } = string.Empty;

    /// <summary>
    /// Identificador del Almacén.
    /// </summary>
    public string WarehouseId { get; set; } = string.Empty;

    /// <summary>
    /// Nombre del Almacén.
    /// </summary>
    public string WarehouseName { get; set; } = string.Empty;

    /// <summary>
    /// Identificador de la Ubicación.
    /// </summary>
    public string LocationId { get; set; } = string.Empty;

    /// <summary>
    /// Nombre de la Ubicación.
    /// </summary>
    public string LocationName { get; set; } = string.Empty;

    /// <summary>
    /// Estado del Inventario.
    /// </summary>
    public string Status { get; set; } = InventoryStatus.Borrador;

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
