using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Nebula.Modules.Inventory.Helpers;
using Nebula.Modules.Inventory.Models;
using Nebula.Modules.Inventory.Stock.Dto;
using MongoDB.Bson;
using Nebula.Common;

namespace Nebula.Modules.Inventory.Stock;

public interface IProductStockService : ICrudOperationService<ProductStock>
{
    Task<List<ProductStock>> CreateManyAsync(List<ProductStock> obj);
    Task<DeleteResult> DeleteAllByWarehouseAsync(string warehouseId, string productId);
    Task<DeleteResult> DeleteAllByWarehouseAndLoteAsync(string warehouseId, string productLoteId, string productId);
    Task<ProductStock> ChangeQuantity(ChangeQuantityStockRequestParams requestParams);
    Task<long> GetStockQuantityByWarehouseAsync(string warehouseId, string productId);
    Task<long> GetLoteStockQuantityByWarehouseAsync(string warehouseId, string productLoteId, string productId);
    Task<List<ProductStock>> GetProductStockListByWarehouseAndLoteAsync(string warehouseId, string productLoteId, string productId);
    Task<List<ProductStock>> GetProductStockListByWarehouseAsync(string warehouseId, string productId);
    Task<List<ProductStock>> GetProductStockListByWarehousesIdsAsync(List<string> warehouseArrId, string productId);
    Task<List<ProductStock>> GetProductStockByWarehouseIdAsync(string warehouseId,
        List<string> productArrId);
    Task<List<TransferenciaDetail>> CalcularCantidadExistenteRestanteTransferenciaAsync(
        List<TransferenciaDetail> transferenciaDetails, string warehouseId);
    Task<List<AjusteInventarioDetail>> CalcularCantidadExistenteAjusteInventarioAsync(
        List<AjusteInventarioDetail> ajusteInventarioDetails, string warehouseId);
    Task<DeleteResult> DeleteProductStockByWarehouseIdAsync(string warehouseId, List<string> productArrId);
}

public class ProductStockService : CrudOperationService<ProductStock>, IProductStockService
{
    public ProductStockService(IOptions<DatabaseSettings> options) : base(options) { }

    public async Task<List<ProductStock>> CreateManyAsync(List<ProductStock> obj)
    {
        await _collection.InsertManyAsync(obj);
        return obj;
    }

    /// <summary>
    /// Elimina registros de stocks de la base de datos basados en los identificadores de almacén y producto.
    /// </summary>
    /// <param name="warehouseId">Identificador del almacén</param>
    /// <param name="productId">Identificador del producto</param>
    /// <returns>El resultado de la operación de eliminación</returns>
    public async Task<DeleteResult> DeleteAllByWarehouseAsync(string warehouseId, string productId)
    {
        var filter = Builders<ProductStock>.Filter;
        var dbQuery = filter.And(filter.Eq(x => x.WarehouseId, warehouseId),
            filter.Eq(x => x.ProductId, productId));
        return await _collection.DeleteManyAsync(dbQuery);
    }

