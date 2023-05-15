using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Nebula.Database.Helpers;
using Nebula.Database.Models.Common;
using Nebula.Database.Models.Inventory;
using Nebula.Database.Dto.Inventory;
using Nebula.Database;
using Nebula.Database.Services;
using Nebula.Plugins.Inventory.Models;
using Nebula.Database.Services.Common;
using Nebula.Plugins.Inventory.Stock.Dto;

namespace Nebula.Plugins.Inventory.Stock;

public class ProductStockService : CrudOperationService<ProductStock>
{
    private readonly CrudOperationService<InvoiceSerie> _invoiceSerieService;
    private readonly WarehouseService _warehouseService;

    public ProductStockService(IOptions<DatabaseSettings> options,
        WarehouseService warehouseService,
        CrudOperationService<InvoiceSerie> invoiceSerieService) : base(options)
    {
        _warehouseService = warehouseService;
        _invoiceSerieService = invoiceSerieService;
    }

    public async Task<List<ProductStock>> CreateManyAsync(List<ProductStock> obj)
    {
        await _collection.InsertManyAsync(obj);
        return obj;
    }

    /// <summary>
    /// Elimina varios registros de productos de la base de datos basados en los identificadores de almacén y producto especificados.
    /// </summary>
    /// <param name="warehouseId">El identificador del almacén</param>
    /// <param name="productId">El identificador del producto</param>
    /// <returns>El resultado de la operación de eliminación</returns>
    public async Task<DeleteResult> RemoveManyAsync(string warehouseId, string productId)
    {
        var filter = Builders<ProductStock>.Filter;
        var dbQuery = filter.And(filter.Eq(x => x.WarehouseId, warehouseId),
            filter.Eq(x => x.ProductId, productId));
        return await _collection.DeleteManyAsync(dbQuery);
    }

    /// <summary>
    /// Cambia la cantidad de existencia de un producto en un almacén especificado.
    /// </summary>
    /// <param name="requestParams">Los parámetros de solicitud para el cambio de cantidad de stock</param>
    /// <returns>El objeto ProductStock actualizado</returns>
    public async Task<ProductStock> ChangeQuantity(ChangeQuantityStockRequestParams requestParams)
    {
        await RemoveManyAsync(requestParams.WarehouseId, requestParams.ProductId);
        var productStock = new ProductStock()
        {
            Id = string.Empty,
            WarehouseId = requestParams.WarehouseId,
            ProductId = requestParams.ProductId,
            ProductLoteId = requestParams.ProductLoteId,
            Type = InventoryType.ENTRADA,
            Quantity = requestParams.Quantity,
        };
        productStock = await CreateAsync(productStock);
        return productStock;
    }

    // ----------------------------------------------------------------------------------

    public async Task<long> GetStockItemCajaAsync(string invoiceSerieId, string productId)
    {
        var invoiceSerie = await _invoiceSerieService.GetByIdAsync(invoiceSerieId);
        var productStocks = await GetProductStockByWarehouseIdAndProductIdAsync(invoiceSerie.WarehouseId, productId);
        var totalEntrada = productStocks.Where(x => x.Type == InventoryType.ENTRADA).Sum(x => x.Quantity);
        var totalSalida = productStocks.Where(x => x.Type == InventoryType.SALIDA).Sum(x => x.Quantity);
        return totalEntrada - totalSalida;
    }

    public async Task<long> GetStockItemComprobanteAsync(string warehouseId, string productId)
    {
        var productStocks = await GetProductStockByWarehouseIdAndProductIdAsync(warehouseId, productId);
        var totalEntrada = productStocks.Where(x => x.Type == InventoryType.ENTRADA).Sum(x => x.Quantity);
        var totalSalida = productStocks.Where(x => x.Type == InventoryType.SALIDA).Sum(x => x.Quantity);
        return totalEntrada - totalSalida;
    }

