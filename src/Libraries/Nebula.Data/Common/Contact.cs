﻿using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Nebula.Data.Common;

/// <summary>
/// Libreta de Contactos.
/// </summary>
[BsonIgnoreExtraElements]
public class Contact : IGenericDocument
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Documento de Identidad.
    /// </summary>
    public string Document { get; set; } = string.Empty;

    /// <summary>
    /// Tipo de documento.
    /// </summary>
    public string DocType { get; set; } = string.Empty;

    /// <summary>
    /// Nombre de contacto.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Dirección de contacto.
    /// </summary>
    public string Address { get; set; } = string.Empty;

    /// <summary>
    /// Número Telefónico.
    /// </summary>
    public string PhoneNumber { get; set; } = string.Empty;

    /// <summary>
    /// Dirección del contacto (Código de ubigeo).
    /// </summary>
    public string CodUbigeo { get; set; } = string.Empty;
}
