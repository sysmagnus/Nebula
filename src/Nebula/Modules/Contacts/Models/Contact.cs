using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Nebula.Common.Models;
using System.ComponentModel.DataAnnotations;

namespace Nebula.Modules.Contacts.Models;

[BsonIgnoreExtraElements]
public class Contact : IGenericModel
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Identificador de la empresa al que pertenece.
    /// </summary>
    [Required(ErrorMessage = "CompanyId es requerido.")]
    public string CompanyId { get; set; } = string.Empty;

    /// <summary>
    /// Documento de Identidad.
    /// </summary>
    [Required(ErrorMessage = "Documento es requerido.")]
    public string Document { get; set; } = string.Empty;

    /// <summary>
    /// Tipo de documento.
    /// </summary>
    [Required(ErrorMessage = "Tipo documento es requerido.")]
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