    [Obsolete]
    public async Task<List<ProductStockReport>> GetProductStockReportAsync(string id)
    {
        var warehouses = await _warehouseService.GetAsync("Name", string.Empty);
        var filter = Builders<ProductStock>.Filter;
        var warehouseArrId = new List<string>();
        warehouses.ForEach(item => warehouseArrId.Add(item.Id));
        var productStocks = await GetProductStockByProductIdAsync(id, warehouseArrId);
        var productStockReports = new List<ProductStockReport>();
        warehouses.ForEach(item =>
        {
            var products = productStocks.Where(x => x.WarehouseId == item.Id).ToList();
            var entrada = products.Where(x => x.Type == InventoryType.ENTRADA).Sum(x => x.Quantity);
            var salida = products.Where(x => x.Type == InventoryType.SALIDA).Sum(x => x.Quantity);
            productStockReports.Add(new ProductStockReport
            {
                WarehouseId = item.Id,
                WarehouseName = item.Name,
                Quantity = entrada - salida,
            });
        });
        return productStockReports;
    }

    public async Task<List<TransferenciaDetail>> CalcularCantidadExistenteRestanteTransferenciaAsync(
        List<TransferenciaDetail> transferenciaDetails, string warehouseId)
    {
        var productArrId = new List<string>();
        transferenciaDetails.ForEach(item => productArrId.Add(item.ProductId));
        var productStocks = await GetProductStockByWarehouseIdAsync(warehouseId, productArrId);
        transferenciaDetails.ForEach(item =>
        {
            item.Id = string.Empty;
            var products = productStocks.Where(x => x.ProductId == item.ProductId).ToList();
            var entrada = products.Where(x => x.Type == InventoryType.ENTRADA).Sum(x => x.Quantity);
            var salida = products.Where(x => x.Type == InventoryType.SALIDA).Sum(x => x.Quantity);
            item.CantExistente = entrada - salida;
            item.CantRestante = item.CantExistente - item.CantTransferido;
        });
        return transferenciaDetails;
    }

    public async Task<List<AjusteInventarioDetail>> CalcularCantidadExistenteAjusteInventarioAsync(
        List<AjusteInventarioDetail> ajusteInventarioDetails, string warehouseId)
    {
        var productArrId = new List<string>();
        ajusteInventarioDetails.ForEach(item => productArrId.Add(item.ProductId));
        var productStocks = await GetProductStockByWarehouseIdAsync(warehouseId, productArrId);
        ajusteInventarioDetails.ForEach(item =>
        {
            item.Id = string.Empty;
            var products = productStocks.Where(x => x.ProductId == item.ProductId).ToList();
            var entrada = products.Where(x => x.Type == InventoryType.ENTRADA).Sum(x => x.Quantity);
            var salida = products.Where(x => x.Type == InventoryType.SALIDA).Sum(x => x.Quantity);
            item.CantExistente = entrada - salida;
        });
        return ajusteInventarioDetails;
    }

    public async Task<DeleteResult> DeleteProductStockByWarehouseIdAsync(string warehouseId, List<string> productArrId)
    {
        var builder = Builders<ProductStock>.Filter;
        var filter = builder.And(builder.Eq(x => x.WarehouseId, warehouseId), builder.In("ProductId", productArrId));
        return await _collection.DeleteManyAsync(filter);
    }

    public async Task<List<ProductStock>> GetProductStockByWarehouseIdAndProductIdAsync(string warehouseId,
        string productId)
    {
        var builder = Builders<ProductStock>.Filter;
        var filter = builder.And(builder.Eq(x => x.WarehouseId, warehouseId), builder.Eq(x => x.ProductId, productId));
        return await _collection.Find(filter).ToListAsync();
    }

    public async Task<List<ProductStock>> GetProductStockByWarehouseIdAsync(string warehouseId,
        List<string> productArrId)
    {
        var builder = Builders<ProductStock>.Filter;
        var filter = builder.And(builder.Eq(x => x.WarehouseId, warehouseId), builder.In("ProductId", productArrId));
        return await _collection.Find(filter).ToListAsync();
    }

    /// <summary>
    /// Devuelve la lista de stock de productos correspondiente a un ID de producto y a una lista de IDs de almacenes.
    /// </summary>
    /// <param name="productId">ID del producto a buscar.</param>
    /// <param name="warehouseArrId">lista de IDs de almacenes en los que se busca el stock del producto.</param>
    /// <returns>Retorna una lista de objetos ProductStock que contienen información del stock del producto en los almacenes especificados.</returns>
    public async Task<List<ProductStock>> GetProductStockByProductIdAsync(string productId, List<string> warehouseArrId)
    {
        var builder = Builders<ProductStock>.Filter;
        var filter = builder.And(builder.Eq(x => x.ProductId, productId), builder.In("WarehouseId", warehouseArrId));
        return await _collection.Find(filter).ToListAsync();
    }
}
