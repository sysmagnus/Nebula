using Nebula.Common;
using Nebula.Modules.Products.Models;

namespace Nebula.Modules.Products;

public interface ICategoryService : ICrudOperationService<Category>
{

}

public class CategoryService : CrudOperationService<Category>, ICategoryService
{
    public CategoryService(MongoDatabaseService mongoDatabase) : base(mongoDatabase)
    {
    }
}
