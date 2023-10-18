using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nebula.Common.Dto;
using Nebula.Common.Helpers;
using Nebula.Modules.Account;
using Nebula.Modules.Account.Models;
using Nebula.Modules.Auth.Helpers;
using Nebula.Modules.Auth;
using Nebula.Modules.InvoiceHub;
using Nebula.Modules.InvoiceHub.Helpers;
using Nebula.Modules.Sales;
using Nebula.Modules.Sales.Comprobantes;
using Nebula.Modules.Sales.Comprobantes.Dto;
using Nebula.Modules.Sales.Helpers;
using Nebula.Modules.Sales.Invoices;
using Nebula.Modules.Sales.Invoices.Dto;
using Nebula.Modules.Sales.Notes;
using Nebula.Common;
using Nebula.Modules.Inventory.Stock;

namespace Nebula.Controllers.Sales;

[Authorize]
[CustomerAuthorize(UserRole = CompanyRoles.User)]
[Route("api/sales/{companyId}/[controller]")]
[ApiController]
public class InvoiceSaleController : ControllerBase
{
    private readonly ICacheAuthService _cacheAuthService;
    private readonly IInvoiceSerieService _invoiceSerieService;
    private readonly IInvoiceSaleService _invoiceSaleService;
    private readonly IInvoiceSaleDetailService _invoiceSaleDetailService;
    private readonly IComprobanteService _comprobanteService;
    private readonly ICreditNoteService _creditNoteService;
    private readonly IConsultarValidezComprobanteService _consultarValidezComprobanteService;
    private readonly IInvoiceHubService _invoiceHubService;
    private readonly ICreditNoteHubService _creditNoteHubService;
    private readonly IValidateStockService _validateStockService;

    public InvoiceSaleController(
        ICacheAuthService cacheAuthService,
        IInvoiceSerieService invoiceSerieService,
        IInvoiceSaleService invoiceSaleService,
        IInvoiceSaleDetailService invoiceSaleDetailService,
        IComprobanteService comprobanteService,
        ICreditNoteService creditNoteService,
        IConsultarValidezComprobanteService consultarValidezComprobanteService,
        IInvoiceHubService invoiceHubService,
        ICreditNoteHubService creditNoteHubService,
        IValidateStockService validateStockService)
    {
        _cacheAuthService = cacheAuthService;
        _invoiceSerieService = invoiceSerieService;
        _invoiceSaleService = invoiceSaleService;
        _invoiceSaleDetailService = invoiceSaleDetailService;
        _comprobanteService = comprobanteService;
        _creditNoteService = creditNoteService;
        _consultarValidezComprobanteService = consultarValidezComprobanteService;
        _invoiceHubService = invoiceHubService;
        _creditNoteHubService = creditNoteHubService;
        _validateStockService = validateStockService;
    }

