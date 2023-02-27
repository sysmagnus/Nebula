namespace Nebula.Database.Dto.Sales;

public class CabeceraComprobanteDto
{
    public string DocType { get; set; } = "BOLETA";
    public string ContactId { get; set; } = string.Empty;
    public string TipDocUsuario { get; set; } = string.Empty;
    public string NumDocUsuario { get; set; } = string.Empty;
    public string RznSocialUsuario { get; set; } = string.Empty;
    public string FecVencimiento { get; set; } = "-";
    public string InvoiceSerieId { get; set; } = "-";
    public string Remark { get; set; } = string.Empty;
    #region DIRECCIÓN_DEL_CLIENTE!
    public string CodUbigeoCliente { get; set; } = "-";
    public string DesDireccionCliente { get; set; } = string.Empty;
    #endregion
}
