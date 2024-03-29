using Nebula.Modules.Purchases.Models;

namespace Nebula.Modules.Purchases.Dto;

public class CabeceraCompraDto
{
    public string DocType { get; set; } = "BOLETA";
    public string ContactId { get; set; } = string.Empty;
    public string TipDocProveedor { get; set; } = string.Empty;
    public string NumDocProveedor { get; set; } = string.Empty;
    public string RznSocialProveedor { get; set; } = string.Empty;
    public string FecEmision { get; set; } = string.Empty;
    public string SerieComprobante { get; set; } = string.Empty;
    public string NumComprobante { get; set; } = string.Empty;
    public string TipoMoneda { get; set; } = string.Empty;
    public decimal TipoDeCambio { get; set; } = 1M;
    public string? FecVencimiento { get; set; } = null;

    public PurchaseInvoice GetPurchaseInvoice()
    {
        var fecha = DateTime.Parse(FecEmision);
        return new PurchaseInvoice()
        {
            Id = string.Empty,
            FecEmision = FecEmision,
            DocType = DocType,
            Serie = SerieComprobante.Trim(),
            Number = NumComprobante.Trim(),
            ContactId = ContactId,
            TipDocProveedor = TipDocProveedor.Trim(),
            NumDocProveedor = NumDocProveedor.Trim(),
            RznSocialProveedor = RznSocialProveedor.Trim(),
            TipMoneda = TipoMoneda,
            TipoCambio = TipoDeCambio,
            // calculo de importes.
            SumTotTributos = 0M,
            SumTotValCompra = 0M,
            SumPrecioCompra = 0M,
            SumImpCompra = 0M,
            Year = fecha.Year.ToString(),
            Month = fecha.Month.ToString("D2"),
        };
    }

    public PurchaseInvoice GetPurchaseInvoice(PurchaseInvoice purchase)
    {
        purchase.FecEmision = FecEmision;
        purchase.DocType = DocType;
        purchase.Serie = SerieComprobante.Trim();
        purchase.Number = NumComprobante.Trim();
        purchase.ContactId = ContactId;
        purchase.TipDocProveedor = TipDocProveedor.Trim();
        purchase.NumDocProveedor = NumDocProveedor.Trim();
        purchase.RznSocialProveedor = RznSocialProveedor.Trim();
        purchase.TipMoneda = TipoMoneda;
        purchase.TipoCambio = TipoDeCambio;
        var fecha = DateTime.Parse(FecEmision);
        purchase.Year = fecha.Year.ToString();
        purchase.Month = fecha.Month.ToString("D2");
        return purchase;
    }
}
