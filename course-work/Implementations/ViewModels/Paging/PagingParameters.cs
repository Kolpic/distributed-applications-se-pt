using System.ComponentModel.DataAnnotations;

namespace ProjectManagementAPI.ViewModels.Paging
{
    /// <summary>
    /// Base class for pagination, sorting, and filtering requests
    /// </summary>
    public class PagingParameters
    {
        /// <summary>
        /// Maximum number of items allowed per page
        /// </summary>
        private const int MaxPageSize = 50;

        /// <summary>
        /// Page number (1-based)
        /// </summary>
        /// <example>1</example>
        [Range(1, int.MaxValue, ErrorMessage = "Page number must be greater than 0")]
        public int PageNumber { get; set; } = 1;

        private int _pageSize = 10;

        /// <summary>
        /// Number of items per page
        /// </summary>
        /// <example>10</example>
        [Range(1, int.MaxValue, ErrorMessage = "Page size must be greater than 0")]
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = value > MaxPageSize ? MaxPageSize : value;
        }

        /// <summary>
        /// Property name to sort by
        /// </summary>
        /// <example>Id</example>
        [StringLength(50)]
        public string SortBy { get; set; } = "Id";

        /// <summary>
        /// Sorting direction (asc or desc)
        /// </summary>
        /// <example>asc</example>
        [RegularExpression("^(asc|desc)$", ErrorMessage = "Sort direction must be 'asc' or 'desc'")]
        public string SortDirection { get; set; } = "asc";
    }

    /// <summary>
    /// Represents the paginated response data
    /// </summary>
    /// <typeparam name="T">Type of items in the collection</typeparam>
    public class PagedResponse<T>
    {
        /// <summary>
        /// Collection of items for the current page
        /// </summary>
        public IEnumerable<T> Items { get; set; }

        /// <summary>
        /// Current page number
        /// </summary>
        public int PageNumber { get; set; }

        /// <summary>
        /// Number of items per page
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// Total number of items across all pages
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        /// Total number of pages
        /// </summary>
        public int TotalPages { get; set; }

        /// <summary>
        /// Whether there is a previous page
        /// </summary>
        public bool HasPrevious => PageNumber > 1;

        /// <summary>
        /// Whether there is a next page
        /// </summary>
        public bool HasNext => PageNumber < TotalPages;

        /// <summary>
        /// Creates a new paged response
        /// </summary>
        public PagedResponse(IEnumerable<T> items, int count, int pageNumber, int pageSize)
        {
            Items = items;
            TotalCount = count;
            PageNumber = pageNumber;
            PageSize = pageSize;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
        }
    }

    /// <summary>
    /// Extension methods for pagination
    /// </summary>
    public static class PaginationExtensions
    {
        /// <summary>
        /// Creates a paged list from an IQueryable
        /// </summary>
        /// <typeparam name="T">Type of the elements in the source</typeparam>
        /// <param name="source">Source queryable</param>
        /// <param name="pageNumber">Page number</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Paged response with the specified items</returns>
        public static PagedResponse<T> ToPagedResponse<T>(this IQueryable<T> source, int pageNumber, int pageSize)
        {
            var count = source.Count();
            var items = source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
            return new PagedResponse<T>(items, count, pageNumber, pageSize);
        }

        /// <summary>
        /// Applies sorting to an IQueryable based on property name and direction
        /// </summary>
        /// <typeparam name="T">Type of the elements in the source</typeparam>
        /// <param name="source">Source queryable</param>
        /// <param name="sortBy">Property name to sort by</param>
        /// <param name="sortDirection">Sort direction (asc or desc)</param>
        /// <returns>Sorted queryable</returns>
        public static IQueryable<T> ApplySort<T>(this IQueryable<T> source, string sortBy, string sortDirection)
        {
            if (string.IsNullOrEmpty(sortBy))
                return source;

            var parameter = System.Linq.Expressions.Expression.Parameter(typeof(T), "x");
            var property = System.Linq.Expressions.Expression.Property(parameter, sortBy);
            var lambda = System.Linq.Expressions.Expression.Lambda(property, parameter);

            var methodName = sortDirection.ToLower() == "desc" ? "OrderByDescending" : "OrderBy";
            var method = typeof(Queryable).GetMethods()
                .Where(m => m.Name == methodName && m.IsGenericMethodDefinition && m.GetParameters().Length == 2)
                .First();

            var genericMethod = method.MakeGenericMethod(typeof(T), property.Type);
            return (IQueryable<T>)genericMethod.Invoke(null, new object[] { source, lambda });
        }
    }
}
