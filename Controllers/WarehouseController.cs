using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Nebula.Data;
using Nebula.Data.Models;

namespace Nebula.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WarehouseController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public WarehouseController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("Index")]
        public async Task<IActionResult> Index([FromQuery] string query)
        {
            var result = from m in _context.Warehouses select m;
            if (!string.IsNullOrWhiteSpace(query))
                result = result.Where(m => m.Name.ToLower().Contains(query.ToLower()));
            var responseData = await result.AsNoTracking().ToListAsync();
            return Ok(responseData);
        }

        [HttpGet("Show/{id}")]
        public async Task<IActionResult> Show(string id)
        {
            var result = await _context.Warehouses.AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id.ToString().Equals(id));
            return Ok(result);
        }

        [HttpPost("Store")]
        public async Task<IActionResult> Store([FromBody] Warehouse model)
        {
            _context.Warehouses.Add(model);
            await _context.SaveChangesAsync();
            return Ok(new
            {
                Ok = true, Data = model,
                Msg = $"{model.Name} ha sido registrado!"
            });
        }

        [HttpPut("Update/{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] Warehouse model)
        {
            if (id != model.Id.ToString()) return BadRequest();
            _context.Warehouses.Update(model);
            await _context.SaveChangesAsync();
            return Ok(new
            {
                Ok = true, Data = model,
                Msg = $"{model.Name} ha sido actualizado!"
            });
        }

        [HttpDelete("Destroy/{id}")]
        public async Task<IActionResult> Destroy(string id)
        {
            var result = await _context.Warehouses.FirstOrDefaultAsync(m => m.Id.ToString().Equals(id));
            if (result == null) return BadRequest();
            _context.Warehouses.Remove(result);
            await _context.SaveChangesAsync();
            return Ok(new {Ok = true, Data = result, Msg = "El almacén ha sido borrado!"});
        }
    }
}
