using Models;
using Services.CategoryService.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.CategoryService
{
    public class CategoryIdRetriver : ICategoryIdRetriver
    {
        public IEnumerable<int> GetIdsOfCategories(IEnumerable<Category> categories)
        {
            var categoryFilteredIds = categories.Select(c => c.Id);
            return categoryFilteredIds;
        }
    }
}
