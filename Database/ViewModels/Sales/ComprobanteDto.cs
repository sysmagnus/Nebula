using Nebula.Database.Models.Common;
using Nebula.Database.Models.Sales;

namespace Nebula.Database.ViewModels.Sales;

public class ComprobanteDto
{
    struct ImporteItem
    {
        /// <summary>
        /// Valor Unitario sin Ningún Tributo.
        /// </summary>
        public decimal MtoValorUnitario { get; set; }
        /// <summary>
        /// Sumatoria Tributos por item, = 9 + 16 + 23
        /// </summary>
        public decimal SumTotTributosItem { get; set; }
        /// <summary>
        /// Monto de IGV por ítem.
        /// </summary>
        public decimal MtoIgvItem { get; set; }
        /// <summary>
        /// Base Imponible IGV por Item.
        /// Siempre mayor a cero. Si el item tiene ISC, agregar dicho monto a la base imponible.
        /// </summary>
        public decimal MtoBaseIgvItem { get; set; }
        /// <summary>
        /// Monto de tributo ICBPER por iItem.
        /// </summary>
        public decimal MtoTriIcbperItem { get; set; }
        /// <summary>
        /// Total Venta Item sin Ningún Tributo.
        /// </summary>
        public decimal MtoValorVentaItem { get; set; }
        /// <summary>
        /// El mismo valor que ItemComprobante.
        /// Solo para Generar los Tributos del Comprobante.
        /// GRAVADO, EXONERADO, GRATUITO.
        /// </summary>
        public string IgvSunat { get; set; }
    }
    struct ImporteVenta
    {
        /// <summary>
        /// Total Precio de Venta.
        /// </summary>
        public decimal SumPrecioVenta { get; set; }
        /// <summary>
        /// Total valor de venta.
        /// </summary>
        public decimal SumTotValVenta { get; set; }
        /// <summary>
        /// Sumatoria Tributos.
        /// </summary>
        public decimal SumTotTributos { get; set; }
        /// <summary>
        /// Sumatoria Tributo ICBPER.
        /// </summary>
        public decimal SumTotTriIcbper { get; set; }
        /// <summary>
        /// Importe total de la venta, cesión en uso o del servicio prestado.
        /// </summary>
        public decimal SumImpVenta { get; set; }
    }
    private Configuration _configuration = new Configuration();
    #region ORIGIN_HTTP_REQUEST!
    public CabeceraComprobanteDto Cabecera { get; set; } = new CabeceraComprobanteDto();
    public List<ItemComprobanteDto> Detalle { get; set; } = new List<ItemComprobanteDto>();
    public DatoPagoComprobanteDto DatoPago { get; set; } = new DatoPagoComprobanteDto();
    public List<CuotaPagoComprobanteDto> DetallePago { get; set; } = new List<CuotaPagoComprobanteDto>();
    #endregion
    private List<ImporteItem> ImporteItems { get; set; } = new List<ImporteItem>();
    public void SetConfiguration(Configuration configuration)
    {
        _configuration = configuration;
    }

    private ImporteVenta CalcularImporteVenta()
    {
        var importeVenta = new ImporteVenta();
        Detalle.ForEach(item =>
        {
            ImporteItem itemObj = new ImporteItem();
            decimal porcentajeIGV = item.IgvSunat == "GRAVADO" ? (_configuration.PorcentajeIgv / 100) + 1 : 1;
            itemObj.MtoValorVentaItem = item.MtoTotalItem / porcentajeIGV;
            itemObj.MtoTriIcbperItem = item.TriIcbper ? item.CtdUnidadItem * _configuration.ValorImpuestoBolsa : 0;
            itemObj.MtoBaseIgvItem = item.MtoTotalItem / porcentajeIGV;
            itemObj.MtoIgvItem = item.MtoTotalItem - itemObj.MtoBaseIgvItem;
            itemObj.SumTotTributosItem = itemObj.MtoIgvItem + itemObj.MtoTriIcbperItem; // el sistema soporta solo IGV/ICBPER.
            itemObj.MtoValorUnitario = item.IgvSunat != "GRATUITO" ? itemObj.MtoValorVentaItem / item.CtdUnidadItem : 0;
            decimal precioVentaUnitario = item.IgvSunat != "GRATUITO" ? item.MtoPrecioVentaUnitario : 0;
            importeVenta.SumTotTributos += itemObj.SumTotTributosItem;
            importeVenta.SumTotValVenta += item.IgvSunat != "GRATUITO" ? itemObj.MtoValorVentaItem : 0;
            importeVenta.SumPrecioVenta += item.CtdUnidadItem * precioVentaUnitario;
            // importeVenta.SumTotTriIcbper += itemObj.MtoTriIcbperItem;
            // importeVenta.SumImpVenta = importeVenta.SumTotValVenta + importeVenta.SumTotTributos + importeVenta.SumTotTriIcbper;
            importeVenta.SumImpVenta = importeVenta.SumTotValVenta + importeVenta.SumTotTributos;
        });
        return importeVenta;
    }

