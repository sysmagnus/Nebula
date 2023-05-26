using Nebula.Modules.Inventory.Models;
using Nebula.Modules.Inventory.Stock.Validations;

namespace Nebula.Modules.Inventory.Stock;

public interface IValidateStockService
{
    Task<InventoryNotas> ValidarNotas(string id);
    Task<Transferencia> ValidarTransferencia(string id);
    Task<AjusteInventario> ValidarAjusteInventario(string id);
    Task<Material> ValidarMaterial(string id);
    Task ValidarInvoiceSale(string id);
}

public class ValidateStockService : IValidateStockService
{
    private readonly IInventoryNotasStockValidator _inventoryNotasStockValidator;
    private readonly IInventoryTransferenciaStockValidator _inventoryTransferenciaStockValidator;
    private readonly IAjusteInventarioStockValidator _ajusteInventarioStockValidator;
    private readonly IInventoryMaterialStockValidator _inventoryMaterialStockValidator;
    private readonly IInvoiceSaleStockValidator _invoiceSaleStockValidator;

    public ValidateStockService(
        IInventoryNotasStockValidator inventoryNotasStockValidator,
        IInventoryTransferenciaStockValidator inventoryTransferenciaStockValidator,
        IAjusteInventarioStockValidator ajusteInventarioStockValidator,
        IInventoryMaterialStockValidator inventoryMaterialStockValidator,
        IInvoiceSaleStockValidator invoiceSaleStockValidator)
    {
        _inventoryNotasStockValidator = inventoryNotasStockValidator;
        _inventoryTransferenciaStockValidator = inventoryTransferenciaStockValidator;
        _ajusteInventarioStockValidator = ajusteInventarioStockValidator;
        _inventoryMaterialStockValidator = inventoryMaterialStockValidator;
        _invoiceSaleStockValidator = invoiceSaleStockValidator;
    }

    public async Task<InventoryNotas> ValidarNotas(string id)
    {
        return await _inventoryNotasStockValidator.ValidarNotas(id);
    }

    public async Task<Transferencia> ValidarTransferencia(string id)
    {
        return await _inventoryTransferenciaStockValidator.ValidarTransferencia(id);
    }

    public async Task<AjusteInventario> ValidarAjusteInventario(string id)
    {
        return await _ajusteInventarioStockValidator.ValidarAjusteInventario(id);
    }

    public async Task<Material> ValidarMaterial(string id)
    {
        return await _inventoryMaterialStockValidator.ValidarMaterial(id);
    }

    public async Task ValidarInvoiceSale(string id)
    {
        await _invoiceSaleStockValidator.ValidarInvoiceSale(id);
    }
}