using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nebula.Database.Helpers;
using Nebula.Database.Services.Common;
using Nebula.Database.Services.Facturador;
using Nebula.Database.Services.Sales;
using Nebula.Database.Dto.Common;
using Nebula.Database.Dto.Sales;
using Nebula.Database.Services;
using Nebula.Database.Models.Common;

namespace Nebula.Controllers.Sales;

[Authorize(Roles = AuthRoles.User)]
[Route("api/[controller]")]
[ApiController]
public class InvoiceSaleController : ControllerBase
{
    private readonly ConfigurationService _configurationService;
    private readonly CrudOperationService<InvoiceSerie> _invoiceSerieService;
    private readonly InvoiceSaleService _invoiceSaleService;
    private readonly InvoiceSaleDetailService _invoiceSaleDetailService;
    private readonly TributoSaleService _tributoSaleService;
    private readonly ComprobanteService _comprobanteService;
    private readonly FacturadorService _facturadorService;
    private readonly CreditNoteService _creditNoteService;

    public InvoiceSaleController(ConfigurationService configurationService, CrudOperationService<InvoiceSerie> invoiceSerieService,
        InvoiceSaleService invoiceSaleService, InvoiceSaleDetailService invoiceSaleDetailService, TributoSaleService tributoSaleService,
        ComprobanteService comprobanteService, FacturadorService facturadorService, CreditNoteService creditNoteService)
    {
        _configurationService = configurationService;
        _invoiceSerieService = invoiceSerieService;
        _invoiceSaleService = invoiceSaleService;
        _invoiceSaleDetailService = invoiceSaleDetailService;
        _tributoSaleService = tributoSaleService;
        _comprobanteService = comprobanteService;
        _facturadorService = facturadorService;
        _creditNoteService = creditNoteService;
    }

    [HttpGet("Index")]
    public async Task<IActionResult> Index([FromQuery] DateQuery model)
    {
        var invoiceSales = await _invoiceSaleService.GetListAsync(model);
        return Ok(invoiceSales);
    }

    [AllowAnonymous]
    [HttpGet("ReporteMensual")]
    public async Task<IActionResult> ReporteMensual([FromQuery] DateQuery dto)
    {
        var invoiceSeries = await _invoiceSerieService.GetAsync("Name", string.Empty);
        var invoiceSales = await _invoiceSaleService.GetListAsync(dto);
        var creditNotes = await _creditNoteService.GetListAsync(dto);
        string filePath = new ExportarReporteMensual(invoiceSeries, invoiceSales, creditNotes).GuardarCambios();
        FileStream stream = new FileStream(filePath, FileMode.Open);
        return new FileStreamResult(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
    }

    [HttpGet("Pendientes")]
    public async Task<IActionResult> Pendientes()
    {
        var invoiceSales = await _invoiceSaleService.GetInvoiceSalesPendingAsync();
        return Ok(invoiceSales);
    }

    [HttpPost("BusquedaAvanzada")]
    public async Task<IActionResult> BusquedaAvanzada([FromBody] BuscarComprobanteFormDto dto)
    {
        var invoiceSales = await _invoiceSaleService.BusquedaAvanzadaAsync(dto);
        return Ok(invoiceSales);
    }

    [HttpPost("Create")]
    public async Task<IActionResult> Create([FromBody] ComprobanteDto dto)
    {
        _comprobanteService.SetComprobanteDto(dto);
        var invoiceSale = await _comprobanteService.SaveChangesAsync();
        await _facturadorService.JsonInvoiceParser(invoiceSale.Id);
        return Ok(invoiceSale);
    }

    [HttpGet("Show/{id}")]
    public async Task<IActionResult> Show(string id)
    {
        var responseData = new ResponseInvoiceSale()
        {
            InvoiceSale = await _invoiceSaleService.GetAsync(id),
            InvoiceSaleDetails = await _invoiceSaleDetailService.GetListAsync(id),
            TributoSales = await _tributoSaleService.GetListAsync(id)
        };
        return Ok(responseData);
    }

    [HttpPatch("AnularComprobante/{id}")]
    public async Task<IActionResult> AnularComprobante(string id)
    {
        var responseAnularComprobante = new ResponseAnularComprobante();
        var creditNote = await _creditNoteService.AnulaciónDeLaOperación(id);
        responseAnularComprobante.CreditNote = creditNote;
        responseAnularComprobante.JsonFileCreated = await _facturadorService.CreateCreditNoteJsonFile(creditNote.Id);
        if (responseAnularComprobante.JsonFileCreated)
        {
            responseAnularComprobante.InvoiceSale = await _invoiceSaleService.AnularComprobante(creditNote.InvoiceSaleId);
        }
        return Ok(responseAnularComprobante);
    }

    [HttpPatch("SituacionFacturador/{id}")]
    public async Task<IActionResult> SituacionFacturador(string id, [FromBody] SituacionFacturadorDto dto)
    {
        var response = await _invoiceSaleService.SetSituacionFacturador(id, dto);
        return Ok(response);
    }

    [HttpDelete("Delete/{id}"), Authorize(Roles = AuthRoles.Admin)]
    public async Task<IActionResult> Delete(string id)
    {
        var invoiceSale = await _invoiceSaleService.GetAsync(id);
        await _invoiceSaleService.RemoveAsync(invoiceSale.Id);
        await _invoiceSaleDetailService.RemoveAsync(invoiceSale.Id);
        await _tributoSaleService.RemoveAsync(invoiceSale.Id);
        return Ok(new { Ok = true, Data = invoiceSale, Msg = "El comprobante de venta ha sido borrado!" });
    }

    [AllowAnonymous]
    [HttpGet("GetPdf/{id}")]
    public async Task<IActionResult> GetPdf(string id)
    {
        var configuration = await _configurationService.GetAsync();
        var comprobante = await _invoiceSaleService.GetAsync(id);
        string tipDocu = string.Empty;
        if (comprobante.DocType.Equals("FACTURA")) tipDocu = "01";
        if (comprobante.DocType.Equals("BOLETA")) tipDocu = "03";
        // 20520485750-03-B001-00000015
        string nomArch = $"{configuration.Ruc}-{tipDocu}-{comprobante.Serie}-{comprobante.Number}.pdf";
        string sfs = Path.Combine(configuration.FileSunat, "sfs");
        string pdfFolder = Path.Combine(sfs, "REPO");
        string pathPdf = Path.Combine(pdfFolder, nomArch);
        FileStream stream = new FileStream(pathPdf, FileMode.Open);
        return new FileStreamResult(stream, "application/pdf");
    }
}
