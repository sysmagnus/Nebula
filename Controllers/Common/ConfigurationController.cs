using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nebula.Database.Helpers;
using Nebula.Database.Models.Common;
using Nebula.Database.Services.Common;

namespace Nebula.Controllers.Common;

[Authorize(Roles = AuthRoles.User)]
[Route("api/[controller]")]
[ApiController]
public class ConfigurationController : ControllerBase
{
    private readonly ConfigurationService _configurationService;

    public ConfigurationController(ConfigurationService configurationService) =>
        _configurationService = configurationService;

    [HttpGet("Show")]
    public async Task<IActionResult> Show()
    {
        var configuration = await _configurationService.GetAsync();
        if (configuration is null)
        {
            await _configurationService.CreateAsync();
            return Ok(await _configurationService.GetAsync());
        }

        return Ok(configuration);
    }

    [HttpPut("Update"), Authorize(Roles = AuthRoles.Admin)]
    public async Task<IActionResult> Update([FromBody] Configuration model)
    {
        var configuration = await _configurationService.GetAsync();
        if (configuration is null) return NotFound();

        model.Id = configuration.Id;
        await _configurationService.UpdateAsync(model);

        return Ok(configuration);
    }

    [HttpGet("SincronizarPago")]
    public async Task<IActionResult> SincronizarPago()
    {
        var response = await _configurationService.SincronizarPago();
        return Ok(response);
    }

    [HttpGet("ValidarAcceso")]
    public async Task<IActionResult> ValidarAcceso()
    {
        var licenseDto = await _configurationService.ValidarAcceso();
        return Ok(licenseDto);
    }

    [HttpPatch("UpdateKey/{subscriptionId}")]
    public async Task<IActionResult> UpdateKey(string subscriptionId)
    {
        var configuration = await _configurationService.UpdateKey(subscriptionId);
        return Ok(configuration);
    }

}
