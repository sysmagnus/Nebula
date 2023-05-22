using Nebula.Database.Dto.Sales;
using Nebula.Common;
using Nebula.Modules.Sales.Models;
using Nebula.Modules.Finanzas.Models;
using Nebula.Modules.Finanzas;
using Nebula.Modules.Configurations.Models;
using Nebula.Modules.Configurations;
using Nebula.Modules.Cashier.Helpers;

namespace Nebula.Modules.Sales;

public class ComprobanteService
{
    private readonly ConfigurationService _configurationService;
    private readonly InvoiceSaleService _invoiceSaleService;
    private readonly InvoiceSaleDetailService _invoiceSaleDetailService;
    private readonly TributoSaleService _tributoSaleService;
    private readonly CrudOperationService<InvoiceSerie> _invoiceSerieService;
    private readonly DetallePagoSaleService _detallePagoSaleService;
    private readonly ReceivableService _receivableService;

    public ComprobanteService(ConfigurationService configurationService,
        InvoiceSaleService invoiceSaleService, InvoiceSaleDetailService invoiceSaleDetailService,
        TributoSaleService tributoSaleService, CrudOperationService<InvoiceSerie> invoiceSerieService,
        DetallePagoSaleService detallePagoSaleService, ReceivableService receivableService)
    {
        _configurationService = configurationService;
        _invoiceSaleService = invoiceSaleService;
        _invoiceSaleDetailService = invoiceSaleDetailService;
        _tributoSaleService = tributoSaleService;
        _invoiceSerieService = invoiceSerieService;
        _detallePagoSaleService = detallePagoSaleService;
        _receivableService = receivableService;
    }

    /// <summary>
    /// modelo de datos.
    /// </summary>
    private ComprobanteDto comprobanteDto = new ComprobanteDto();

    /// <summary>
    /// cargar modelo de datos al servicio.
    /// </summary>
    public void SetComprobanteDto(ComprobanteDto dto)
    {
        comprobanteDto = dto;
    }

    public async Task<InvoiceSale> SaveChangesAsync()
    {
        var configuration = await _configurationService.GetAsync();
        comprobanteDto.SetConfiguration(configuration);
        var invoiceSale = comprobanteDto.GetInvoiceSale();
        var invoiceSerieId = comprobanteDto.Cabecera.InvoiceSerieId;
        var invoiceSerie = await _invoiceSerieService.GetByIdAsync(invoiceSerieId);
        comprobanteDto.GenerarSerieComprobante(ref invoiceSerie, ref invoiceSale);
        invoiceSale.InvoiceSerieId = invoiceSerie.Id;

        // agregar Información del comprobante.
        await _invoiceSerieService.UpdateAsync(invoiceSerie.Id, invoiceSerie);
        await _invoiceSaleService.CreateAsync(invoiceSale);

        // agregar detalles del comprobante.
        var invoiceSaleDetails = comprobanteDto.GetInvoiceSaleDetails(invoiceSale.Id);
        await _invoiceSaleDetailService.CreateManyAsync(invoiceSaleDetails);

        // agregar Tributos de Factura.
        var tributoSales = comprobanteDto.GetTributoSales(invoiceSale.Id);
        await _tributoSaleService.CreateManyAsync(tributoSales);

        // registrar detalle de pago si la operación es a crédito.
        if (comprobanteDto.DatoPago.FormaPago == FormaPago.Credito)
        {
            if (invoiceSale.DocType == "FACTURA")
            {
                var detallePagos = comprobanteDto.GetDetallePagos(invoiceSale.Id);
                if (detallePagos.Count() > 0) await _detallePagoSaleService.InsertManyAsync(detallePagos);
            }

            // registrar cargo.
            Receivable cargo = GenerarCargo(invoiceSale);
            await _receivableService.CreateAsync(cargo);
        }

        return invoiceSale;
    }

    /// <summary>
    /// Generar Cargo comprobante a crédito.
    /// </summary>
    /// <param name="invoiceSale">Comprobante</param>
    /// <returns>Cargo</returns>
    private Receivable GenerarCargo(InvoiceSale invoiceSale)
    {
        return new Receivable()
        {
            Type = "CARGO",
            ContactId = comprobanteDto.Cabecera.ContactId,
            ContactName = comprobanteDto.Cabecera.RznSocialUsuario,
            Remark = comprobanteDto.Cabecera.Remark,
            InvoiceSale = invoiceSale.Id,
            DocType = invoiceSale.DocType,
            Document = $"{invoiceSale.Serie}-{invoiceSale.Number}",
            FormaPago = "-",
            Cargo = invoiceSale.SumImpVenta,
            Status = "PENDIENTE",
            CreatedAt = DateTime.Now.ToString("yyyy-MM-dd"),
            EndDate = comprobanteDto.Cabecera.FecVencimiento,
            Year = DateTime.Now.ToString("yyyy"),
            Month = DateTime.Now.ToString("MM"),
        };
    }
}
