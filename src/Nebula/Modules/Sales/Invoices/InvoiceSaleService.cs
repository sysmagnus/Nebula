using MongoDB.Driver;
using Nebula.Common;
using Nebula.Modules.Sales.Models;
using Nebula.Common.Dto;
using Nebula.Modules.Sales.Invoices.Dto;

namespace Nebula.Modules.Sales.Invoices;

public interface IInvoiceSaleService : ICrudOperationService<InvoiceSale>
{
    Task<List<InvoiceSale>> GetListAsync(string companyId, DateQuery query);
    Task<ResponseInvoiceSale> GetInvoiceSaleAsync(string companyId, string invoiceSaleId);
    Task<List<InvoiceSale>> GetByContactIdAsync(string companyId, string contactId, string month, string year);
    Task<List<InvoiceSale>> GetInvoicesByNumDocs(string companyId, List<string> series, List<string> numbers);
    Task<List<InvoiceSale>> GetInvoiceSaleByMonthAndYear(string companyId, string month, string year);
    Task<List<InvoiceSale>> GetInvoiceSaleByDate(string companyId, string date);
    Task<InvoiceSale> AnularComprobante(string companyId, string invoiceSaleId);
    Task<List<InvoiceSale>> GetInvoiceSalesPendingAsync(string companyId);
    Task<List<InvoiceSale>> BusquedaAvanzadaAsync(string companyId, BuscarComprobanteFormDto dto);
}

public class InvoiceSaleService : CrudOperationService<InvoiceSale>, IInvoiceSaleService
{
    private readonly IInvoiceSaleDetailService _invoiceSaleDetailService;

    public InvoiceSaleService(MongoDatabaseService mongoDatabase,
        IInvoiceSaleDetailService invoiceSaleDetailService) : base(mongoDatabase)
    {
        _invoiceSaleDetailService = invoiceSaleDetailService;
    }

    public async Task<List<InvoiceSale>> GetListAsync(string companyId, DateQuery query)
    {
        var builder = Builders<InvoiceSale>.Filter;
        var filter = builder.And(
            builder.Eq(x => x.CompanyId, companyId),
            builder.Eq(x => x.Month, query.Month),
            builder.Eq(x => x.Year, query.Year),
            builder.In("TipoDoc", new List<string>() { "03", "01" }));
        return await _collection.Find(filter).Sort(new SortDefinitionBuilder<InvoiceSale>().Descending("$natural"))
            .ToListAsync();
    }

    public async Task<ResponseInvoiceSale> GetInvoiceSaleAsync(string companyId, string invoiceSaleId)
    {
        var invoiceSale = await GetByIdAsync(companyId, invoiceSaleId);
        var invoiceSaleDetails = await _invoiceSaleDetailService.GetListAsync(companyId, invoiceSale.Id);
        return new ResponseInvoiceSale()
        {
            InvoiceSale = invoiceSale,
            InvoiceSaleDetails = invoiceSaleDetails
        };
    }

    public async Task<List<InvoiceSale>> GetByContactIdAsync(string companyId, string contactId, string month, string year)
    {
        var builder = Builders<InvoiceSale>.Filter;
        var filter = builder.And(builder.Eq(x => x.CompanyId, companyId),
            builder.Eq(x => x.ContactId, contactId),
            builder.Eq(x => x.Month, month), builder.Eq(x => x.Year, year));
        return await _collection.Find(filter).ToListAsync();
    }

    /// <summary>
    /// Obtener comprobantes por serie y números.
    /// </summary>
    /// <param name="companyId">Identificador empresa</param>
    /// <param name="series">series</param>
    /// <param name="correlativos">correlativos</param>
    /// <returns></returns>
    public async Task<List<InvoiceSale>> GetInvoicesByNumDocs(string companyId, List<string> series, List<string> correlativos)
    {
        var builder = Builders<InvoiceSale>.Filter;
        var filter = builder.And(builder.Eq(x => x.CompanyId, companyId),
            builder.In(x => x.Serie, series), builder.In(x => x.Correlativo, correlativos));
        return await _collection.Find(filter).ToListAsync();
    }

    /// <summary>
    /// Obtener lista de comprobantes por mes y año.
    /// </summary>
    /// <param name="month">mes</param>
    /// <param name="year">año</param>
    /// <returns>Lista de comprobantes</returns>
    public async Task<List<InvoiceSale>> GetInvoiceSaleByMonthAndYear(string companyId, string month, string year)
    {
        var builder = Builders<InvoiceSale>.Filter;
        var filter = builder.And(builder.Eq(x => x.CompanyId, companyId),
            builder.Eq(x => x.Month, month), builder.Eq(x => x.Year, year));
        return await _collection.Find(filter).ToListAsync();
    }

    /// <summary>
    /// Obtener lista de comprobantes de una fecha especifica.
    /// </summary>
    /// <param name="date">fecha de emisión</param>
    /// <returns>lista de comprobantes</returns>
    public async Task<List<InvoiceSale>> GetInvoiceSaleByDate(string companyId, string date)
    {
        var builder = Builders<InvoiceSale>.Filter;
        var filter = builder.And(builder.Eq(x => x.CompanyId, companyId), builder.Eq(x => x.FechaEmision, date));
        return await _collection.Find(filter).ToListAsync();
    }

    public async Task<InvoiceSale> AnularComprobante(string companyId, string invoiceSaleId)
    {
        var invoiceSale = await GetByIdAsync(companyId, invoiceSaleId);
        invoiceSale.Anulada = true;
        invoiceSale = await UpdateAsync(invoiceSale.Id, invoiceSale);
        return invoiceSale;
    }

    public async Task<List<InvoiceSale>> GetInvoiceSalesPendingAsync(string companyId)
    {
        var builder = Builders<InvoiceSale>.Filter;
        var filter = builder.And(builder.Eq(x => x.CompanyId, companyId),
            builder.Not(builder.Eq(x => x.TipoDoc, "NOTA")),
            builder.Eq(x => x.BillingResponse.Success, false));
        return await _collection.Find(filter).ToListAsync();
    }

    public async Task<List<InvoiceSale>> BusquedaAvanzadaAsync(string companyId, BuscarComprobanteFormDto dto)
    {
        var filterBuilder = Builders<InvoiceSale>.Filter;
        var filters = new List<FilterDefinition<InvoiceSale>>();

        filters.Add(filterBuilder.Eq(x => x.CompanyId, companyId));
        filters.Add(filterBuilder.Gte(x => x.FechaEmision, dto.FechaDesde));
        filters.Add(filterBuilder.Lte(x => x.FechaEmision, dto.FechaHasta));

        if (!string.IsNullOrEmpty(dto.ContactId))
        {
            filters.Add(filterBuilder.Eq(x => x.ContactId, dto.ContactId));
        }

        var query = filterBuilder.And(filters);
        return await _collection.Find(query).ToListAsync();
    }

}