    [HttpGet]
    public async Task<IActionResult> Index(string companyId, [FromQuery] DateQuery model)
    {
        var invoiceSales = await _invoiceSaleService.GetListAsync(companyId, model);
        return Ok(invoiceSales);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Show(string companyId, string id)
    {
        var responseInvoiceSale = await _invoiceSaleService.GetInvoiceSaleAsync(companyId, id);
        return Ok(responseInvoiceSale);
    }

    [HttpPost]
    public async Task<IActionResult> Create(string companyId, [FromBody] ComprobanteDto dto)
    {
        var company = await _cacheAuthService.GetCompanyByIdAsync(companyId);
        var comprobante = await _comprobanteService.SaveChangesAsync(company, dto);
        await _validateStockService.ValidarInvoiceSale(comprobante);
        var invoiceRequest = InvoiceMapper.MapToInvoiceRequestHub(company.Ruc, comprobante);
        var billingResponse = await _invoiceHubService.SendInvoiceAsync(companyId, invoiceRequest);
        comprobante.InvoiceSale.BillingResponse = billingResponse;
        await _invoiceSaleService.UpdateAsync(comprobante.InvoiceSale.Id, comprobante.InvoiceSale);
        return Ok(new { Data = billingResponse, InvoiceId = comprobante.InvoiceSale.Id });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string companyId, string id)
    {
        var invoiceSale = await _invoiceSaleService.GetByIdAsync(companyId, id);
        await _invoiceSaleService.RemoveAsync(companyId, invoiceSale.Id);
        await _invoiceSaleDetailService.RemoveAsync(companyId, invoiceSale.Id);
        return Ok(invoiceSale);
    }

    [AllowAnonymous]
    [HttpGet("ExcelRegistroVentasF141")]
    public async Task<IActionResult> ExcelRegistroVentasF141(string companyId, [FromQuery] DateQuery dto)
    {
        string[] fieldNames = new string[] { "Name" };
        ParamExcelRegistroVentasF141 param = new ParamExcelRegistroVentasF141
        {
            InvoiceSeries = await _invoiceSerieService.GetFilteredAsync(companyId, fieldNames, string.Empty),
            InvoiceSales = await _invoiceSaleService.GetListAsync(companyId, dto),
            CreditNotes = await _creditNoteService.GetListAsync(companyId, dto),
        };
        // Obtener lista de comprobantes de notas de crédito.
        List<string> seriesComprobante = new List<string>();
        List<string> númerosComprobante = new List<string>();
        param.CreditNotes.ForEach(item =>
        {
            seriesComprobante.Add(item.NumDocfectado.Split("-")[0].Trim());
            númerosComprobante.Add(item.NumDocfectado.Split("-")[1].Trim());
        });
        seriesComprobante = seriesComprobante.Distinct().ToList();
        númerosComprobante = númerosComprobante.Distinct().ToList();
        var comprobantesDeNotas = await _invoiceSaleService.GetInvoicesByNumDocs(companyId, seriesComprobante, númerosComprobante);
        param.ComprobantesDeNotas = comprobantesDeNotas;
        // generar archivo excel y enviar como respuesta de solicitud.
        string filePath = new ExcelRegistroVentasF141(param).CrearArchivo();
        FileStream stream = new FileStream(filePath, FileMode.Open);
        return new FileStreamResult(stream, ContentTypeFormat.Excel);
    }

    [HttpGet("Pendientes")]
    public async Task<IActionResult> Pendientes(string companyId)
    {
        var invoiceSales = await _invoiceSaleService.GetInvoiceSalesPendingAsync(companyId);
        return Ok(invoiceSales);
    }

    [HttpPost("BusquedaAvanzada")]
    public async Task<IActionResult> BusquedaAvanzada(string companyId, [FromBody] BuscarComprobanteFormDto dto)
    {
        var invoiceSales = await _invoiceSaleService.BusquedaAvanzadaAsync(companyId, dto);
        return Ok(invoiceSales);
    }

    [HttpPatch("AnularComprobante/{id}")]
    public async Task<IActionResult> AnularComprobante(string companyId, string id)
    {
        var company = await _cacheAuthService.GetCompanyByIdAsync(companyId);
        var invoiceCancellationResponse = await _creditNoteService.InvoiceCancellation(companyId, id);
        var creditNoteRequest = CreditNoteMapper.MapToCreditNoteRequestHub(company.Ruc, invoiceCancellationResponse);
        var billingResponse = await _creditNoteHubService.SendCreditNoteAsync(companyId, creditNoteRequest);
        var creditNote = invoiceCancellationResponse.CreditNote;
        creditNote.BillingResponse = billingResponse;
        await _creditNoteService.UpdateAsync(creditNote.Id, creditNote);
        var invoice = invoiceCancellationResponse.InvoiceSale;
        invoice.Anulada = billingResponse.Success;
        await _invoiceSaleService.UpdateAsync(invoice.Id, invoice);
        return Ok(new { billingResponse, creditNote });
    }

    [HttpGet("Ticket/{id}")]
    public async Task<IActionResult> Ticket(string companyId, string id)
    {
        var responseInvoice = await _invoiceSaleService.GetInvoiceSaleAsync(companyId, id);
        var ticket = new TicketDto()
        {
            Company = await _cacheAuthService.GetCompanyByIdAsync(companyId),
            InvoiceSale = responseInvoice.InvoiceSale,
            InvoiceSaleDetails = responseInvoice.InvoiceSaleDetails,
        };
        return Ok(ticket);
    }

    [AllowAnonymous]
    [HttpGet("ConsultarValidez")]
    public async Task<IActionResult> ConsultarValidez(string companyId, [FromQuery] QueryConsultarValidezComprobante query)
    {
        string pathArchivoZip = await _consultarValidezComprobanteService.CrearArchivosDeValidación(new Company(), query);
        FileStream stream = new FileStream(pathArchivoZip, FileMode.Open);
        return new FileStreamResult(stream, "application/zip");
    }
}
