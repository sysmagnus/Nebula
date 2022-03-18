using Nebula.Data.Services;

namespace Nebula.Data.Helpers;

public static class PaginationHelper
{
    public static PagedResponse<List<T>> CreatePagedResponse<T>(List<T> pagedData, PaginationFilter validFilter,
        int totalRecords, IUriService uriService, string route)
    {
        var response = new PagedResponse<List<T>>(pagedData, validFilter.PageNumber, validFilter.PageSize);
        var totalPages = ((double)totalRecords / (double)validFilter.PageSize);
        int roundedTotalPages = Convert.ToInt32(Math.Ceiling(totalPages));
        response.NextPage =
            validFilter.PageNumber >= 1 && validFilter.PageNumber < roundedTotalPages
                ? uriService.GetPageUri(
                    new PaginationFilter(validFilter.PageNumber + 1, validFilter.PageSize, validFilter.Query),
                    route)
                : null;
        response.PreviousPage =
            validFilter.PageNumber - 1 >= 1 && validFilter.PageNumber <= roundedTotalPages
                ? uriService.GetPageUri(
                    new PaginationFilter(validFilter.PageNumber - 1, validFilter.PageSize, validFilter.Query),
                    route)
                : null;
        response.FirstPage =
            uriService.GetPageUri(new PaginationFilter(1, validFilter.PageSize, validFilter.Query), route);
        response.LastPage =
            uriService.GetPageUri(new PaginationFilter(roundedTotalPages, validFilter.PageSize, validFilter.Query),
                route);
        response.TotalPages = roundedTotalPages;
        response.TotalRecords = totalRecords;
        return response;
    }
}
