using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nebula.Modules.Inventory.Stock;
using Nebula.Modules.Sales;
using Nebula.Modules.Facturador;
using Nebula.Modules.Configurations;
using Nebula.Modules.Auth.Helpers;
using Nebula.Common.Helpers;
using Nebula.Modules.Facturador.Helpers;
using Nebula.Modules.Sales.Dto;
using Nebula.Common.Dto;
using Nebula.Modules.Configurations.Subscriptions;
using Nebula.Modules.Configurations.Warehouses;
using Nebula.Modules.Sales.Helpers;

namespace Nebula.Controllers.Sales;

[Authorize(Roles = AuthRoles.User)]
[Route("api/[controller]")]
[ApiController]
public class InvoiceSaleController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly ISubscriptionService _subscriptionService;
    private readonly IConfigurationService _configurationService;
    private readonly IInvoiceSerieService _invoiceSerieService;
    private readonly IInvoiceSaleService _invoiceSaleService;
    private readonly IInvoiceSaleDetailService _invoiceSaleDetailService;
    private readonly ITributoSaleService _tributoSaleService;
    private readonly ITributoCreditNoteService _tributoCreditNoteService;
    private readonly IComprobanteService _comprobanteService;
    private readonly IFacturadorService _facturadorService;
    private readonly ICreditNoteService _creditNoteService;
    private readonly IValidateStockService _validateStockService;
    private readonly IConsultarValidezComprobanteService _consultarValidezComprobanteService;

    public InvoiceSaleController(ISubscriptionService subscriptionService,
        IConfigurationService configurationService,
        IInvoiceSerieService invoiceSerieService,
        IInvoiceSaleService invoiceSaleService,
        IInvoiceSaleDetailService invoiceSaleDetailService,
        ITributoSaleService tributoSaleService,
        IComprobanteService comprobanteService,
        IFacturadorService facturadorService,
        ICreditNoteService creditNoteService,
        IValidateStockService validateStockService,
        IConfiguration configuration,
        IConsultarValidezComprobanteService consultarValidezComprobanteService,
        ITributoCreditNoteService tributoCreditNoteService)
    {
        _subscriptionService = subscriptionService;
        _configuration = configuration;
        _configurationService = configurationService;
        _invoiceSerieService = invoiceSerieService;
        _invoiceSaleService = invoiceSaleService;
        _invoiceSaleDetailService = invoiceSaleDetailService;
        _tributoSaleService = tributoSaleService;
        _comprobanteService = comprobanteService;
        _facturadorService = facturadorService;
        _creditNoteService = creditNoteService;
        _validateStockService = validateStockService;
        _consultarValidezComprobanteService = consultarValidezComprobanteService;
        _tributoCreditNoteService = tributoCreditNoteService;
    }

    [HttpGet("Index")]
    public async Task<IActionResult> Index([FromQuery] DateQuery model)
    {
        var invoiceSales = await _invoiceSaleService.GetListAsync(model);
        return Ok(invoiceSales);
    }

    [AllowAnonymous]
    [HttpGet("ExcelRegistroVentasF141")]
    public async Task<IActionResult> ExcelRegistroVentasF141([FromQuery] DateQuery dto)
    {
        ParamExcelRegistroVentasF141 param = new ParamExcelRegistroVentasF141
        {
            InvoiceSeries = await _invoiceSerieService.GetAsync("Name", string.Empty),
            InvoiceSales = await _invoiceSaleService.GetListAsync(dto),
            CreditNotes = await _creditNoteService.GetListAsync(dto),
            TributoSales = await _tributoSaleService.GetTributosMensual(dto),
            TributoCreditNotes = await _tributoCreditNoteService.GetTributosMensual(dto)
        };
        // Obtener lista de comprobantes de notas de crédito.
        List<string> seriesComprobante = new List<string>();
        List<string> númerosComprobante = new List<string>();
        param.CreditNotes.ForEach(item =>
        {
            seriesComprobante.Add(item.NumDocAfectado.Split("-")[0].Trim());
            númerosComprobante.Add(item.NumDocAfectado.Split("-")[1].Trim());
        });
        seriesComprobante = seriesComprobante.Distinct().ToList();
        númerosComprobante = númerosComprobante.Distinct().ToList();
        var comprobantesDeNotas = await _invoiceSaleService.GetInvoicesByNumDocs(seriesComprobante, númerosComprobante);
        param.ComprobantesDeNotas = comprobantesDeNotas;
        // generar archivo excel y enviar como respuesta de solicitud.
        string filePath = new ExcelRegistroVentasF141(param).CrearArchivo();
        FileStream stream = new FileStream(filePath, FileMode.Open);
        return new FileStreamResult(stream, ContentTypeFormat.Excel);
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
        var license = await _subscriptionService.ValidarAcceso();
        if (!license.Ok) return BadRequest(new { Ok = false, Msg = "Error, Verificar suscripción!" });
        _comprobanteService.SetComprobanteDto(dto);
        var invoiceSale = await _comprobanteService.SaveChangesAsync();
        await _facturadorService.JsonInvoiceParser(invoiceSale.Id);
        await _validateStockService.ValidarInvoiceSale(invoiceSale.Id);
        return Ok(invoiceSale);
    }

    [HttpGet("Show/{id}")]
    public async Task<IActionResult> Show(string id)
    {
        var responseInvoiceSale = await _invoiceSaleService.GetInvoiceSaleAsync(id);
        return Ok(responseInvoiceSale);
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
            responseAnularComprobante.InvoiceSale =
                await _invoiceSaleService.AnularComprobante(creditNote.InvoiceSaleId);
        }

        return Ok(responseAnularComprobante);
    }

    [HttpPatch("SituacionFacturador/{id}")]
    public async Task<IActionResult> SituacionFacturador(string id, [FromBody] SituacionFacturadorDto dto)
    {
        var response = await _invoiceSaleService.SetSituacionFacturador(id, dto);
        return Ok(response);
    }

    [HttpPatch("SaveInControlFolder/{invoiceSaleId}")]
    public async Task<IActionResult> SituacionFacturador(string invoiceSaleId)
    {
        try
        {
            var invoiceSale = await _facturadorService.SaveInvoiceInControlFolder(invoiceSaleId);
            return Ok(invoiceSale);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("Delete/{id}"), Authorize(Roles = AuthRoles.Admin)]
    public async Task<IActionResult> Delete(string id)
    {
        var invoiceSale = await _invoiceSaleService.GetByIdAsync(id);
        await _invoiceSaleService.RemoveAsync(invoiceSale.Id);
        await _invoiceSaleDetailService.RemoveAsync(invoiceSale.Id);
        await _tributoSaleService.RemoveAsync(invoiceSale.Id);
        return Ok(new { Ok = true, Data = invoiceSale, Msg = "El comprobante de venta ha sido borrado!" });
    }

    [HttpDelete("BorrarArchivosAntiguos/{invoiceSaleId}")]
    public async Task<IActionResult> BorrarArchivos(string invoiceSaleId)
    {
        var invoiceSale = await _facturadorService.BorrarArchivosAntiguosInvoice(invoiceSaleId);
        await _facturadorService.JsonInvoiceParser(invoiceSale.Id);
        return Ok(invoiceSale);
    }

    [AllowAnonymous]
    [HttpGet("GetPdf/{id}")]
    [Obsolete("Este método está obsoleto.")]
    public async Task<IActionResult> GetPdf(string id)
    {
        var configuration = await _configurationService.GetAsync();
        var comprobante = await _invoiceSaleService.GetByIdAsync(id);
        string tipDocu = string.Empty;
        if (comprobante.DocType.Equals("FACTURA")) tipDocu = "01";
        if (comprobante.DocType.Equals("BOLETA")) tipDocu = "03";
        // 20520485750-03-B001-00000015
        string nomArch = $"{configuration.Ruc}-{tipDocu}-{comprobante.Serie}-{comprobante.Number}.pdf";
        string pathPdf = string.Empty;
        var storagePath = _configuration.GetValue<string>("StoragePath");
        if (comprobante.DocumentPath == DocumentPathType.SFS)
        {
            string? sunatArchivos = _configuration.GetValue<string>("sunatArchivos");
            if (sunatArchivos is null) sunatArchivos = string.Empty;
            string carpetaArchivoSunat = Path.Combine(sunatArchivos, "sfs");
            pathPdf = Path.Combine(carpetaArchivoSunat, "REPO", nomArch);
        }

        if (comprobante.DocumentPath == DocumentPathType.CONTROL)
        {
            if (storagePath is null) storagePath = string.Empty;
            string carpetaArchivoSunat = Path.Combine(storagePath, "sunat");
            string carpetaRepo = Path.Combine(carpetaArchivoSunat, "REPO", comprobante.Year, comprobante.Month);
            pathPdf = Path.Combine(carpetaRepo, nomArch);
        }

        FileStream stream = new FileStream(pathPdf, FileMode.Open);
        return new FileStreamResult(stream, "application/pdf");
    }

    [HttpGet("Ticket/{id}")]
    public async Task<IActionResult> Ticket(string id)
    {
        var ticket = await _invoiceSaleService.GetTicketDto(id);
        return Ok(ticket);
    }

    [AllowAnonymous]
    [HttpGet("ConsultarValidez")]
    public async Task<IActionResult> ConsultarValidez([FromQuery] QueryConsultarValidezComprobante query)
    {
        string pathArchivoZip = await _consultarValidezComprobanteService.CrearArchivosDeValidación(query);
        FileStream stream = new FileStream(pathArchivoZip, FileMode.Open);
        return new FileStreamResult(stream, "application/zip");
    }
}
