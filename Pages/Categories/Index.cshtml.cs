using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SalesManagement.Models;

namespace SalesManagement.Pages.Categories
{
    public class IndexModel : PageModel
    {
        private readonly SalesManagementDbContext _context;
        public IndexModel(SalesManagementDbContext context) => _context = context;

        public class CategoryRow
        {
            public int CategoryId { get; set; }
            public string Name { get; set; } = "";
            public int ProductCount { get; set; }
        }

        public List<CategoryRow> Categories { get; set; } = new();

        [BindProperty]
        [Required(ErrorMessage = "Enter a category name.")]
        [StringLength(100, ErrorMessage = "Keep the name under 100 characters.")]
        public string? NewName { get; set; }

        public async Task OnGetAsync() => await LoadAsync();

        public async Task<IActionResult> OnPostAddAsync()
        {
            if (!ModelState.IsValid)
            {
                await LoadAsync();
                return Page();
            }

            var name = NewName!.Trim();
            if (await _context.Categories.AnyAsync(c => c.Name == name))
            {
                ModelState.AddModelError(nameof(NewName), "That category already exists.");
                await LoadAsync();
                return Page();
            }

            _context.Categories.Add(new Category { Name = name });
            await _context.SaveChangesAsync();
            TempData["Success"] = "Category added.";
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null) return NotFound();

            if (await _context.Products.AnyAsync(p => p.CategoryId == id))
            {
                TempData["Error"] = "This category has products and can't be deleted.";
                return RedirectToPage();
            }

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Category deleted.";
            return RedirectToPage();
        }

        private async Task LoadAsync()
        {
            Categories = await _context.Categories
                .AsNoTracking()
                .OrderBy(c => c.Name)
                .Select(c => new CategoryRow
                {
                    CategoryId = c.CategoryId,
                    Name = c.Name,
                    ProductCount = c.Products.Count
                })
                .ToListAsync();
        }
    }
}
