using System.Reflection;
using System.Linq.Dynamic.Core;
using Microsoft.EntityFrameworkCore;

namespace LibraryProjectAPI.DTO
{
    public class ApiResult<T>
    {
        public List<T> Data { get; private set; }
        public int PageIndex { get; private set; }
        public int PageSize { get; private set; }
        public int TotalCount { get; private set; }
        public int TotalPages { get; private set; }
        public string? SortColumn { get; private set; }
        public string? SortOrder { get; private set; }
        public string? FilterColumn { get; private set; }
        public string? FilterQuery { get; private set; }


        private ApiResult(
            List<T> data,
            int count,
            int pageIndex,
            int pageSize,
            string? sortColumn,
            string? sortOrder,
            string? filterColumn,
            string? filterQuery)
        {
            Data = data;
            PageIndex = pageIndex;
            PageSize = pageSize;
            TotalCount = count;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            SortColumn = sortColumn;
            SortOrder = sortOrder;
            FilterColumn = filterColumn;
            FilterQuery = filterQuery;
        }


        public static async Task<ApiResult<T>> CreateAsync(
            IQueryable<T> source,
            int pageIndex,
            int pageSize,
            string? sortColumn = null,
            string? sortOrder = null,
            string? filterColumn = null,
            string? filterQuery = null)
        {
            if (!string.IsNullOrEmpty(filterColumn) && !string.IsNullOrEmpty(filterQuery) &&
                IsValidProperty(filterColumn))
            {
                source = source.Where(string.Format("{0}.Contains(@0)", filterColumn), filterQuery);
            }

            int count = await source.CountAsync();

            if (!string.IsNullOrEmpty(sortOrder) && !string.IsNullOrEmpty(sortColumn) && IsValidProperty(sortColumn))
            {
                sortOrder = !string.IsNullOrEmpty(sortOrder)
                            && sortOrder.ToUpper() == "ASC"
                    ? "ASC"
                    : "DESC";

                source = source.OrderBy(
                    string.Format(
                        "{0} {1}",
                        sortColumn,
                        sortOrder)
                );
            }

            source = source
                .Skip(pageIndex * pageSize)
                .Take(pageSize);

            var list = await source.ToListAsync();
            return new ApiResult<T>(
                list,
                count,
                pageIndex,
                pageSize,
                sortColumn,
                sortOrder,
                filterColumn,
                filterQuery);
        }
         
        

        /// <summary>
        /// Checks if the given property name exists
        /// to protect against SQL injection attacks
        /// </summary>
        public static bool IsValidProperty(
            string propertyName,
            bool throwExceptionIfNotFound = false)
        {
            var prop = typeof(T).GetProperty(
                propertyName,
                BindingFlags.IgnoreCase |
                BindingFlags.Public |
                BindingFlags.Static |
                BindingFlags.Instance);
            if (prop == null && throwExceptionIfNotFound)
                throw new NotSupportedException($"ERROR: Property '{propertyName}' does not exist.");
            return prop != null;
        }

        ///<summary>
        /// TRUE if the current page has a previous page,
        /// FALSE otherwise.
        /// </summary>
        public bool HasPreviousPage => PageIndex > 0;

        /// <summary>
        /// TRUE if the current page has a next page, FALSE otherwise.
        /// </summary>
        public bool HasNextPage => ((PageIndex + 1) < TotalPages);
    }
}
