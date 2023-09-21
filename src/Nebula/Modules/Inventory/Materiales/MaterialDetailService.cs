using MongoDB.Driver;
using Nebula.Common;
using Nebula.Modules.Inventory.Models;

namespace Nebula.Modules.Inventory.Materiales;

public interface IMaterialDetailService : ICrudOperationService<MaterialDetail>
{
    Task<List<MaterialDetail>> GetListAsync(string id);
    Task<long> CountDocumentsAsync(string id);
}

public class MaterialDetailService : CrudOperationService<MaterialDetail>, IMaterialDetailService
{
    public MaterialDetailService(MongoDatabaseService mongoDatabase) : base(mongoDatabase) { }

    public async Task<List<MaterialDetail>> GetListAsync(string id)
    {
        var filter = Builders<MaterialDetail>.Filter;
        var query = filter.Eq(x => x.MaterialId, id);
        return await _collection.Find(query).ToListAsync();
    }

    public async Task<long> CountDocumentsAsync(string id) =>
        await _collection.CountDocumentsAsync(x => x.MaterialId == id);
}
