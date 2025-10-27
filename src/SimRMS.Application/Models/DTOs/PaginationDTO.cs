/// <summary>
/// <para>
/// ===================================================================
/// Title:       Pagination Model DTO Class
/// Author:      Md. Raihan Sharif
/// Purpose:     This class serves as the base Pagination for all entities, providing Pagination data.
/// Creation:    26/Oct/2025
/// ===================================================================
/// Modification History
/// Author             Date         Description of Change
/// -------------------------------------------------------------------
///
/// 
/// ===================================================================
/// </para>
/// </summary>
/// 

namespace SimRMS.Application.Models.DTOs
{
    public class PaginationDTO
    {
        /// <summary>
        /// The current page number being viewed.
        /// Corresponds to SQL variable @PageNumber.
        /// </summary>
        public int CurrentPage { get; set; }

        /// <summary>
        /// The maximum number of records per page.
        /// Corresponds to SQL variable @PageSize.
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// The total number of records across all pages.
        /// Corresponds to SQL variable @TotalCount.
        /// </summary>
        public int TotalRecords { get; set; }

        /// <summary>
        /// The total number of pages available.
        /// Corresponds to SQL variable @TotalPages.
        /// </summary>
        public int TotalPages { get; set; }

        /// <summary>
        /// A flag indicating if a previous page exists (1 = True, 0 = False).
        /// </summary>
        public int HasPreviousPage { get; set; }

        /// <summary>
        /// A flag indicating if a next page exists (1 = True, 0 = False).
        /// </summary>
        public int HasNextPage { get; set; }

        /// <summary>
        /// The page number of the previous page (or 1 if at the first page).
        /// </summary>
        public int PreviousPage { get; set; }

        /// <summary>
        /// The page number of the next page (or TotalPages if at the last page).
        /// </summary>
        public int NextPage { get; set; }

        /// <summary>
        /// The column name used for sorting.
        /// Corresponds to SQL variable @SortColumn.
        /// </summary>
        public string? SortBy { get; set; }

        /// <summary>
        /// The direction of the sort (e.g., "ASC" or "DESC").
        /// Corresponds to SQL variable @SortDirection.
        /// </summary>
        public string? SortOrder { get; set; }
    }
}
