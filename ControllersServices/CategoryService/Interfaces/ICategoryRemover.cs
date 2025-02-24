namespace Services.CategoryService.Interfaces
{
    public interface ICategoryRemover
    {
        /// <summary>
        /// Deletes the specified category, including all its subcategories and the products contained within.
        /// </summary>
        /// <param name="id">The ID of the category to delete. If null, no action is performed.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task DeleteAsync(int? id);
    }
}