    public InvoiceSale GetInvoiceSale()
    {
        var importeVenta = CalcularImporteVenta();
        return new InvoiceSale
        {
            DocType = Cabecera.DocType,
            TipOperacion = "0101",
            FecEmision = DateTime.Now.ToString("yyyy-MM-dd"),
            HorEmision = DateTime.Now.ToString("HH:mm:ss"),
            FecVencimiento = Cabecera.FecVencimiento,
            CodLocalEmisor = _configuration.CodLocalEmisor,
            FormaPago = DatoPago.FormaPago,
            ContactId = Cabecera.ContactId,
            TipDocUsuario = Cabecera.TipDocUsuario,
            NumDocUsuario = Cabecera.NumDocUsuario,
            RznSocialUsuario = Cabecera.RznSocialUsuario,
            TipMoneda = _configuration.TipMoneda,
            SumTotValVenta = importeVenta.SumTotValVenta,
            SumPrecioVenta = importeVenta.SumPrecioVenta,
            SumTotTributos = importeVenta.SumTotTributos,
            SumImpVenta = importeVenta.SumImpVenta,
            Year = DateTime.Now.ToString("yyyy"),
            Month = DateTime.Now.ToString("MM"),
        };
    }

    public List<InvoiceSaleDetail> GetInvoiceSaleDetails(string invoiceId)
    {
        var invoiceSaleDetails = new List<InvoiceSaleDetail>();
        Detalle.ForEach(item =>
        {
            // Tributo: Afectación al IGV por ítem.
            string tipAfeIgv = "10";
            string codTriIgv = string.Empty;
            string nomTributoIgvItem = string.Empty;
            string codTipTributoIgvItem = string.Empty;
            switch (item.IgvSunat)
            {
                case "GRAVADO":
                    tipAfeIgv = "10";
                    codTriIgv = "1000";
                    nomTributoIgvItem = "IGV";
                    codTipTributoIgvItem = "VAT";
                    break;
                case "EXONERADO":
                    tipAfeIgv = "20";
                    codTriIgv = "9997";
                    nomTributoIgvItem = "EXO";
                    codTipTributoIgvItem = "VAT";
                    break;
                case "GRATUITO":
                    tipAfeIgv = "21";
                    codTriIgv = "9996";
                    nomTributoIgvItem = "GRA";
                    codTipTributoIgvItem = "FRE";
                    break;
            }
            ImporteItem importeItem = new ImporteItem();
            importeItem.IgvSunat = item.IgvSunat;
            decimal porcentajeIGV = item.IgvSunat == "GRAVADO" ? (_configuration.PorcentajeIgv / 100) + 1 : 1;
            importeItem.MtoValorVentaItem = item.MtoTotalItem / porcentajeIGV;
            importeItem.MtoTriIcbperItem = item.TriIcbper ? item.CtdUnidadItem * _configuration.ValorImpuestoBolsa : 0;
            importeItem.MtoBaseIgvItem = item.MtoTotalItem / porcentajeIGV;
            importeItem.MtoIgvItem = item.MtoTotalItem - importeItem.MtoBaseIgvItem;
            importeItem.SumTotTributosItem = importeItem.MtoIgvItem + importeItem.MtoTriIcbperItem; // el sistema soporta solo IGV/ICBPER.
            importeItem.MtoValorUnitario = item.IgvSunat != "GRATUITO" ? importeItem.MtoValorVentaItem / item.CtdUnidadItem : 0;
            ImporteItems.Add(importeItem);
            // agregar items al comprobante.
            invoiceSaleDetails.Add(new InvoiceSaleDetail
            {
                InvoiceSale = invoiceId,
                CajaDiaria = "-",
                TipoItem = item.TipoItem,
                CodUnidadMedida = item.CodUnidadMedida,
                CtdUnidadItem = item.CtdUnidadItem,
                CodProducto = "-",
                CodProductoSunat = "-", // refactorizar agregar código de barra.
                DesItem = item.DesItem,
                MtoValorUnitario = importeItem.MtoValorUnitario,
                SumTotTributosItem = importeItem.SumTotTributosItem,
                // Tributo: IGV(1000).
                CodTriIgv = codTriIgv,
                MtoIgvItem = importeItem.MtoIgvItem,
                MtoBaseIgvItem = importeItem.MtoBaseIgvItem,
                NomTributoIgvItem = nomTributoIgvItem,
                CodTipTributoIgvItem = codTipTributoIgvItem,
                TipAfeIgv = tipAfeIgv,
                PorIgvItem = item.IgvSunat == "GRAVADO" ? _configuration.PorcentajeIgv.ToString("N2") : "0.00",
                // Tributo ICBPER 7152.
                CodTriIcbper = item.TriIcbper ? "7152" : "-",
                MtoTriIcbperItem = item.TriIcbper ? importeItem.MtoTriIcbperItem : 0,
                CtdBolsasTriIcbperItem = item.TriIcbper ? Convert.ToInt32(item.CtdUnidadItem) : 0,
                NomTributoIcbperItem = "ICBPER",
                CodTipTributoIcbperItem = "OTH",
                MtoTriIcbperUnidad = item.TriIcbper ? _configuration.ValorImpuestoBolsa : 0,
                // Precio de Venta Unitario.
                MtoPrecioVentaUnitario = item.IgvSunat != "GRATUITO" ? item.MtoPrecioVentaUnitario : 0,
                MtoValorVentaItem = importeItem.MtoValorVentaItem,
                MtoValorReferencialUnitario = item.IgvSunat == "GRATUITO" ? item.MtoPrecioVentaUnitario : 0,
                WarehouseId = item.WarehouseId,
            });
        });
        return invoiceSaleDetails;
    }

