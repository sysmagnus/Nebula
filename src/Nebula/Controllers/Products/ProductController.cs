using Microsoft.AspNetCore.Mvc;
using Nebula.Modules.Products.Models;
using Nebula.Modules.Products;
using Nebula.Modules.Configurations;
using Nebula.Modules.Auth.Helpers;
using Nebula.Modules.Sales.Helpers;
using Nebula.Modules.Products.Dto;
using Nebula.Modules.Auth;

namespace Nebula.Controllers.Products;

[Route("api/[controller]")]
[ApiController]
public class ProductController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly IProductService _productService;
    private readonly IConfigurationService _configurationService;

    public ProductController(IConfiguration configuration,
        IProductService productService,
        IConfigurationService configurationService)
    {
        _configuration = configuration;
        _productService = productService;
        _configurationService = configurationService;
    }

    public class FormData : Product
    {
        public IFormFile? File { get; set; }
    }

    [HttpGet("Index"), UserAuthorize(Permission.ProductRead)]
    public async Task<IActionResult> Index([FromQuery] string? query)
    {
        var products = await _productService.GetListAsync(query);
        return Ok(products);
    }

    [HttpGet("Show/{id}"), UserAuthorize(Permission.ProductRead)]
    public async Task<IActionResult> Show(string id)
    {
        var product = await _productService.GetByIdAsync(id);
        return Ok(product);
    }

    [HttpGet("Select2"), UserAuthorize(Permission.ProductRead)]
    public async Task<IActionResult> Select2([FromQuery] string? term)
    {
        var products = await _productService.GetListAsync(term, 10);
        var data = new List<ProductSelect>();
        products.ForEach(item =>
        {
            var itemSelect2 = new ProductSelect
            {
                Id = item.Id,
                Description = item.Description,
                IgvSunat = item.IgvSunat,
                Icbper = item.Icbper,
                ValorUnitario = item.ValorUnitario,
                PrecioVentaUnitario = item.PrecioVentaUnitario,
                Barcode = item.Barcode,
                CodProductoSUNAT = item.CodProductoSUNAT,
                Type = item.Type,
                UndMedida = item.UndMedida,
                Category = item.Category,
                ControlStock = item.ControlStock,
                PathImage = item.PathImage,
                ProductType = item.ProductType,
                HasLotes = item.HasLotes,
                Text = $"{item.Description} | {Convert.ToDecimal(item.PrecioVentaUnitario):N2}"
            };
            data.Add(itemSelect2);
        });
        return Ok(new { Results = data });
    }

    [HttpPost("Create"), UserAuthorize(Permission.ProductCreate)]
    public async Task<IActionResult> Create([FromForm] FormData model)
    {
        if (model.File?.Length > 0)
        {
            var storagePath = _configuration.GetValue<string>("StoragePath");
            var dirPath = Path.Combine(storagePath, "uploads");
            var fileName = Guid.NewGuid() + model.File.FileName;
            var filePath = Path.Combine(dirPath, fileName);
            await using var stream = System.IO.File.Create(filePath);
            await model.File.CopyToAsync(stream);
            model.PathImage = fileName;
        }
        else
        {
            model.PathImage = "default.jpg";
        }

        var configuration = await _configurationService.GetAsync();
        decimal porcentajeIgv = configuration.PorcentajeIgv / 100 + 1;
        decimal porcentajeTributo = model.IgvSunat == TipoIGV.Gravado ? porcentajeIgv : 1;
        model.ValorUnitario = model.PrecioVentaUnitario / porcentajeTributo;

        Product product = new Product()
        {
            Description = model.Description.ToUpper(),
            IgvSunat = model.IgvSunat,
            Icbper = model.Icbper,
            ValorUnitario = model.ValorUnitario,
            PrecioVentaUnitario = model.PrecioVentaUnitario,
            Barcode = model.Barcode.ToUpper(),
            CodProductoSUNAT = model.CodProductoSUNAT.ToUpper(),
            Type = model.Type,
            UndMedida = model.UndMedida,
            Category = model.Category,
            ControlStock = model.ControlStock,
            PathImage = model.PathImage,
            ProductType = model.ProductType,
            HasLotes = false
        };
        await _productService.CreateAsync(product);

        return Ok(product);
    }

    [HttpPut("Update/{id}"), UserAuthorize(Permission.ProductEdit)]
    public async Task<IActionResult> Update(string id, [FromForm] FormData model)
    {
        if (id != model.Id) return BadRequest();
        var product = await _productService.GetByIdAsync(id);

        if (model.File?.Length > 0)
        {
            var storagePath = _configuration.GetValue<string>("StoragePath");
            var dirPath = Path.Combine(storagePath, "uploads");
            // borrar archivo antiguo si existe.
            var oldFile = Path.Combine(dirPath, product.PathImage ?? string.Empty);
            if (product.PathImage != null && !product.PathImage.Equals("default.jpg"))
                if (System.IO.File.Exists(oldFile))
                    System.IO.File.Delete(oldFile);
            // copiar nuevo archivo.
            var fileName = Guid.NewGuid() + model.File.FileName;
            var filePath = Path.Combine(dirPath, fileName);
            await using var stream = System.IO.File.Create(filePath);
            await model.File.CopyToAsync(stream);
            model.PathImage = fileName;
        }
        else
        {
            model.PathImage = product.PathImage;
        }

        var configuration = await _configurationService.GetAsync();
        decimal porcentajeIgv = configuration.PorcentajeIgv / 100 + 1;
        decimal porcentajeTributo = model.IgvSunat == TipoIGV.Gravado ? porcentajeIgv : 1;
        model.ValorUnitario = model.PrecioVentaUnitario / porcentajeTributo;
        // actualización de datos del modelo.
        product.Description = model.Description.ToUpper();
        product.IgvSunat = model.IgvSunat;
        product.Icbper = model.Icbper;
        product.ValorUnitario = model.ValorUnitario;
        product.PrecioVentaUnitario = model.PrecioVentaUnitario;
        product.Barcode = model.Barcode.ToUpper();
        product.CodProductoSUNAT = model.CodProductoSUNAT.ToUpper();
        product.Type = model.Type;
        product.UndMedida = model.UndMedida;
        product.Category = model.Category;
        product.ControlStock = model.ControlStock;
        product.PathImage = model.PathImage;
        product.ProductType = model.ProductType;
        product.HasLotes = product.HasLotes;
        await _productService.UpdateAsync(id, product);

        return Ok(product);
    }

    [HttpDelete("Delete/{id}"), UserAuthorize(Permission.ProductDelete)]
    public async Task<IActionResult> Delete(string id)
    {
        var product = await _productService.GetByIdAsync(id);
        // directorio principal.
        var storagePath = _configuration.GetValue<string>("StoragePath");
        var dirPath = Path.Combine(storagePath, "uploads");
        // borrar archivo si existe.
        if (product.PathImage != "default.jpg")
        {
            var file = Path.Combine(dirPath, product.PathImage);
            if (System.IO.File.Exists(file)) System.IO.File.Delete(file);
        }

        // borrar registro.
        await _productService.RemoveAsync(id);
        return Ok(product);
    }

}