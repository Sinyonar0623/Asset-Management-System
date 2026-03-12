namespace Shared.Pagination
{
    /// <summary>
    /// Extension methods for working with paginated results.
    /// </summary>
    public static class PaginatedResultExtensions
    {
        /// <summary>
        /// Determines if there is a previous page of data.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <param name="result">The paginated result.</param>
        /// <returns>True if there is a previous page; otherwise, false.</returns>
        public static bool HasPreviousPage<T>(this PaginatedResult<T> result)
        {
            return result.PageNumber > 0;
        }

        /// <summary>
        /// Determines if there is a next page of data.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <param name="result">The paginated result.</param>
        /// <returns>True if there is a next page; otherwise, false.</returns>
        public static bool HasNextPage<T>(this PaginatedResult<T> result)
        {
            return (result.PageNumber + 1) * result.PageSize < result.Count;
        }

        /// <summary>
        /// Calculates the total number of pages.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <param name="result">The paginated result.</param>
        /// <returns>The total number of pages.</returns>
        public static int TotalPages<T>(this PaginatedResult<T> result)
        {
            return (int)Math.Ceiling((double)result.Count / result.PageSize);
        }

        /// <summary>
        /// Projects each element of a paginated result into a new form.
        /// </summary>
        /// <typeparam name="T">The source type.</typeparam>
        /// <typeparam name="TProjection">The target type.</typeparam>
        /// <param name="result">The paginated result.</param>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <returns>A paginated result whose elements are the result of invoking the transform function on each element of source.</returns>
        public static PaginatedResult<TProjection> Select<T, TProjection>(
            this PaginatedResult<T> result, 
            Func<T, TProjection> selector)
        {
            var projectedItems = result.Items.Select(selector);
            return new PaginatedResult<TProjection>(projectedItems, result.Count, result.PageNumber, result.PageSize);
        }
    }
}