    public List<TributoSale> GetTributoSales(string invoiceId)
    {
        decimal opGravada = 0;
        decimal opExonerada = 0;
        decimal opGratuita = 0;
        decimal totalIgv = 0;
        decimal totalIcbper = 0;
        ImporteItems.ForEach(item =>
        {
            totalIcbper += item.MtoTriIcbperItem;
            switch (item.IgvSunat)
            {
                case "GRAVADO":
                    opGravada += item.MtoBaseIgvItem;
                    totalIgv += item.MtoIgvItem;
                    break;
                case "EXONERADO":
                    opExonerada += item.MtoBaseIgvItem;
                    break;
                case "GRATUITO":
                    opGratuita += item.MtoBaseIgvItem;
                    break;
            }
        });
        var tributos = new List<TributoSale>();
        if (opGratuita > 0)
        {
            tributos.Add(new TributoSale()
            {
                InvoiceSale = invoiceId,
                IdeTributo = "9996",
                NomTributo = "GRA",
                CodTipTributo = "FRE",
                MtoBaseImponible = opGratuita,
                MtoTributo = 0,
                Year = DateTime.Now.ToString("yyyy"),
                Month = DateTime.Now.ToString("MM")
            });
        }

        if (opExonerada > 0)
        {
            tributos.Add(new TributoSale()
            {
                InvoiceSale = invoiceId,
                IdeTributo = "9997",
                NomTributo = "EXO",
                CodTipTributo = "VAT",
                MtoBaseImponible = opExonerada,
                MtoTributo = 0,
                Year = DateTime.Now.ToString("yyyy"),
                Month = DateTime.Now.ToString("MM")
            });
        }

        if (opGravada > 0)
        {
            tributos.Add(new TributoSale()
            {
                InvoiceSale = invoiceId,
                IdeTributo = "1000",
                NomTributo = "IGV",
                CodTipTributo = "VAT",
                MtoBaseImponible = opGravada,
                MtoTributo = totalIgv,
                Year = DateTime.Now.ToString("yyyy"),
                Month = DateTime.Now.ToString("MM")
            });
        }

        if (totalIcbper > 0)
        {
            tributos.Add(new TributoSale()
            {
                InvoiceSale = invoiceId,
                IdeTributo = "7152",
                NomTributo = "ICBPER",
                CodTipTributo = "OTH",
                MtoBaseImponible = 0,
                MtoTributo = totalIcbper,
                Year = DateTime.Now.ToString("yyyy"),
                Month = DateTime.Now.ToString("MM")
            });
        }

        return tributos;
    }
}
