﻿using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Nebula.Data.Models.Sales;

namespace Nebula.Data.Services.Sales;

public class InvoiceSaleService
{
    private readonly IMongoCollection<InvoiceSale> _collection;

    public InvoiceSaleService(IOptions<DatabaseSettings> options)
    {
        var mongoClient = new MongoClient(options.Value.ConnectionString);
        var mongoDatabase = mongoClient.GetDatabase(options.Value.DatabaseName);
        _collection = mongoDatabase.GetCollection<InvoiceSale>("InvoiceSales");
    }

    public async Task CreateAsync(InvoiceSale invoiceSale) =>
        await _collection.InsertOneAsync(invoiceSale);
}
