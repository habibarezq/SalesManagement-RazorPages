using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SalesManagement.Models;
namespace SalesManagement.Pages.Customers
{
    public class IndexModel : PageModel
    {
        private readonly SalesManagementDbContext _context;
        public IndexModel(SalesManagementDbContext context)
        {
            _context = context;
        }

        public List<Customer> customers { get; set; } = new();

        public void OnGet()
        {
            customers = _context.Customers
                        .ToList();
        }

        public IActionResult OnPostDelete(int id)
        {
            var customer = _context.Customers.Find(id);
            if (customer == null) return NotFound();

            if (_context.SalesInvoices.Any(i => i.CustomerId == id))
            {
                TempData["Error"] = "This customer has invoices and can't be deleted.";
                return RedirectToPage("Index");
            }

            _context.Customers.Remove(customer);
            _context.SaveChanges();
            TempData["Success"] = "Customer deleted.";
            return RedirectToPage("Index");
        }
    }
}
