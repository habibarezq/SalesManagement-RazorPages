using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SalesManagement.Models;
namespace SalesManagement.Pages.Products
{
    public class IndexModel  : PageModel
    {
        private readonly SalesManagementDbContext _context;
        public IndexModel(SalesManagementDbContext context)
        {
            _context = context;
        }

        public List<Product> products { get; set; } = new();

        public void OnGet()
        {
            products = _context.Products
                        .Include(p => p.Category) // , this makes EF Core generate a SQL query with a JOIN to get the Category itself using the Fk
                        .ToList();
        }

        public IActionResult OnPostDelete(int id)
        {
            var product = _context.Products.Find(id);
            if (product == null) return NotFound();

            bool inUse =
                _context.SalesItems.Any(x => x.ProductId == id) ||
                _context.PurchaseInvoiceItems.Any(x => x.ProductId == id) ||
                _context.StockHistories.Any(x => x.ProductId == id);

            if (inUse)
            {
                TempData["Error"] = "This product has invoices or stock history and can't be deleted.";
                return RedirectToPage();
            }

            _context.Products.Remove(product);
            _context.SaveChanges();
            TempData["Success"] = "Product deleted.";
            return RedirectToPage();
        }
    }
}
