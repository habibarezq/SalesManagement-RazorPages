using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using SalesManagement.Models;

namespace SalesManagement.Pages.Products
{
    public class EditProductModel : PageModel
    {
        private readonly SalesManagementDbContext _context;

        public EditProductModel(SalesManagementDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Product Product { get; set; } = default!;

        public SelectList Categories { get; set; } = default!;

        public IActionResult OnGet(int id)
        {
            var product = _context.Products.Find(id);
            if (product == null)
                return NotFound();

            Product = product;
            LoadCategories();
            return Page();
        }

        public IActionResult OnPost()
        {
            ModelState.Remove("Product.Category");

            if (!ModelState.IsValid)
            {
                LoadCategories();
                return Page();
            }

            _context.Products.Update(Product);
            _context.SaveChanges();

            return RedirectToPage("Index");
        }

        private void LoadCategories()
        {
            Categories = new SelectList(_context.Categories.ToList(), "CategoryId", "Name", Product?.CategoryId);
        }
    }
}