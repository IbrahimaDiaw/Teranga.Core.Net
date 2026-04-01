namespace Teranga.Core.Models
{
    /// <summary>
    /// Generic page result.
    /// </summary>
    public sealed class PagedResult<T>
    {
        /// <summary>
        /// Items in the current page.
        /// </summary>
        public IReadOnlyList<T> Items { get; init; } = [];

        /// <summary>
        /// Current page number.
        /// </summary>
        public int Page { get; init; }

        /// <summary>
        /// Requested page size.
        /// </summary>
        public int PageSize { get; init; }

        /// <summary>
        /// Total item count before pagination.
        /// </summary>
        public int TotalCount { get; init; }

        /// <summary>
        /// Total page count.
        /// </summary>
        public int TotalPages { get; init; }

        /// <summary>
        /// Whether a previous page exists.
        /// </summary>
        public bool HasPreviousPage { get; init; }

        /// <summary>
        /// Whether a next page exists.
        /// </summary>
        public bool HasNextPage { get; init; }
    }
}
