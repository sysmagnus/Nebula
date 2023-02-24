using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Nebula.Database.Models.Sales;

namespace Nebula.Database.Services.Sales;

public class DetallePagoSaleService : CrudOperationService<DetallePagoSale>
{
    public DetallePagoSaleService(IOptions<DatabaseSettings> options) : base(options) { }

    public async Task<List<DetallePagoSale>> GetListAsync(string id) =>
        await _collection.Find(x => x.InvoiceSale == id).ToListAsync();

    public async Task CreateManyAsync(List<DetallePagoSale> detallePago) =>
        await _collection.InsertManyAsync(detallePago);
}