using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nebula.Modules.Cashier;
using Nebula.Modules.Cashier.Models;
using Nebula.Modules.Configurations.Warehouses;
using Nebula.Modules.Auth.Helpers;
using Nebula.Modules.Cashier.Helpers;
using Nebula.Modules.Cashier.Dto;
using Nebula.Common.Dto;
using Nebula.Modules.Configurations.Subscriptions;

namespace Nebula.Controllers.Cashier;

[Authorize(Roles = AuthRoles.User)]
[Route("api/[controller]")]
[ApiController]
public class CajaDiariaController : ControllerBase
{
    private readonly ISubscriptionService _subscriptionService;
    private readonly ICajaDiariaService _cajaDiariaService;
    private readonly IInvoiceSerieService _invoiceSerieService;
    private readonly ICashierDetailService _cashierDetailService;

    public CajaDiariaController(ISubscriptionService subscriptionService,
        ICajaDiariaService cajaDiariaService,
        IInvoiceSerieService invoiceSerieService,
        ICashierDetailService cashierDetailService)
    {
        _subscriptionService = subscriptionService;
        _cajaDiariaService = cajaDiariaService;
        _invoiceSerieService = invoiceSerieService;
        _cashierDetailService = cashierDetailService;
    }

    [HttpGet("Index")]
    public async Task<IActionResult> Index([FromQuery] DateQuery model)
    {
        var cajaDiarias = await _cajaDiariaService.GetListAsync(model);
        return Ok(cajaDiarias);
    }

    [HttpGet("Show/{id}")]
    public async Task<IActionResult> Show(string id)
    {
        var cajaDiaria = await _cajaDiariaService.GetByIdAsync(id);
        return Ok(cajaDiaria);
    }

    [HttpPost("Create")]
    public async Task<IActionResult> Create([FromBody] AperturaCaja model)
    {
        var invoiceSerie = await _invoiceSerieService.GetByIdAsync(model.InvoiceSerie);
        var cajaDiaria = new CajaDiaria()
        {
            InvoiceSerie = invoiceSerie.Id,
            Terminal = invoiceSerie.Name,
            Status = "ABIERTO",
            TotalApertura = model.Total,
            TotalContabilizado = 0.0M,
            TotalCierre = 0.0M,
            Turno = model.Turno
        };
        await _cajaDiariaService.CreateAsync(cajaDiaria);

        // registrar apertura de caja.
        var detalleCaja = new CashierDetail()
        {
            CajaDiaria = cajaDiaria.Id,
            Remark = "APERTURA DE CAJA",
            TypeOperation = TypeOperationCaja.AperturaDeCaja,
            FormaPago = FormaPago.Contado,
            Amount = model.Total
        };
        await _cashierDetailService.CreateAsync(detalleCaja);

        return Ok(new
        {
            Ok = true,
            Data = cajaDiaria,
            Msg = "La apertura de caja ha sido registrado!"
        });
    }

    [HttpPut("Update/{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] CerrarCaja model)
    {
        var license = await _subscriptionService.ValidarAcceso();
        if (!license.Ok) return BadRequest(new { Ok = false, Msg = "Error, Verificar suscripción!" });
        var cajaDiaria = await _cajaDiariaService.GetByIdAsync(id);
        cajaDiaria.TotalContabilizado = model.TotalContabilizado;
        cajaDiaria.TotalCierre = model.TotalCierre;
        cajaDiaria.Status = "CERRADO";
        await _cajaDiariaService.UpdateAsync(id, cajaDiaria);
        return Ok(new
        {
            Ok = true,
            Data = cajaDiaria,
            Msg = "El cierre de caja ha sido registrado!"
        });
    }

    [HttpDelete("Delete/{id}"), Authorize(Roles = AuthRoles.Admin)]
    public async Task<IActionResult> Delete(string id)
    {
        var license = await _subscriptionService.ValidarAcceso();
        if (!license.Ok) return BadRequest(new { Ok = false, Msg = "Error, Verificar suscripción!" });
        var cajaDiaria = await _cajaDiariaService.GetByIdAsync(id);
        await _cajaDiariaService.RemoveAsync(cajaDiaria.Id);
        return Ok(new { Ok = true, Data = cajaDiaria, Msg = "La caja diaria ha sido borrado!" });
    }

    [HttpGet("CajasAbiertas")]
    public async Task<IActionResult> CajasAbiertas()
    {
        return Ok(await _cajaDiariaService.GetCajasAbiertasAsync());
    }
}
