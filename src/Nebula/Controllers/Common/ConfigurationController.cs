using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nebula.Data.Models.Common;
using Nebula.Data.Services.Common;

namespace Nebula.Controllers.Common;

[Route("api/[controller]")]
[ApiController]
public class ConfigurationController : ControllerBase
{
    private readonly ConfigurationService _configurationService;

    public ConfigurationController(ConfigurationService configurationService) =>
        _configurationService = configurationService;

    [HttpGet("Show"), Authorize]
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

    [HttpPut("Update"), Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update([FromBody] Configuration model)
    {
        var configuration = await _configurationService.GetAsync();
        if (configuration is null) return NotFound();

        model.Id = configuration.Id;
        await _configurationService.UpdateAsync(model);

        return Ok(new
        {
            Ok = true,
            Data = configuration,
            Msg = $"La configuración {configuration.Ruc}, ha sido actualizado!"
        });
    }
}