    /// <summary>
    /// Elimina registros de stocks de la base de datos basados en los identificadores de almacén lote y producto.
    /// </summary>
    /// <param name="warehouseId">Identificador del almacén</param>
    /// <param name="productLoteId">Identificador de lote de producción</param>
    /// <param name="productId">Identificador del producto</param>
    /// <returns>El resultado de la operación de eliminación</returns>
    public async Task<DeleteResult> DeleteAllByWarehouseAndLoteAsync(string warehouseId, string productLoteId, string productId)
    {
        var filter = Builders<ProductStock>.Filter;
        var dbQuery = filter.And(filter.Eq(x => x.WarehouseId, warehouseId),
            filter.Eq(x => x.ProductLoteId, productLoteId),
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
        // validar si identificador de lote es un objectId valido.
        if (ObjectId.TryParse(requestParams.ProductLoteId, out _))
            await DeleteAllByWarehouseAndLoteAsync(requestParams.WarehouseId, requestParams.ProductLoteId, requestParams.ProductId);
        else
            await DeleteAllByWarehouseAsync(requestParams.WarehouseId, requestParams.ProductId);
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

    /// <summary>
    /// Calcular la cantidad neta de existencias.
    /// </summary>
    /// <param name="productStocks">Lista de Stocks</param>
    /// <returns>Valor Calculado</returns>
    private long CalculateNetStockQuantity(List<ProductStock> productStocks)
    {
        var totalEntrada = productStocks.Where(x => x.Type == InventoryType.ENTRADA).Sum(x => x.Quantity);
        var totalSalida = productStocks.Where(x => x.Type == InventoryType.SALIDA).Sum(x => x.Quantity);
        return totalEntrada - totalSalida;
    }

    /// <summary>
    /// Obtener la cantidad de existencias por almacén.
    /// </summary>
    /// <param name="warehouseId">Identificador del almacén</param>
    /// <param name="productId">Identificador del producto</param>
    /// <returns>Cantidad de Existencias</returns>
    public async Task<long> GetStockQuantityByWarehouseAsync(string warehouseId, string productId)
    {
        var productStocks = await GetProductStockListByWarehouseAsync(warehouseId, productId);
        return CalculateNetStockQuantity(productStocks);
    }

    /// <summary>
    /// Obtener la cantidad de existencias de un lote por almacén.
    /// </summary>
    /// <param name="warehouseId">Identificador del almacén</param>
    /// <param name="productLoteId">Identificador del lote del producto</param>
    /// <param name="productId">Identificador del producto</param>
    /// <returns>Cantidad de Existencias</returns>
    public async Task<long> GetLoteStockQuantityByWarehouseAsync(string warehouseId, string productLoteId, string productId)
    {
        var productStocks = await GetProductStockListByWarehouseAndLoteAsync(warehouseId, productLoteId, productId);
        return CalculateNetStockQuantity(productStocks);
    }

    /// <summary>
    /// Obtiene la lista de existencias de productos por almacén y lote.
    /// </summary>
    /// <param name="warehouseId">Identificador del almacén</param>
    /// <param name="productLoteId">Identificador del producto</param>
    /// <param name="productId">Identificador del lote</param>
    /// <returns>Lista de existencias de productos</returns>
    public async Task<List<ProductStock>> GetProductStockListByWarehouseAndLoteAsync(string warehouseId, string productLoteId, string productId)
    {
        var builder = Builders<ProductStock>.Filter;
        var filter = builder.And(builder.Eq(x => x.WarehouseId, warehouseId),
            builder.Eq(x => x.ProductLoteId, productLoteId),
            builder.Eq(x => x.ProductId, productId));
        return await _collection.Find(filter).ToListAsync();
    }

    /// <summary>
    /// Obtiene la lista de existencias de productos por almacén.
    /// </summary>
    /// <param name="warehouseId">Identificador del almacén</param>
    /// <param name="productId">Identificador del producto</param>
    /// <returns>Lista de existencias de productos</returns>
    public async Task<List<ProductStock>> GetProductStockListByWarehouseAsync(string warehouseId, string productId)
    {
        var builder = Builders<ProductStock>.Filter;
        var filter = builder.And(builder.Eq(x => x.WarehouseId, warehouseId),
            builder.Eq(x => x.ProductId, productId));
        return await _collection.Find(filter).ToListAsync();
    }

    /// <summary>
    /// Obtener la lista de stock de productos correspondiente a un ID de producto y a una lista de IDs de almacenes.
    /// </summary>
    /// <param name="productId">ID del producto a buscar.</param>
    /// <param name="warehouseArrId">lista de IDs de almacenes en los que se busca el stock del producto.</param>
    /// <returns>Retorna una lista de objetos ProductStock que contienen información del stock del producto en los almacenes especificados.</returns>
    public async Task<List<ProductStock>> GetProductStockListByWarehousesIdsAsync(List<string> warehouseArrId, string productId)
    {
        var builder = Builders<ProductStock>.Filter;
        var filter = builder.And(builder.Eq(x => x.ProductId, productId), builder.In("WarehouseId", warehouseArrId));
        return await _collection.Find(filter).ToListAsync();
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

    public async Task<List<ProductStock>> GetProductStockByWarehouseIdAsync(string warehouseId,
        List<string> productArrId)
    {
        var builder = Builders<ProductStock>.Filter;
        var filter = builder.And(builder.Eq(x => x.WarehouseId, warehouseId), builder.In("ProductId", productArrId));
        return await _collection.Find(filter).ToListAsync();
    }

}